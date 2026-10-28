using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using JG.Data.Entities.Factories;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Contexts.Common;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Factories.Contexts.Common;
using JG.Data.Entities.Interfaces.Factories.Models.Common;
using JG.Data.Entities.Interfaces.Models.Common;
using JG.Data.Entities.Interfaces.Repositories.Common;
using JG.Data.Generators.Extensions;
using JG.Data.Generators.Services;
using Microsoft.CSharp;

namespace JG.Data.Generators.Systems
{
    public class ContextGeneratorSystem
    {
        Type[] _types;
        public ContextGeneratorSystem()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName[2] == '.' || assembly.FullName[3] == '.')
                .SelectMany(a => a.GetTypes()).ToArray();
        }

        public void GenerateContexts()
        {
            var contextType = typeof(IContext);
            var contextInterfaces = _types
                .Where(type => type != contextType)
                .Where(type => contextType.IsAssignableFrom(type))
                .Where(type => type.IsInterface);
            
            foreach (var contextInterface in contextInterfaces)
            {
                GeneratePureContext(contextInterface);
                GenerateContextSave(contextInterface);
                GenerateContextSaveFactory(contextInterface);
            }
        }

        void GenerateContextSaveFactory(Type contextInterface)
        {
            var (factoryPath, className) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}Factories{Path.DirectorySeparatorChar}Contexts", "{0}ContextSaveFactory", "Context");
            var (saveContextPath, saveClassName) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ContextSave", "Context");
            var (saveCodeNamespace, _,_) = GeneratorService.GetNewClassConfig(contextInterface, saveContextPath, className, ignoreRootNamespace: true);
            var (codeNamespace, targetClass, _) = GeneratorService.GetNewClassConfig(contextInterface, factoryPath, className);
            var (pureContextPath, pureClassName) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}", "{0}Context", "Context");
            var (pureCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(contextInterface, pureContextPath, className);
            
            targetClass.BaseTypes.Add(new CodeTypeReference($"ContextFactoryBase<{contextInterface.Name}, {saveClassName}>"));
            targetClass.BaseTypes.Add(new CodeTypeReference($"IContextSaveFactory<{contextInterface.Name}, {saveClassName}>"));
            codeNamespace.Imports.Add(new CodeNamespaceImport(saveCodeNamespace.Name));
            codeNamespace.Imports.Add(new CodeNamespaceImport(contextInterface.Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(IContextSaveFactory<,>).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(pureCodeNamespace.Name));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(List<>).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(ContextFactoryBase<,>).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(IModelSaveFactory<,>).Namespace));

            var constructorRepositoriesParams = new HashSet<Type>();
            var constructorFactoriesParams = new HashSet<Type>();
            
            var keyProperty = new CodeMemberProperty();
            keyProperty.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            keyProperty.Name = nameof(IContextFactory.IgnoreSave);
            keyProperty.Type = new CodeTypeReference(typeof(bool));
            var ignoreSaveContext = contextInterface.GetCustomAttribute<IgnoreSaveAttribute>() != null;
            keyProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"return {ignoreSaveContext.ToString().ToLower()}")));
            targetClass.Members.Add(keyProperty);
            
            var lifeScopeContext = contextInterface.GetCustomAttribute<ContextLifeScopeAttribute>();
            if (lifeScopeContext != null && lifeScopeContext.scope != ContextLifeScope.Global)
            {
                var lifeScopeKeyProperty = new CodeMemberProperty();
                lifeScopeKeyProperty.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                lifeScopeKeyProperty.Name = nameof(IContextFactory.LifeScope);
                lifeScopeKeyProperty.Type = new CodeTypeReference(typeof(ContextLifeScope));
                lifeScopeKeyProperty.GetStatements.Add(
                    new CodeExpressionStatement(
                        new CodeSnippetExpression($"return ContextLifeScope.{Enum.GetName(typeof(ContextLifeScope), lifeScopeContext.scope)}")));
                targetClass.Members.Add(lifeScopeKeyProperty);
                codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(ContextLifeScope).Namespace));
            }


            CodeMemberMethod createPureMethod = new CodeMemberMethod();
            createPureMethod.Name = "CreateContextInternal";
            createPureMethod.ReturnType = new CodeTypeReference(contextInterface);
            createPureMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var context = new {pureClassName}()")));
            foreach (var propertyInfo in contextInterface.GetProperties())
            {
                var defaultValue = propertyInfo.GetCustomAttribute<DefaultValueSnippetAttribute>()?.Value;
                var notNull = propertyInfo.GetCustomAttribute<NotNullAttribute>() != null;

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    createPureMethod.Statements.Add(new CodeExpressionStatement(
                        new CodeSnippetExpression(
                            $"context.{propertyInfo.Name} = {defaultValue}")));
                }
                else
                {
                    if (propertyInfo.PropertyType.ToString().Contains("Generic.List"))
                    {
                        createPureMethod.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression(
                                $"context.{propertyInfo.Name} = new List<{propertyInfo.PropertyType.GetGenericArguments()[0]}>()")));
                    } 
                    else if (propertyInfo.PropertyType.ToString().Contains("Generic.Dictionary"))
                    {
                        createPureMethod.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression(
                                $"context.{propertyInfo.Name} = new Dictionary<{propertyInfo.PropertyType.GetGenericArguments()[0]}, {propertyInfo.PropertyType.GetGenericArguments()[1]}>()")));
                    }
                    else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (notNull)
                        {
                            createPureMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Factory.Create()")));
                            constructorFactoriesParams.Add(propertyInfo.PropertyType);
                        }
                    }
                }
            }
            createPureMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("context") ) );
            createPureMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            targetClass.Members.Add(createPureMethod);
            
            CodeMemberMethod createSaveMethod = new CodeMemberMethod();
            createSaveMethod.Name = "CreateSaveInternal";
            createSaveMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(new CodeTypeReference(contextInterface), "context"));
            createSaveMethod.ReturnType = new CodeTypeReference(saveClassName);
            createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var saveContext = new {saveClassName}()")));
            foreach (var propertyInfo in contextInterface.GetProperties())
            {
                if (ignoreSaveContext || propertyInfo.GetCustomAttribute<IgnoreSaveAttribute>() != null)
                {
                    continue;
                }
                
                if (typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context?.{propertyInfo.Name}?.Id")));
                } 
                else if (propertyInfo.PropertyType.IsArray && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetElementType())
                         || propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context.{propertyInfo.Name}?.Select(p => p?.Id).ToArray()")));
                }
                else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Factory.CreateSaveInternal(context.{propertyInfo.Name})")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType);
                }
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetElementType().Name}Factory.CreateSaveInternal(sm)).ToArray()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetElementType());
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}Factory.CreateSaveInternal(sm)).ToArray()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetGenericArguments()[0]);
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List"))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context.{propertyInfo.Name} ?? new List<{propertyInfo.PropertyType.GetGenericArguments()[0].Namespace}.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}>()")));
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.Dictionary"))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context.{propertyInfo.Name} ?? new Dictionary<{propertyInfo.PropertyType.GetGenericArguments()[0].Name}, {propertyInfo.PropertyType.GetGenericArguments()[1].Name}>()")));
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = (int)context?.{propertyInfo.Name}")));
                }
                else
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveContext.{propertyInfo.Name} = context.{propertyInfo.Name}")));
                }
            }
            
            createSaveMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("saveContext") ) );
            createSaveMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            targetClass.Members.Add(createSaveMethod);
            
            CodeMemberMethod createFromSaveMethod = new CodeMemberMethod();
            createFromSaveMethod.Name = "CreateContextInternal";
            createFromSaveMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(new CodeTypeReference(saveClassName), "saveContext"));
            createFromSaveMethod.ReturnType = new CodeTypeReference(contextInterface);
            createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var context = CreateContextInternal()")));

            foreach (var propertyInfo in contextInterface.GetProperties())
            {
                if (ignoreSaveContext || propertyInfo.GetCustomAttribute<IgnoreSaveAttribute>() != null)
                {
                    continue;
                }
                
                if (typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Repository.Get(saveContext.{propertyInfo.Name})")));
                    constructorRepositoriesParams.Add(propertyInfo.PropertyType);
                } 
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = saveContext.{propertyInfo.Name}?.Where(p => p != null).Select(entityId => this.{propertyInfo.PropertyType.GetElementType().Name}Repository.Get(entityId)).ToArray()")));
                    constructorRepositoriesParams.Add(propertyInfo.PropertyType.GetElementType());
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = saveContext.{propertyInfo.Name}?.Where(p => p != null).Select(entityId => this.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}Repository.Get(entityId)).ToList() ?? new List<{propertyInfo.PropertyType.GetGenericArguments()[0].Name}>()")));
                    constructorRepositoriesParams.Add(propertyInfo.PropertyType.GetGenericArguments()[0]);
                }
                else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Factory.Create(saveContext.{propertyInfo.Name})")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType);
                }
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = saveContext.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetElementType().Name}Factory.Create(sm)).ToArray()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetElementType());
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = saveContext.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}Factory.Create(sm)).ToList() ?? new List<{propertyInfo.PropertyType.GetGenericArguments()[0].Name}>()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetGenericArguments()[0]);
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = ({propertyInfo.PropertyType})saveContext?.{propertyInfo.Name}")));
                }
                else
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"context.{propertyInfo.Name} = saveContext.{propertyInfo.Name}")));
                }
            }

            createFromSaveMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("context") ) );
            createFromSaveMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            targetClass.Members.Add(createFromSaveMethod);
            
            CodeConstructor factoryConstructor = new CodeConstructor();
            factoryConstructor.Attributes = MemberAttributes.Public;
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(IEntityRepository<>).Namespace));
            foreach (var repositoryType in constructorRepositoriesParams)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(repositoryType.Namespace));
                targetClass.Members.Add(new CodeMemberField(new CodeTypeReference($"IEntityRepository<{repositoryType.Name}>"), $"{repositoryType.Name}Repository"));
                factoryConstructor.Parameters.Add( new CodeParameterDeclarationExpression(new CodeTypeReference($"IEntityRepository<{repositoryType.Name}>"), $"{repositoryType.Name}Repository") );
                factoryConstructor.Statements.Add(new CodeExpressionStatement(
                    new CodeSnippetExpression(
                        $"this.{repositoryType.Name}Repository = {repositoryType.Name}Repository")));
            }
            
            foreach (var modelType in constructorFactoriesParams)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(modelType.Namespace));
                var (saveModelPath, saveModelClassName) = GeneratorService.GetPath(modelType, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(contextInterface, saveModelPath, saveModelClassName, ignoreRootNamespace: true);
                codeNamespace.Imports.Add(new CodeNamespaceImport(saveModelCodeNamespace.Name));
                targetClass.Members.Add(new CodeMemberField(new CodeTypeReference($"IModelSaveFactory<{modelType.Name}, {saveModelClassName}>"), $"{modelType.Name}Factory"));
                factoryConstructor.Parameters.Add( new CodeParameterDeclarationExpression(new CodeTypeReference($"IModelSaveFactory<{modelType.Name}, {saveModelClassName}>"), $"{modelType.Name}Factory") );
                factoryConstructor.Statements.Add(new CodeExpressionStatement(
                    new CodeSnippetExpression(
                        $"this.{modelType.Name}Factory = {modelType.Name}Factory")));
            }
            
            targetClass.Members.Add(factoryConstructor);
            
            
            var targetUnit = new CodeCompileUnit();
            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);
            CodeGenExtensions.WriteSourceFile($"{factoryPath}.cs", new CSharpCodeProvider(), targetUnit);
        }

        void GeneratePureContext(Type contextInterface)
        {
            var (pureContextPath, className) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}", "{0}Context", "Context");
            
            var (codeNamespace, targetClass, properties) = GeneratorService.GetNewClassConfig(contextInterface, pureContextPath, className);
            targetClass.BaseTypes.Add(contextInterface);
            foreach (var propertyInfo in properties)
            {
                var property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Name = propertyInfo.Name;
                var fieldName = $"_{propertyInfo.Name[0].ToString().ToLower()}{propertyInfo.Name.Substring(1)}";
                property.Type = new CodeTypeReference(propertyInfo.PropertyType);
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),new CodePropertySetValueReferenceExpression()));

                codeNamespace.Imports.Add(new CodeNamespaceImport(propertyInfo.PropertyType.Namespace));
                targetClass.Members.Add(property);
                targetClass.Members.Add(new CodeMemberField(propertyInfo.PropertyType, fieldName));
            }
            
            // var keyProperty = new CodeMemberProperty();
            // keyProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            // keyProperty.Name = nameof(IContext.Key);
            // keyProperty.Type = new CodeTypeReference(typeof(string));
            // keyProperty.GetStatements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"return \"{className}\"")));
            // targetClass.Members.Add(keyProperty);
            
            var targetUnit = new CodeCompileUnit();
            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);
            CodeGenExtensions.WriteSourceFile($"{pureContextPath}.cs", new CSharpCodeProvider(), targetUnit);
        }
        
        void GenerateContextSave(Type contextInterface)
        {
            var (saveContextPath, className) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ContextSave", "Context");
            
            var (codeNamespace, targetClass, properties) = GeneratorService.GetNewClassConfig(contextInterface, saveContextPath, className, ignoreRootNamespace: true);
            foreach (var propertyInfo in properties)
            {
                var codeMemberField = new CodeMemberField();
                if (propertyInfo.GetCustomAttribute<IgnoreSaveAttribute>() != null)
                {
                    continue;
                }
                if (typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    codeMemberField.Type = new CodeTypeReference(typeof(string));
                    codeMemberField.Name = $"{propertyInfo.Name}";
                }
                // Костыль, тк если [] проверять на IEnumerable<> - юнити крашится
                else if (propertyInfo.PropertyType.IsArray && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetElementType())
                          || propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    codeMemberField.Type = new CodeTypeReference(typeof(string[]));
                    codeMemberField.Name = propertyInfo.Name;
                }
                else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var (saveModelPath, saveModelClassName) = GeneratorService.GetPath(propertyInfo.PropertyType, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                    var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propertyInfo.PropertyType, saveModelPath, saveModelClassName, ignoreRootNamespace: true);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(saveModelCodeNamespace.Name));
                    codeMemberField.Type = new CodeTypeReference(saveModelClassName);
                    codeMemberField.Name = $"{propertyInfo.Name}";
                }
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    var (saveModelPath, saveModelClassName) = GeneratorService.GetPath(propertyInfo.PropertyType.GetElementType(), $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                    var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propertyInfo.PropertyType.GetElementType(), saveModelPath, saveModelClassName, ignoreRootNamespace: true);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(saveModelCodeNamespace.Name));
                    codeMemberField.Type = new CodeTypeReference($"{saveModelClassName}[]");
                    codeMemberField.Name = propertyInfo.Name;
                }
                else if (propertyInfo.PropertyType.IsArray && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType())
                         || propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    var (saveModelPath, saveModelClassName) = GeneratorService.GetPath(propertyInfo.PropertyType.GetGenericArguments()[0], $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                    var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propertyInfo.PropertyType.GetGenericArguments()[0], saveModelPath, saveModelClassName, ignoreRootNamespace: true);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(saveModelCodeNamespace.Name));
                    codeMemberField.Type = new CodeTypeReference($"{saveModelClassName}[]");
                    codeMemberField.Name = propertyInfo.Name;
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    codeMemberField.Name = propertyInfo.Name;
                    codeMemberField.Type = new CodeTypeReference(typeof(int));
                }
                else
                {
                    codeMemberField.Name = propertyInfo.Name;
                    codeMemberField.Type = new CodeTypeReference(propertyInfo.PropertyType);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(propertyInfo.PropertyType.Namespace));
                }

                codeMemberField.Attributes = MemberAttributes.Public;
                targetClass.Members.Add(codeMemberField);
            }

            var targetUnit = new CodeCompileUnit();
            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);
            CodeGenExtensions.WriteSourceFile($"{saveContextPath}.cs", new CSharpCodeProvider(), targetUnit);
        }
    }
}