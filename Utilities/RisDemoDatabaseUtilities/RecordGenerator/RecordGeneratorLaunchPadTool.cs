#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    [MenuAction("apply", "global-menus/Demo/Record Generator Launch Pad")]
    //[ButtonAction("apply", "global-toolbars/ToolbarMyTools/RecordGeneratorLaunchPadTool")]
    [Tooltip("apply", "Launches Demo Entity Generators")]
    [IconSet("apply", IconScheme.Colour, "Icons.RecordGeneratorLaunchPadToolSmall.png", "Icons.RecordGeneratorLaunchPadToolMedium.png", "Icons.RecordGeneratorLaunchPadToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class RecordGeneratorLaunchPadTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;
        private RecordGeneratorLaunchPadComponent _component;
        

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public RecordGeneratorLaunchPadTool()
        {
            _enabled = true;
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            _component = null;
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
            if (_component == null)
            {
                _component = new RecordGeneratorLaunchPadComponent(this.Context.DesktopWindow);

                ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow,
                    _component,
                    "Entity Generator Launch Pad",
                    ShelfDisplayHint.DockLeft,
                    delegate(IApplicationComponent component) { _component = null; });
            }
        }
    }
}
