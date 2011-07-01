using JetBrains.CommonControls;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    [UnitTestPresenter]
    public class Presenter : IUnitTestPresenter
    {
        private readonly TestTreePresenter _treePresenter = new TestTreePresenter();

        public void Present(IUnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            if (element is NBehaveUnitTestElementBase)
                _treePresenter.UpdateItem(element, node, item, state);
        }
    }
}