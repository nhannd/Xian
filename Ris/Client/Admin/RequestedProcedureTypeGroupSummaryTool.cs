using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Requested Procedure Type Groups")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class RequestedProcedureTypeGroupSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    RequestedProcedureTypeGroupSummaryComponent component = new RequestedProcedureTypeGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleAuthorityGroup,
                        delegate(IApplicationComponent c) { _workspace = null; });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }
}