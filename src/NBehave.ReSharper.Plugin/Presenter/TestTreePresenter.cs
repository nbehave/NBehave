using JetBrains.CommonControls;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace NBehave.ReSharper.Plugin.Presenter
{
    public class TestTreePresenter : TreeModelBrowserPresenter
    {
        public TestTreePresenter()
        {
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveScenarioTestElement>(PresentScenario));
        }

        protected override bool IsNaturalParent(object parentValue, object childValue)
        {
            var unitTestNamespace = parentValue as IUnitTestNamespace;
            var scenarioTestElement = childValue as NBehaveScenarioTestElement;
            if (scenarioTestElement != null && unitTestNamespace != null)
            {
                return unitTestNamespace.NamespaceName.Equals(scenarioTestElement.GetNamespace().NamespaceName);
            }
            return base.IsNaturalParent(parentValue, childValue);
        }

        protected override object Unwrap(object value)
        {
            var testElement = value as NBehaveScenarioTestElement;
            if (testElement != null)
            {
                value = testElement.GetDeclaredElement();
            }
            return base.Unwrap(value);
        }

        private void PresentScenario(NBehaveScenarioTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        {
            if (IsNodeParentNatural(modelNode, value))
            {
                //item.RichText = value.TypeName.ShortName;
                item.RichText = value.TypeName.ToString();
                return;
            }
            if (string.IsNullOrEmpty(value.TypeName.GetNamespaceName()))
            {
                item.RichText = value.TypeName.ShortName;
                return;
            }
            item.RichText = string.Format("{0}.{1}", value.TypeName.GetNamespaceName(), value.TypeName.ShortName);
        }
    }
}