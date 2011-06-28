using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.AttributeChecker;
using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class MetadataExplorer
    {
        private static readonly IClrTypeName ActionStepsAttribute = new ClrTypeName("NBehave.Narrator.Framework.ActionStepsAttribute");
        private static readonly IClrTypeName ActionStepAttribute = new ClrTypeName("NBehave.Narrator.Framework.ActionStepAttribute");
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
        }

        public void ExploreProject()
        {
            var featureFiles = GetFeatureFilesFromProject();

            var features = featureFiles
                .Select(feature =>
                            {
                                //TODO: feature.Name is NOT a ClrTypeName !
                                var clrType = new ClrTypeName(feature.Name);
                                return new NBehaveScenarioTestElement(feature, _testProvider, _projectEnvoy, clrType);
                            })
                .ToList();
            BindFeatures(features);
        }

        private IEnumerable<IProjectFile> GetFeatureFilesFromProject()
        {
            var validExtensions = new NBehaveConfiguration().FeatureFileExtensions;
            var featureFiles = _project
                .GetAllProjectFiles()
                .Where(_ => validExtensions.Any(e => _.Name.EndsWith(e, StringComparison.CurrentCultureIgnoreCase)));
            return featureFiles;
        }

        private void BindFeatures(IEnumerable<NBehaveScenarioTestElement> features)
        {
            foreach (var feature in features)
                _consumer(feature);
        }

        public static bool IsActionStepMethod(ITypeMember element, UnitTestAttributeCache attributeChecker)
        {
            var method = element as IMethod;
            return method != null
                   && !method.IsAbstract
                   && method.GetAccessRights() == AccessRights.PUBLIC
                   && !method.TypeParameters.Any()
                   && HasAnyAttributeOrDerivedAttribute(method, attributeChecker, new[] {ActionStepsAttribute, ActionStepAttribute});
        }

        public static bool IsActionStepsClass(ITypeElement typeElement, out bool isAbstract, UnitTestAttributeCache attributeChecker)
        {
            isAbstract = false;
            var @class = typeElement as IClass;
            if (@class == null)
                return false;
            var modifiersOwner = (IModifiersOwner) typeElement;
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