using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("show", "global-menus/Admin/HL7/Queue")]
    [Tooltip("show", "HL7 Queue")]
    [ClickHandler("show", "Show")]
    [ActionPermission("show", AuthorityTokens.HL7Admin)]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    class HL7QueuePreviewTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Show()
        {
            try
            {
                if (_workspace == null)
                {
                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        new HL7QueuePreviewComponent(),
                        SR.TitleHL7Queue,
                        delegate(IApplicationComponent component) { _workspace = null; });
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
