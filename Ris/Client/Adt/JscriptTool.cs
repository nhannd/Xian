using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/JScript")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class JscriptTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private JscriptComponent _component;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public JscriptTool()
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
            if (_component == null)
            {
                _component = new JscriptComponent();
                ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow,
                    _component,
                    "JScript Window",
                    ShelfDisplayHint.DockFloat,
                    delegate(IApplicationComponent c) { _component = null; });
            }
        }
    }
}
