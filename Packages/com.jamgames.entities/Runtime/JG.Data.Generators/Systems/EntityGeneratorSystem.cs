using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.JG.Extensions.Editor.Extensions;
using JetBrains.Annotations;
using JG.Data.Entities.Editor.Common;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Generators.Extensions;
using JG.Data.Generators.Services;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JG.Data.Generators.Systems
{
    public class EntityGeneratorSystem
    {
        Type[] _types;

        struct CsvParseGenerateArgs
        {
            public PropertyInfo property;
            public CodeMemberField field;
            public CsvParseColumnAttribute attribute;
        }

        public EntityGeneratorSystem()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName[2] == '.' || assembly.FullName[3] == '.' || assembly.FullName.Contains("Assembly-CSharp"))
                .SelectMany(a => a.GetTypes()).ToArray();
        }

        public void GenerateEntities()
        {
            Debug.Log("Start generating");
            var entityType = typeof(IEntity);
            var editorEntityType = typeof(IEditorEntity<>);
            var entityInterfaces = _types.Where(type => type != entityType)
                .Where(type => entityType.IsAssignableFrom(type) && !editorEntityType.IsAssignableFrom(type))
                .Where(type => type.IsInterface)
                .Where(type => !type.IsAbstractEntity());
            
            foreach (var entityInterface in entityInterfaces)
            {
                GeneratePureEntity(entityInterface);
                GenerateEditorEntity(entityInterface);
            }
            
            AssetDatabase.Refresh();
        }
        
        void GenerateEditorEntity(Type entityInterface)
        {
            var types = _types;
            foreach (var typeEditorEntity in types)
            {
                foreach (var customAttribute in typeEditorEntity.GetCustomAttributes())
                {
                    if (customAttribute is CustomEditorEntityAttribute customEditorEntityAttribute 
                        && customEditorEntityAttribute.EditorEntityType == entityInterface)
                    {
                        return;
                    }
                }
            }

            var (pureEditorEntityPath, className) = GeneratorService.GetPath(entityInterface, $".Interfaces{Path.DirectorySeparatorChar}Entities", $".Editor{Path.DirectorySeparatorChar}EditorEntities", "{0}EditorEntity", "Entity");
            var (codeNamespace, targetClass, properties) = GeneratorService.GetNewClassConfig(entityInterface, pureEditorEntityPath, className);
            targetClass.BaseTypes.Add($"EditorEntityBase<{entityInterface.ToString().Split('.').Last()}>");
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(EditorEntityBase<>).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(SerializeField).Namespace));
            codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(ObjectExtensions).Namespace));
            targetClass.BaseTypes.Add(entityInterface);
            
            
            CodeMemberMethod toEntityMethod = new CodeMemberMethod();
            toEntityMethod.Name = "ToEntity";
            toEntityMethod.ReturnType = new CodeTypeReference(entityInterface);
            var (pureClassPath, pureClassName) = GeneratorService.GetPath(entityInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}", "{0}Entity", "Entity");
            var (pureCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(entityInterface, pureClassPath, pureClassName);
            var subfolder = string.Empty;
            if (typeof(ISettingsEntity).IsAssignableFrom(entityInterface))
            {
                subfolder = $"/Settings";
            }
            else
            {
                foreach (var customAttribute in entityInterface.GetCustomAttributes())
                {
                    if (customAttribute is CustomSubfolderAttribute customSubfolderAttribute)
                    {
                        subfolder = $"/{customSubfolderAttribute.Subfolder}";
                    }
                }
            }

            targetClass.CustomAttributes.Add(new CodeAttributeDeclaration("CreateAssetMenu",
                new CodeAttributeArgument(new CodeSnippetExpression($"fileName = \"{pureClassName.Replace("Entity", "").ToLower()}.\"")),
                new CodeAttributeArgument(new CodeSnippetExpression($"menuName = \"{entityInterface.Namespace.Split(".")[0]}{subfolder}/{pureClassName.Replace("Entity", "")}\"")),
                new CodeAttributeArgument(new CodeSnippetExpression("order = 1"))));

            codeNamespace.Imports.Add(new CodeNamespaceImport(pureCodeNamespace.Name));
            toEntityMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"var entity = new {pureClassName}()")));

            var csvParseArgsList = new List<CsvParseGenerateArgs>();

            foreach (var propertyInfo in properties)
            {
                var property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Name = propertyInfo.Name;
                var fieldName = $"_{propertyInfo.Name[0].ToString().ToLower()}{propertyInfo.Name.Substring(1)}";

                property.Type = new CodeTypeReference(propertyInfo.PropertyType);
                codeNamespace.Imports.Add(new CodeNamespaceImport(propertyInfo.PropertyType.Namespace));
                targetClass.Members.Add(property);
                var isSprite = propertyInfo.GetCustomAttribute<SpriteFieldAttribute>() != null;
                var isAudio = propertyInfo.GetCustomAttribute<AudioClipAttribute>() != null;
                var prefabFieldType = propertyInfo.GetCustomAttribute<PrefabFieldAttribute>()?.Type;
                var isMaterial = propertyInfo.GetCustomAttribute<MaterialFieldAttribute>() != null;
                Type entityLink = propertyInfo.GetCustomAttribute<EntityLinkAttribute>()?.EntityType;
                var field = new CodeMemberField();
                var isObjectName = propertyInfo.GetCustomAttribute<ObjectNameAttribute>() != null;
                CustomEditorEntityAttribute customEditorEntity = null;

                Type editorEntityType = null;
                var isVisualAsset = propertyInfo.GetCustomAttribute<VisualAssetField>() != null;
                var defaultValueSnippetAttribute = propertyInfo.GetCustomAttribute<DefaultValueSnippetAttribute>();
                if (defaultValueSnippetAttribute != null)
                {
                    field.InitExpression = new CodeSnippetExpression(defaultValueSnippetAttribute.Value);
                }

                var customEditorEntityAttributeEntity = propertyInfo.GetCustomAttribute<CustomEditorEntityAttribute>();
                if (customEditorEntityAttributeEntity != null)
                {
                    customEditorEntity = customEditorEntityAttributeEntity;
                    editorEntityType = customEditorEntityAttributeEntity.EditorEntityType;
                }

                var csvParseColumnAttribute = propertyInfo.GetCustomAttribute<CsvParseColumnAttribute>();
                if (csvParseColumnAttribute != null)
                {
                    var csvParseGenerateArgs = new CsvParseGenerateArgs();
                    csvParseGenerateArgs.attribute = csvParseColumnAttribute;
                    csvParseGenerateArgs.property = propertyInfo;
                    csvParseGenerateArgs.field = field;
                    csvParseArgsList.Add(csvParseGenerateArgs);

                    property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),new CodePropertySetValueReferenceExpression()));
                }
                
                if (customEditorEntity == null)
                {
                    foreach (var typeEditorEntity in types)
                    {
                        var customEditorEntityAttribute =
                            typeEditorEntity.GetCustomAttribute<CustomEditorEntityAttribute>();
                        if (customEditorEntityAttribute == null)
                        {
                            continue;
                        }
                        
                        if (customEditorEntityAttribute.EditorEntityType == propertyInfo.PropertyType &&
                            !propertyInfo.PropertyType.IsArray)
                        {
                            customEditorEntity = customEditorEntityAttribute;
                            editorEntityType = typeEditorEntity;
                        }
                        else if (propertyInfo.PropertyType.IsArray &&
                                 customEditorEntityAttribute.EditorEntityType ==
                                 propertyInfo.PropertyType.GetElementType())
                        {
                            customEditorEntity = customEditorEntityAttribute;
                            editorEntityType = typeEditorEntity;
                        }
                    }
                }

                field.Name = fieldName;

                if (customEditorEntity != null)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(editorEntityType.Namespace));
                    if (!propertyInfo.PropertyType.IsArray)
                    {
                        field.Type = new CodeTypeReference(editorEntityType);
                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}{customEditorEntity.ConvertSnippet}")));
                    }
                    else
                    {
                        field.Type = new CodeTypeReference($"{editorEntityType.Name}[]");
                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}?.Select(p => p{customEditorEntity.ConvertSnippet}).ToArray()")));
                    }
                }
                else if (entityLink != null)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(entityLink.Namespace));
                    field.Type = new CodeTypeReference(entityLink);
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}?.Id")));
                }
                else if (prefabFieldType != null && !propertyInfo.PropertyType.IsArray)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(prefabFieldType.Namespace));
                    field.Type = new CodeTypeReference(prefabFieldType);
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}?.GetAddress()")));
                }
                else if (prefabFieldType != null && propertyInfo.PropertyType.IsArray)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(prefabFieldType.Namespace));
                    field.Type = new CodeTypeReference(prefabFieldType.MakeArrayType(1));
                    property.GetStatements.Add(new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                            $"{fieldName}?.Select(p => p?.GetAddress()).ToArray()")));
                }
                else if (isMaterial)
                {
                    ProcessProperty<Material>(propertyInfo, codeNamespace, field, property, fieldName, "?.GetAddress()");
                }
                else if (isVisualAsset)
                {
                    ProcessProperty<VisualTreeAsset>(propertyInfo, codeNamespace, field, property, fieldName, "?.GetAddress()");
                }
                else if (isSprite)
                {
                    ProcessProperty<Sprite>(propertyInfo, codeNamespace, field, property, fieldName, "?.GetAddress()");
                }
                else if (isObjectName && !propertyInfo.PropertyType.IsArray)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(Sprite).Namespace));
                    field.Type = new CodeTypeReference(typeof(Sprite));
                    if (propertyInfo.GetCustomAttribute<NotNullAttribute>() != null || propertyInfo.GetCustomAttribute<System.Diagnostics.CodeAnalysis.NotNullAttribute>() != null)
                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}.name")));
                    else
                        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName} != null ? {fieldName}?.name : null")));
                }
                else if (isObjectName && propertyInfo.PropertyType.IsArray)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(Sprite).Namespace));
                    field.Type = new CodeTypeReference(typeof(Sprite[]));
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}?.Select(p => p?.name).ToArray()")));
                }
                else if (isAudio)
                {
                    ProcessProperty<AudioClip>(propertyInfo, codeNamespace, field, property, fieldName, "?.GetAddress()");
                }
                else if (propertyInfo.PropertyType.IsAbstractEntity())
                {
                    AddEntityTypeFilter(codeNamespace, field, propertyInfo.PropertyType);
                    field.Type = new CodeTypeReference(typeof(SimpleEditorEntityBase));
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(
                        $"{fieldName} != null ? ({propertyInfo.PropertyType.Name}){fieldName}.ToEntityObject() : null")));
                }
                else if (propertyInfo.PropertyType.IsArray && propertyInfo.PropertyType.GetElementType().IsAbstractEntity())
                {
                    var elementType = propertyInfo.PropertyType.GetElementType();
                    AddEntityTypeFilter(codeNamespace, field, elementType);
                    field.Type = new CodeTypeReference(typeof(SimpleEditorEntityBase[]));
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(
                        $"{fieldName}?.Where(f => f != null).Select(f => ({elementType.Name})f.ToEntityObject()).ToArray() ?? Array.Empty<{elementType.Name}>()")));
                }
                else if (propertyInfo.PropertyType.IsArray && propertyInfo.PropertyType.GetElementType().ToString().Split('.').Last().Contains("Entity"))
                {
                    var (customEditorEntityPath, customEditorClassName) = GeneratorService.GetPath(propertyInfo.PropertyType.GetElementType(), $".Interfaces{Path.DirectorySeparatorChar}Entities", $".Editor{Path.DirectorySeparatorChar}EditorEntities", "{0}EditorEntity", "Entity");
                    var (customEditorCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propertyInfo.PropertyType.GetElementType(), customEditorEntityPath, customEditorClassName);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(customEditorCodeNamespace.Name));
                    field.Type = new CodeTypeReference($"{customEditorClassName}[]");
                    // field.CustomAttributes.Add(new CodeAttributeDeclaration("ReorderableList"));
                    property.GetStatements.Add(new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), $"{fieldName}?.Where(f => f != null).Select(f => f.ToEntity()).ToArray() ?? Array.Empty<{customEditorClassName}>()")));
                }
                else if (propertyInfo.PropertyType.ToString().Split('.').Last().Contains("Entity"))
                {
                    var (customEditorEntityPath, customEditorClassName) = GeneratorService.GetPath(propertyInfo.PropertyType, $".Interfaces{Path.DirectorySeparatorChar}Entities", $".Editor{Path.DirectorySeparatorChar}EditorEntities", "{0}EditorEntity", "Entity");
                    var (customEditorCodeNamespace, _, _) = GeneratorService.GetNewClassConfig(propertyInfo.PropertyType, customEditorEntityPath, customEditorClassName);
                    codeNamespace.Imports.Add(new CodeNamespaceImport(customEditorCodeNamespace.Name));
                    field.Type = new CodeTypeReference($"{customEditorClassName}");
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}?.ToEntity()")));
                }
                else if (propertyInfo.PropertyType.IsArray)
                {
                    field.Type = new CodeTypeReference(propertyInfo.PropertyType);
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName} ?? Array.Empty<{propertyInfo.PropertyType.GetElementType()}>()")));
                }
                else if (propertyInfo.PropertyType.ToString().Contains("Generic.Dictionary"))
                {
                    field.Type = new CodeTypeReference(propertyInfo.PropertyType);
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                    field.InitExpression = new CodeSnippetExpression("new ()");
                }
                else
                {
                    field.Type = new CodeTypeReference(propertyInfo.PropertyType);
                    property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                }

                var ignoreField = false;
                //foreach (var propertyInfoCustomAttribute in propertyInfo.GetCustomAttributes(true))
                foreach (var customAttributeData in propertyInfo.GetCustomAttributesData())
                {
                    // foreach (var customAttributeData in propertyInfo.GetCustomAttributesData())
                    // {
                    //     customAttributeData.
                    // }
                    if (customAttributeData.AttributeType == typeof(GenerateCustomAttributeAttribute))
                    {
                        // codeNamespace.Imports.Add(new CodeNamespaceImport(generateCustomAttribute.Attribute.Namespace));
                        // var codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(generateCustomAttribute.Attribute));
                        // foreach (var parameter in generateCustomAttribute.Parameters)
                        // {
                        //     codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(parameter.ToString()))
                        // }

                            
                            // field.CustomAttributes.Add(codeAttributeDeclaration);
                        // foreach (var attribute in generateCustomAttribute.Attributes)
                        // {
                        //     codeNamespace.Imports.Add(new CodeNamespaceImport(attribute.Namespace));
                        //     var codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(attribute));
                        //     
                        //     field.CustomAttributes.Add(codeAttributeDeclaration);
                        // }
                    }

                    if (customAttributeData.AttributeType == typeof(IgnoreEntityFieldAttribute))
                    {
                        ignoreField = true;
                    }
                }

                field.CustomAttributes.Add(new CodeAttributeDeclaration("SerializeField"));
                if (!ignoreField)
                {
                    targetClass.Members.Add(field);
                }

                toEntityMethod.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"entity.{propertyInfo.Name} = this.{propertyInfo.Name}")));
            }

            toEntityMethod.Statements.Add( new CodeMethodReturnStatement( new CodeArgumentReferenceExpression("entity") ) );
            toEntityMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            targetClass.Members.Add(toEntityMethod);

            if (csvParseArgsList.Count > 0)
            {
                var onValidateMethod = new CodeMemberMethod();
                onValidateMethod.Name = "OnValidate";
                onValidateMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                onValidateMethod.Statements.Add(new CodeSnippetStatement("#if UNITY_EDITOR"));
                foreach (CsvParseGenerateArgs args in csvParseArgsList)
                {
                    string methodName;

                    if (args.property.PropertyType == typeof(List<int>))
                    {
                        methodName = "LoadCsvAndGetColumnInt";
                    }
                    else if (args.property.PropertyType == typeof(List<string>))
                    {
                        methodName = "LoadCsvAndGetColumn";
                    }
                    else if (args.property.PropertyType.IsArray && args.property.PropertyType.GetElementType() == typeof(int))
                    {
                        methodName = "LoadCsvAndGetColumnIntArray";
                    }
                    else if (args.property.PropertyType.IsArray && args.property.PropertyType.GetElementType() == typeof(string))
                    {
                        methodName = "LoadCsvAndGetColumnArray";
                    }
                    else
                    {
                        Debug.LogError($"property type {args.property.PropertyType.Name} is not supported for parsing");
                        continue;
                    }

                    onValidateMethod.Statements.Add(new CodeConditionStatement(
                        new CodeSnippetExpression(
                            $"{args.field.Name} == null || {args.field.Name}.Count() == 0"),
                        new CodeSnippetStatement(
                            $"{args.field.Name} = ParseCsvExtensions.{methodName}(\"{args.attribute.CsvFileName}\", \"{args.attribute.ColumnTitle}\");")
                    ));
                }

                onValidateMethod.Statements.Add(new CodeSnippetStatement("#endif"));

                targetClass.Members.Add(onValidateMethod);
            }

            var targetUnit = new CodeCompileUnit();
            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);
            CodeGenExtensions.WriteSourceFile($"{pureEditorEntityPath}.cs", new CSharpCodeProvider(), targetUnit);
        }

        void GeneratePureEntity(Type entityInterface)
        {
            var (pureEntityPath, className) = GeneratorService.GetPath(entityInterface, $".Interfaces{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}", "{0}Entity", "Entity");
            
            var (codeNamespace, targetClass, properties) = GeneratorService.GetNewClassConfig(entityInterface, pureEntityPath, className);
            targetClass.BaseTypes.Add(entityInterface);
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
            CodeGenExtensions.WriteSourceFile($"{pureEntityPath}.cs", new CSharpCodeProvider(), targetUnit);
        }

        static void AddEntityTypeFilter(CodeNamespace codeNamespace, CodeMemberField field, Type entityType)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport(entityType.Namespace));
            field.CustomAttributes.Add(new CodeAttributeDeclaration(nameof(EntityTypeFilterAttribute).Replace("Attribute", ""),
                new CodeAttributeArgument(new CodeTypeOfExpression(entityType.Name))));
        }

        void ProcessProperty<T>(PropertyInfo propertyInfo, CodeNamespace codeNamespace, CodeMemberField field, CodeMemberProperty property, string fieldName, string fieldConvert)
        {
            if (!propertyInfo.PropertyType.IsArray)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(Sprite).Namespace));
                field.Type = new CodeTypeReference(typeof(T));
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"{fieldName}{fieldConvert}")));
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(typeof(Sprite).Namespace));
                field.Type = new CodeTypeReference(typeof(T[]));
                property.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                        $"{fieldName}?.Select(p => p{fieldConvert}).ToArray()")));
            }
        }
    }
}
