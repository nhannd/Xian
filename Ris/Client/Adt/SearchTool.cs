using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [ButtonAction("search", "folderexplorer-folders-toolbar/Search")]
    [ButtonAction("search", "folders-toolbar/Search")]
    [Tooltip("search", "Search")]
	[IconSet("search", IconScheme.Colour, "Icons.SearchToolSmall.png", "Icons.SearchToolMedium.png", "Icons.SearchToolLarge.png")]
    [ClickHandler("search", "Search")]

    [ExtensionOf(typeof(RegistrationWorkflowFolderToolExtensionPoint))]
    [ExtensionOf(typeof(TechnologistWorkflowFolderToolExtensionPoint))]
    public class SearchTool : Tool<IToolContext>
    {
        public void Search()
        {
            if (this.ContextBase is IRegistrationWorkflowFolderToolContext)
            {
                IRegistrationWorkflowFolderToolContext context = (IRegistrationWorkflowFolderToolContext)this.ContextBase;
                SearchComponent.Launch(context.DesktopWindow);
            }
            else if (this.ContextBase is ITechnologistWorkflowFolderToolContext)
            {
                ITechnologistWorkflowFolderToolContext context = (ITechnologistWorkflowFolderToolContext)this.ContextBase;
                SearchComponent.Launch(context.DesktopWindow);
            }
        }
    }
}
