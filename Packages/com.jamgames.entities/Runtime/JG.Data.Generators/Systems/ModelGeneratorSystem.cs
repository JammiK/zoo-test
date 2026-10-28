using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Factories.Models.Common;
using JG.Data.Entities.Interfaces.Models.Common;
using JG.Data.Entities.Interfaces.Repositories.Common;
using JG.Data.Entities.Models;
using JG.Data.Generators.Extensions;
using JG.Data.Generators.Services;
using Microsoft.CSharp;
using UnityEditor;

namespace JG.Data.Generators.Systems
{
    public class ModelGeneratorSystem
    {
        Type[] _types;

        public ModelGeneratorSystem()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName[2] == '.' || assembly.FullName[3] == '.')
                .SelectMany(a => a.GetTypes()).ToArray();
        }

        public void GenerateModels()
        {
            var contextType = typeof(IModel);
            var modelInterfaces = _types
                .Where(type => type != contextType)
                .Where(type => contextType.IsAssignableFrom(type))
                .Where(type => type.IsInterface);
            
            foreach (var modelInterface in modelInterfaces)
            {
                GeneratePureModel(modelInterface);
                GenerateModelSave(modelInterface);
                GenerateModelSaveFactory(modelInterface);
            }
            
            AssetDatabase.Refresh();
        }

        void GenerateModelSaveFactory(Type modelInterface)
        {
            var (factoryPath, className) = GeneratorService.GetPath(modelInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}Factories{Path.DirectorySeparatorChar}Models", "{0}ModelSaveFactory", "Model");
            var (saveModelPath, saveClassName) = GeneratorService.GetPath(modelInterface, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
            var (saveCodeNamespace, _, classProperties) = GeneratorService.GetNewClassConfig(modelInterface, saveModelPath, className, ignoreRootNamespace: true);
            var (codeNamespace, targetClass, _) = GeneratorService.GetNewClassConfig(modelInterface, factoryPath, className);
            var (pureModelPath, pureClassName) = GeneratorService.GetPath(modelInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}", "{0}Model", "Model");
            var (pureCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(modelInterface, pureModelPath, className);
            
            targetClass.BaseTypes.Add(new CodeTypeReference($"ModelFactoryBase<{modelInterface.Name}, {saveClassName}>"));
            targetClass.BaseTypes.Add(new CodeTypeReference($"IModelSaveFactory<{modelInterface.Name}, {saveClassName}>"));
            codeNamespace.Imports.Add(new CodeNamespaceImport(saveCodeNamespace.Name));
            codeNamespace.Imports.Add(new CodeNamespaceImport(modelInterface.Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(IModelSaveFactory<,>).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(pureCodeNamespace.Name));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(List<>).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(ModelFactoryBase<,>).Namespace));

            var constructorRepositoriesParams = new HashSet<Type>();
            var constructorFactoriesParams = new HashSet<Type>();

            CodeMemberMethod createPureMethod = new CodeMemberMethod();
            createPureMethod.Name = "Create";
            createPureMethod.ReturnType = new CodeTypeReference(modelInterface);
            createPureMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var model = new {pureClassName}()")));
            foreach (var propertyInfo in classProperties)
            {
                var notNull = propertyInfo.GetCustomAttribute<NotNullAttribute>() != null;
                var defaultValue = propertyInfo.GetCustomAttribute<DefaultValueSnippetAttribute>()?.Value;
                
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    createPureMethod.Statements.Add(new CodeExpressionStatement(
                        new CodeSnippetExpression(
                            $"model.{propertyInfo.Name} = {defaultValue}")));
                }
                else
                {
                    if (propertyInfo.PropertyType.ToString().Contains("Generic.List"))
                    {
                        createPureMethod.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression(
                                $"model.{propertyInfo.Name} = new List<{propertyInfo.PropertyType.GetGenericArguments()[0]}>()")));
                    } 
                    else if (propertyInfo.PropertyType.ToString().Contains("Generic.Dictionary"))
                    {
                        createPureMethod.Statements.Add(new CodeExpressionStatement(
                            new CodeSnippetExpression(
                                $"model.{propertyInfo.Name} = new Dictionary<{propertyInfo.PropertyType.GetGenericArguments()[0]}, {propertyInfo.PropertyType.GetGenericArguments()[1]}>()")));
                    } 
                    else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (notNull)
                        {
                            createPureMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Factory.Create()")));
                            constructorFactoriesParams.Add(propertyInfo.PropertyType);
                        }
                    }
                }
            }

            createPureMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("model") ) );
            createPureMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            targetClass.Members.Add(createPureMethod);
            
            CodeMemberMethod createSaveMethod = new CodeMemberMethod();
            createSaveMethod.Name = "CreateSaveInternal";
            createSaveMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(new CodeTypeReference(modelInterface), "model"));
            createSaveMethod.ReturnType = new CodeTypeReference(saveClassName);
            createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"if (model == null) return default")));
            createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var saveModel = new {saveClassName}()")));
            foreach (var propertyInfo in classProperties)
            {
                var ignoreSave = false;
                foreach (var customAttribute in propertyInfo.GetCustomAttributes())
                {
                    if (customAttribute is IgnoreSaveAttribute)
                    {
                        ignoreSave = true;
                    }
                }

                if (ignoreSave)
                {
                    continue;
                }
                
                if (typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = model.{propertyInfo.Name}?.Id")));
                } 
                else if (propertyInfo.PropertyType.IsArray && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetElementType())
                         || propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = model.{propertyInfo.Name}.Select(p => p?.Id).ToArray()")));
                }
                else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Factory.CreateSaveInternal(model.{propertyInfo.Name})")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType);
                }
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = model.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetElementType().Name}Factory.CreateSaveInternal(sm)).ToArray()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetElementType());
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = model.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}Factory.CreateSaveInternal(sm)).ToArray()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetGenericArguments()[0]);
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = (int)model.{propertyInfo.Name}")));
                }
                else
                {
                    createSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"saveModel.{propertyInfo.Name} = model.{propertyInfo.Name}")));
                }
            }
            
            createSaveMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("saveModel") ) );
            createSaveMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            targetClass.Members.Add(createSaveMethod);
            
            CodeMemberMethod createFromSaveMethod = new CodeMemberMethod();
            createFromSaveMethod.Name = "Create";
            createFromSaveMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(new CodeTypeReference(saveClassName), "saveModel"));
            createFromSaveMethod.ReturnType = new CodeTypeReference(modelInterface);
            
            createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"if (saveModel == null) return default")));
            createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var model = Create()")));

            foreach (var propertyInfo in classProperties)
            {
                var ignoreSave = propertyInfo.GetCustomAttribute<IgnoreSaveAttribute>() != null;

                if (ignoreSave)
                {
                    continue;
                }
                
                if (typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Repository.Get(saveModel.{propertyInfo.Name})")));
                    constructorRepositoriesParams.Add(propertyInfo.PropertyType);
                }
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = saveModel.{propertyInfo.Name}?.Where(p => p != null).Select(entityId => this.{propertyInfo.PropertyType.GetElementType().Name}Repository.Get(entityId)).ToArray()")));
                    constructorRepositoriesParams.Add(propertyInfo.PropertyType.GetElementType());
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IEntity).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = saveModel.{propertyInfo.Name}?.Where(p => p != null).Select(entityId => this.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}Repository.Get(entityId)).ToList() ?? new List<{propertyInfo.PropertyType.GetGenericArguments()[0].Name}>()")));
                    constructorRepositoriesParams.Add(propertyInfo.PropertyType.GetGenericArguments()[0]);
                }
                else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = this.{propertyInfo.PropertyType.Name}Factory.Create(saveModel.{propertyInfo.Name})")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType);
                }
                else if (propertyInfo.PropertyType.IsArray &&
                         typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType()))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = saveModel.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetElementType().Name}Factory.Create(sm)).ToArray()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetElementType());
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = saveModel.{propertyInfo.Name}?.Select(sm => this.{propertyInfo.PropertyType.GetGenericArguments()[0].Name}Factory.Create(sm)).ToList() ?? new List<{propertyInfo.PropertyType.GetGenericArguments()[0].Name}>()")));
                    constructorFactoriesParams.Add(propertyInfo.PropertyType.GetGenericArguments()[0]);
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = ({propertyInfo.PropertyType})saveModel.{propertyInfo.Name}")));
                }
                else
                {
                    createFromSaveMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"model.{propertyInfo.Name} = saveModel.{propertyInfo.Name}")));
                }
            }

            createFromSaveMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("model") ) );
            createFromSaveMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
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
                var (saveModelTempPath, saveModelClassName) = GeneratorService.GetPath(modelType, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(modelType, saveModelTempPath, saveModelClassName, ignoreRootNamespace: true);
                codeNamespace.Imports.Add(new CodeNamespaceImport(saveModelCodeNamespace.Name));
                if (modelType == modelInterface)
                {
                    targetClass.Members.Add(new CodeMemberField(new CodeTypeReference($"IModelSaveFactory<{modelType.Name}, {saveModelClassName}>"), $"{modelType.Name}Factory"));
                    factoryConstructor.Statements.Add(new CodeExpressionStatement(
                        new CodeSnippetExpression(
                            $"this.{modelType.Name}Factory = this")));
                    continue;
                }
                
                codeNamespace.Imports.Add(new CodeNamespaceImport(modelType.Namespace));
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
        
        void GenerateModelSave(Type contextInterface)
        {
            var (saveContextPath, className) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
            
            var (codeNamespace, targetClass, properties) = GeneratorService.GetNewClassConfig(contextInterface, saveContextPath, className, ignoreRootNamespace: true);
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(List<>).Namespace));
            
            foreach (var propertyInfo in properties)
            {
                var codeMemberField = new CodeMemberField();
                var ignoreSave = false;
                foreach (var propertyInfoCustomAttribute in propertyInfo.GetCustomAttributes(true))
                {
                    if (propertyInfoCustomAttribute is IgnoreSaveAttribute)
                    {
                        ignoreSave = true;
                    }
                }

                if (ignoreSave)
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
                    // codeNamespace.Imports.Add(new CodeNamespaceImport(propertyInfo.PropertyType.Namespace));
                } 
                else if (typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var (saveModelPath, saveModelClassName) = GeneratorService.GetPath(propertyInfo.PropertyType, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                    var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propertyInfo.PropertyType, saveModelPath, saveModelClassName, ignoreRootNamespace: true);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(saveModelCodeNamespace.Name));
                    codeMemberField.Type = new CodeTypeReference(saveModelClassName);
                    codeMemberField.Name = $"{propertyInfo.Name}";
                }
                else if (propertyInfo.PropertyType.IsArray && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetElementType())
                         || propertyInfo.PropertyType.ToString().Contains("Generic.List") && typeof(IModel).IsAssignableFrom(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    Type propElementType;
                    if (propertyInfo.PropertyType.IsArray)
                    {
                        propElementType = propertyInfo.PropertyType.GetElementType();
                    }
                    else
                    {
                        propElementType = propertyInfo.PropertyType.GetGenericArguments()[0];
                    }
                    var (saveModelPath, saveModelClassName) = GeneratorService.GetPath(propElementType, $".Interfaces{Path.DirectorySeparatorChar}", $".Models{Path.DirectorySeparatorChar}", "{0}ModelSave", "Model");
                    var (saveModelCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propElementType, saveModelPath, saveModelClassName, ignoreRootNamespace: true);
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
        
        void GeneratePureModel(Type contextInterface)
        {
            var (pureModelPath, className) = GeneratorService.GetPath(contextInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}", "{0}Model", "Model");
            var (codeNamespace, targetClass, properties) = GeneratorService.GetNewClassConfig(contextInterface, pureModelPath, className);
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
            
            
            var targetUnit = new CodeCompileUnit();
            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);
            CodeGenExtensions.WriteSourceFile($"{pureModelPath}.cs", new CSharpCodeProvider(), targetUnit);
        }
    }
}