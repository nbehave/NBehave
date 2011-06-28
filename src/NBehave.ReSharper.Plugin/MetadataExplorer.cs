using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.AttributeChecker;

namespace NBehave.ReSharper.Plugin
{
    public class MetadataExplorer
    {
        private static readonly IClrTypeName ActionStepsAttribute = new ClrTypeName("NBehave.Narrator.Framework.ActionStepsAttribute");
        private static readonly IClrTypeName ActionStepAttribute = new ClrTypeName("NBehave.Narrator.Framework.ActionStepAttribute");
        private readonly UnitTestAttributeCache _attributeCache;
        private readonly TestProvider _testProvider;
        private readonly UnitTestElementConsumer _consumer;
        private readonly IProject _project;
        private readonly ProjectModelElementEnvoy _projectEnvoy;

        public MetadataExplorer(TestProvider provider, IProject project, UnitTestElementConsumer consumer)
        {
            _testProvider = provider;
            _consumer = consumer;
            _project = project;
            _projectEnvoy = new ProjectModelElementEnvoy(_project);
            _attributeCache = project.GetSolution().GetComponent<UnitTestAttributeCache>();
        }

        public void ExploreAssembly(IMetadataAssembly assembly)
        {
            var features = new List<NBehaveScenarioTestElement>();
            foreach (IMetadataTypeInfo current2 in assembly.GetTypes())
                features.AddRange(ExploreType(current2));
            features = features.Distinct(_ => _.Id).ToList();
            BindFeatures(features);
        }

        private void BindFeatures(IEnumerable<NBehaveScenarioTestElement> features)
        {
            foreach (var feature in features)
                _consumer(feature);
        }

        private IEnumerable<NBehaveScenarioTestElement> ExploreType(IMetadataTypeInfo type)
        {
            var files = new List<NBehaveScenarioTestElement>();
            if (IsActionStepsClass(type))
            {
                //TODO: see StoryLocator for file extensions to find
                var featureFiles = _project.GetAllProjectFiles().Where(_ => _.Name.EndsWith(".feature", StringComparison.CurrentCultureIgnoreCase)).ToList();
                foreach (IProjectFile feature in featureFiles)
                {
                    //TODO: feature.Name is NOT a ClrTypeName !
                    var clrType = new ClrTypeName(feature.Name);
                    var f = new NBehaveScenarioTestElement(feature, _testProvider, _projectEnvoy, clrType);
                    files.Add(f);
                }
            }
            return files;
        }

        private bool IsActionStepsClass(IMetadataTypeInfo typeInfo)
        {
            if (typeInfo.IsAbstract)
            {
                if (typeInfo.GetMethods().Any(method => !method.IsAbstract && !method.IsStatic))
                {
                    return false;
                }
            }
            return HasActionStepsAttribute(typeInfo, ActionStepsAttribute)
                || typeInfo.GetMethods().Any(IsActionStepMethod);

        }

        private bool HasActionStepsAttribute(IMetadataTypeInfo typeInfo, IClrTypeName attribute)
        {
            if (HasAttributeOrDerivedAttribute(typeInfo, attribute))
            {
                return true;
            }
            IMetadataClassType @base = typeInfo.Base;
            return @base != null && HasActionStepsAttribute(@base.Type, attribute);
        }

        private bool IsActionStepMethod(IMetadataMethod method)
        {
            return !method.IsAbstract
                && method.IsPublic
                && method.GenericArguments.Length == 0
                && (HasAttributeOrDerivedAttribute(method, ActionStepAttribute));
        }

        public static bool IsActionStepMethod(ITypeMember element, UnitTestAttributeCache attributeChecker)
        {
            var method = element as IMethod;
            return method != null
                && !method.IsAbstract
                && method.GetAccessRights() == AccessRights.PUBLIC
                && !method.TypeParameters.Any()
                && HasAnyAttributeOrDerivedAttribute(method, attributeChecker, new[] { ActionStepsAttribute, ActionStepAttribute});
        }

        public static bool IsActionStepsClass(ITypeElement typeElement, out bool isAbstract, UnitTestAttributeCache attributeChecker)
        {
            isAbstract = false;
            var @class = typeElement as IClass;
            if (@class == null)
                return false;
            var modifiersOwner = (IModifiersOwner)typeElement;
            isAbstract = modifiersOwner.IsAbstract;
            AccessRights accessRights = modifiersOwner.GetAccessRights();
            if (accessRights != AccessRights.PUBLIC)
                return false;
            if (HasAttributeOrDerivedAttribute(typeElement, ActionStepsAttribute, attributeChecker))
                return true;
            return (
                       @class
                       .GetAllSuperClasses()
                       .Select(superType => superType.GetTypeElement()))
                       .Any(superClass => superClass != null && HasAttributeOrDerivedAttribute(superClass, ActionStepsAttribute, attributeChecker))
                       || typeElement.GetMembers().Any(member => IsActionStepMethod(member, attributeChecker));
        }

        private bool HasAttributeOrDerivedAttribute(IMetadataEntity entity, IClrTypeName attributeClrName)
        {
            return attributeClrName != null && _attributeCache.HasAttributeOrDerivedAttribute(entity, attributeClrName);
        }

        private static bool HasAttributeOrDerivedAttribute(IAttributesOwner member, IClrTypeName attribute, UnitTestAttributeCache attributeChecker)
        {
            return attributeChecker.HasAttributeOrDerivedAttribute(member, attribute);
        }

        private static bool HasAnyAttributeOrDerivedAttribute(IAttributesOwner member, UnitTestAttributeCache attributeChecker, params IClrTypeName[] attributes)
        {
            return attributeChecker.HasAttributeOrDerivedAttribute(member, attributes);
        }
    }
}