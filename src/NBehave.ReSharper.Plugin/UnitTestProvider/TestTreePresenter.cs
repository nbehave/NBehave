using System;
using System.Drawing;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.RichText;
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
        {
            //if (IsNodeParentNatural(modelNode, value))
            //{
            //    return;
            //}
            //if (string.IsNullOrEmpty(value.TypeName.GetNamespaceName()))
            //{
            //    item.RichText = value.TypeName.ShortName;
            //    return;
            //}
            //item.RichText = string.Format("{0}.{1}", value.TypeName.GetNamespaceName(), value.TypeName.ShortName);
        }

        private void PresentScenario(NBehaveScenarioTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        {
            //if (IsNodeParentNatural(modelNode, value))
            //{
            //    //var style = new TextStyle(FontStyle.Regular, Color.Black, Color.BurlyWood);
            //    //var part1 = new RichText(value.State.ToString(), style);
            //    //item.RichText += part1;
            //    //var style2 = new TextStyle(FontStyle.Regular, Color.Black, Color.Aqua);
            //    //item.RichText += new RichText("En rad med text", style2);
            //    //item.RichText += new RichText("nästa rad med text", style2);
            //    return;
            //}
            //if (string.IsNullOrEmpty(value.TypeName.GetNamespaceName()))
            //{
            //    item.RichText = value.TypeName.ShortName;
            //    return;
            //}
            //item.RichText = string.Format("{0}.{1}", value.TypeName.GetNamespaceName(), value.TypeName.ShortName);
        }

        private void PresentStep(NBehaveStepTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        {
            //if (IsNodeParentNatural(modelNode, value))
            //{
            //    return;
            //}
            //if (string.IsNullOrEmpty(value.TypeName.GetNamespaceName()))
            //{
            //    item.RichText = value.TypeName.ShortName;
            //    return;
            //}
            //item.RichText = string.Format("{0}.{1}", value.TypeName.GetNamespaceName(), value.TypeName.ShortName);
        }


        private static RichText FormatText(NBehaveScenarioTestElement value)
        {
            //return value.TypeName.ToString();
            var content = value.GetPresentation().Split(new[] { '\n', '\r' });
            var r = new RichText();
            foreach (var s in content)
            {
                var t = TextStyle.Default;
                var str = s.Trim(new[] { ' ', '\t' });
                if (str.StartsWith("Feature", StringComparison.CurrentCultureIgnoreCase))
                    t = new TextStyle(FontStyle.Bold, Color.Blue);
                if (str.StartsWith("Scenario", StringComparison.CurrentCultureIgnoreCase))
                    t = new TextStyle(FontStyle.Bold);
                if (str.StartsWith("Given", StringComparison.CurrentCultureIgnoreCase)
                    || str.StartsWith("When", StringComparison.CurrentCultureIgnoreCase)
                    || str.StartsWith("Then", StringComparison.CurrentCultureIgnoreCase))
                    t = new TextStyle(FontStyle.Italic);
                r.Append(s + Environment.NewLine, t);
                //r.Append(Environment.NewLine);
            }
            return r;
        }
    }
}