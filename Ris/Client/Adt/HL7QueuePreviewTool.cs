using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("show", "global-menus/Admin/HL7/Queue")]
    [Tooltip("show", "HL7 Queue")]
    [ClickHandler("show", "Show")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    class HL7QueuePreviewTool : Tool<IDesktopToolContext>
    {
        private HL7QueuePreviewComponent _component;
        
        public override void Initialize()
        {
            base.Initialize();
        }

        public void Show()
        {
            if (_component == null)
            {
                _component = new HL7QueuePreviewComponent();

                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    _component,
                    SR.TitleHL7Queue,
                    delegate(IApplicationComponent component) { _component = null; });
            }
        }

    }
}
