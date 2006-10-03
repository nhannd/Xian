using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/PatientAddTool")]
    //[ButtonAction("apply", "global-toolbars/ToolbarMyTools/PatientAddTool")]
    [Tooltip("apply", "Add Patient")]
    [IconSet("apply", IconScheme.Colour, "Icons.PatientAddToolSmall.png", "Icons.PatientAddToolMedium.png", "Icons.PatientAddToolLarge.png")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientAddTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public PatientAddTool()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, new PatientEditorComponent(), "New Patient", null);
        }
    }
}
