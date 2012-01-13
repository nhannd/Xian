using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Automation
{
    [ExtensionPoint]
    public sealed class AutomationExtensionPoint<T> : ExtensionPoint<T>
    {
    }

    public interface IAutomationContext
    {
        IImageBox SelectedImageBox { get; }
        ITile SelectedTile { get; }
    }

    public class AutomationContext : IAutomationContext
    {
        public static IAutomationContext Current { get; internal set; }

        private readonly ImageViewerComponent _viewer;

        internal AutomationContext(ImageViewerComponent viewer)
        {
            _viewer = viewer;
        }

        #region IContext Members

        public IWorkspaceLayout WorkspaceLayoutService { get { return _viewer.GetAutomationService<IWorkspaceLayout>(); } }
        public IDisplaySetLayout DisplaySetLayoutService { get { return _viewer.GetAutomationService<IDisplaySetLayout>(); } }
        public IStack StackService { get { return _viewer.GetAutomationService<IStack>(); } }

        public IImageBox SelectedImageBox { get { return _viewer.SelectedImageBox; } }
        public ITile SelectedTile { get { return _viewer.SelectedTile; } }

        #endregion
    }
}
