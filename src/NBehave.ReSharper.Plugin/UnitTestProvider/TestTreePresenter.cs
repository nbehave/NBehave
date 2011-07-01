using JetBrains.CommonControls;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class TestTreePresenter : TreeModelBrowserPresenter
    {
        public TestTreePresenter()
        {
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveFeatureTestElement>(PresentFeature));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveScenarioTestElement>(PresentScenario));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveStepTestElement>(PresentStep));
        }

        protected override bool IsNaturalParent(object parent, object child)
        {
            var unitTestNamespace = parent as IUnitTestNamespace;
            var featureElement = child as NBehaveFeatureTestElement;
            if (featureElement != null && unitTestNamespace != null)
                return unitTestNamespace.NamespaceName.Equals(featureElement.GetNamespace().NamespaceName);

            var p = parent as NBehaveUnitTestElementBase;
            var c = child as NBehaveUnitTestElementBase;

            if (p != null && c != null)
                return c.Id.StartsWith(p.Id);

            return base.IsNaturalParent(parent, child);
        }

        protected override object Unwrap(object value)
        {
            var testElement = value as NBehaveUnitTestElementBase;
            if (testElement != null)
                value = testElement.GetDeclaredElement();
            return base.Unwrap(value);
        }

        private void PresentFeature(NBehaveFeatureTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        { }

        private void PresentScenario(NBehaveScenarioTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        { }

        private void PresentStep(NBehaveStepTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        { }
    }
}