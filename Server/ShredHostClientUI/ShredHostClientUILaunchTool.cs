using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Server.ShredHostClientUI
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/ShredHostClientUILaunchTool")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/ShredHostClientUILaunchTool")]
    [Tooltip("apply", "Place tooltip text here")]
    [IconSet("apply", IconScheme.Colour, "Icons.ShredHostClientUILaunchToolSmall.png", "Icons.ShredHostClientUILaunchToolMedium.png", "Icons.ShredHostClientUILaunchToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class ShredHostClientUILaunchTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public ShredHostClientUILaunchTool()
        {
            _enabled = true;
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
        /// Called to determine whether this tool is enabled/disabled in the UI.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            protected set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            // TODO
            // Add code here to implement the functionality of the tool
            // If this tool is associated with a workspace, you can access the workspace
            // using the Workspace property

            ApplicationComponent.LaunchAsShelf(
                this.Context.DesktopWindow,
                new ShredHostClientComponent(),
                "ShredHost Client UI",
                ShelfDisplayHint.DockLeft,
                delegate(IApplicationComponent component)
                { Console.WriteLine("Done!"); }
            );
        }
    }
}
