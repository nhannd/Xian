using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Samples.Google.Calendar
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/SchedulingTool")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/SchedulingTool")]
    [Tooltip("apply", "SchedulingToolTooltip")]
    [IconSet("apply", IconScheme.Colour, "Icons.SchedulingToolSmall.png", "Icons.SchedulingToolMedium.png", "Icons.SchedulingToolLarge.png")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class SchedulingTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private Shelf _shelf;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public SchedulingTool()
        {
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            if (_shelf == null)
            {
                _shelf = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    new SchedulingComponent(),
                    SR.SchedulingTool,
                    ShelfDisplayHint.DockRight,
                    delegate(IApplicationComponent c)
                    {
                        _shelf = null;
                    });
            }
            else
            {
                _shelf.Activate();
            }
        }
    }
}
