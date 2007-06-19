using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/Technologist Documentation")]
    [ClickHandler("apply", "Apply")]
    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class TechnologistDocumentationTestTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Apply()
        {
            try
            {
                if (_workspace == null)
                {
                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        new TechnologistDocumentationComponent(),
                        "Technologist Documentation",
                        delegate(IApplicationComponent c) { _workspace = null;  });
                }
                else
                {
                    _workspace.Activate();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }
    }
}