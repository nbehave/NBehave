using System.Collections.Generic;
using System.Linq;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Features.Shared.TreePsiBrowser;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class TestPresenter : IUnitTestPresenter
    {
        private readonly TestTreePresenter presenter = new TestTreePresenter();

        public void Present(IUnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            if (element is NBehaveUnitTestElementBase)
                presenter.UpdateItem(element, node, item, state);
        }
    }

    public class TestTreePresenter : TreeModelBrowserPresenter
    {
        private readonly Dictionary<NBehaveFeatureTestElement, TreeModelNode> _treeModels = new Dictionary<NBehaveFeatureTestElement, TreeModelNode>();

        public TestTreePresenter()
        {
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveFeatureTestElement>(PresentFeature));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveScenarioTestElement>(PresentScenario));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveBackgroundTestElement>(PresentBackgroundScenario));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveStepTestElement>(PresentStep));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveExampleTestElement>((a, b, c, d) => { }));
            Present(new PresentationCallback<TreeModelNode, IPresentableItem, NBehaveExampleParentTestElement>((a, b, c, d) => { }));
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
            TreeModelNode parentModel;
            if (_treeModels.TryGetValue(value, out parentModel) == false)
            {
                parentModel = modelNode.Parent;
                _treeModels.Add(value, parentModel);
            }
        }

        private void PresentScenario(NBehaveScenarioTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        { }

        private void PresentBackgroundScenario(NBehaveBackgroundTestElement value, IPresentableItem item, TreeModelNode structureelement, PresentationState state)
        { }

        private void PresentStep(NBehaveStepTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        { }

        private TaskResult GetTaskResult(IEnumerable<ScenarioResult> results)
        {
            var taskResult = (results.Any()) ? TaskResult.Skipped : TaskResult.Exception;
            taskResult = (results.Any(_ => _.Result is Passed)) ? TaskResult.Success : taskResult;
            taskResult = (results.Any(_ => _.Result is Pending)) ? TaskResult.Inconclusive : taskResult;
            taskResult = (results.Any(_ => _.Result is Failed)) ? TaskResult.Error : taskResult;
            return taskResult;
        }
    }
}