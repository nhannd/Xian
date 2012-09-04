using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Configuration
{

    [MenuAction("show", "global-menus/MenuTools/MenuSharedConfiguration", "Show")]
    [Tooltip("show", "MenuOptions")]
    [GroupHint("show", "Application.Configuration")]
    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class SharedConfigurationTool : Tool<IDesktopToolContext>
    {
        public void Show()
        {
            try
            {
                SharedConfigurationDialog.Show(this.Context.DesktopWindow);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }
    }
}
