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

namespace $rootnamespace$
{
    // This template provides the boiler-plate code for creating a basic tool
    // that performs a single action when its menu item or toolbar button is clicked.

	// Declares a menu action with action ID "apply"
    // TODO: Change the action path hint to your desired menu path, or
    // remove this attribute if you do not want to create a menu item for this tool
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/$tool$", "Apply")]

    // Declares a toolbar button action with action ID "apply"
    // TODO: Change the action path hint to your desired toolbar path, or
    // remove this attribute if you do not want to create a toolbar button for this tool
	[ButtonAction("apply", "global-toolbars/ToolbarMyTools/$tool$", "Apply")]

    // Specifies tooltip text for the "apply" action
    // TODO: Replace tooltip text
	[Tooltip("apply", "Place tooltip text here")]

    // Specifies icon resources to use for the "apply" action
    // TODO: Replace the icon resource names with your desired icon resources
	[IconSet("apply", IconScheme.Colour, "Icons.$tool$Small.png", "Icons.$tool$Medium.png", "Icons.$tool$Large.png")]
    
	// Specifies that the enablement of the "apply" action in the user-interface
    // is controlled by observing a boolean property named "Enabled", listening to
    // an event named "EnabledChanged" for changes to this property
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof($tool_ext_point$))]
    public class $tool$: Tool<$tool_context$>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        /// <summary>
        /// Default constructor.
        /// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
        public $tool$()
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
        /// Gets whether this tool is enabled/disabled in the UI.
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
        /// Notifies that the <see cref="Enabled"/> state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

		// Note: you may change the name of the 'Apply' method as desired, but be sure to change the
        // corresponding parameter in the MenuAction and ButtonAction attributes

		/// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
		public void Apply()
        {
            // TODO: add code here to implement the functionality of the tool
        }
    }
}
