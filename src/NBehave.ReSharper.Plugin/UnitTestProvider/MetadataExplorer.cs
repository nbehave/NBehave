using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly IClrTypeName ActionStepsAttribute = new ClrTypeName(typeof(ActionStepsAttribute).FullName);
        private static readonly IClrTypeName ActionStepAttribute = new ClrTypeName(typeof(ActionStepAttribute).FullName);
        private readonly TestProvider _testProvider;
        private readonly UnitTestElementConsumer _consumer;
        private readonly IProject _project;
        private readonly ProjectModelElementEnvoy _projectEnvoy;
        private GherkinFileParser _gherkinParser;

        public MetadataExplorer(TestProvider provider, IProject project, UnitTestElementConsumer consumer)
        {
            _testProvider = provider;
            _consumer = consumer;
            _project = project;
            _projectEnvoy = new ProjectModelElementEnvoy(_project);
        }

        public void ExploreProject()
        {
            var featureFiles = GetFeatureFilesFromProject().ToList();
            _gherkinParser = new GherkinFileParser(_project.GetOutputAssemblyFile(), featureFiles, _testProvider, _projectEnvoy);
            var elements = _gherkinParser.ParseFilesToElements(featureFiles).ToList();
            BindFeatures(elements);
        }

        private IEnumerable<IProjectFile> GetFeatureFilesFromProject()
        {
            var validExtensions = new NBehaveConfiguration().FeatureFileExtensions;
            var featureFiles = _project
                .GetAllProjectFiles()
                .Where(_ => validExtensions.Any(e => e.Equals(Path.GetExtension(_.Name), StringComparison.CurrentCultureIgnoreCase)))
                .ToList();
            return featureFiles;
        }

        private void BindFeatures(IEnumerable<NBehaveUnitTestElementBase> features)
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
                   && HasAnyAttributeOrDerivedAttribute(method, attributeChecker, new[] { ActionStepsAttribute, ActionStepAttribute });
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