using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ButtonAction("search", "folderexplorer-folders-toolbar/Search")]
    [ButtonAction("search", "folders-toolbar/Search")]
    [Tooltip("search", "Search")]
	[IconSet("search", IconScheme.Colour, "Icons.SearchToolSmall.png", "Icons.SearchToolMedium.png", "Icons.SearchToolLarge.png")]
    [ClickHandler("search", "Search")]

    [ExtensionOf(typeof(ReportingWorkflowFolderToolExtensionPoint))]
    public class SearchTool : Tool<IReportingWorkflowFolderToolContext>
    {
        public void Search()
        {
            SearchComponent.Launch(this.Context.DesktopWindow);
        }
    }
}
