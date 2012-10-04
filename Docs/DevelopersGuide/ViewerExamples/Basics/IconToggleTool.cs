#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;

// ... (other using namespace statements here)

namespace MyPlugin.Basics
{
	[MenuAction("stateToggle", "global-menus/MenuTools/MenuStandard/MenuIconToggleTool", "Toggle")]
	[ButtonAction("stateToggle", "global-toolbars/ToolbarStandard/ToolbarIconToggleTool", "Toggle")]
	[Tooltip("stateToggle", "TooltipIconToggleTool")]
	[IconSetObserver("stateToggle", "StateIconSet", "StateChanged")]
	[EnabledStateObserver("stateToggle", "Enabled", "EnabledChanged")]
	//
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class IconToggleTool : ImageViewerTool
	{
		private readonly IconSet _trueIcons;
		private readonly IconSet _falseIcons;
		private bool _state;
		private event EventHandler _stateChangedEvent;

		public IconToggleTool()
		{
			_trueIcons = new IconSet(IconScheme.Colour, "Icons.TrueSmall.png", "Icons.TrueMedium.png", "Icons.TrueLarge.png");
			_falseIcons = new IconSet(IconScheme.Colour, "Icons.FalseSmall.png", "Icons.FalseMedium.png", "Icons.FalseLarge.png");
		}

		public IconSet StateIconSet
		{
			get { return this.State ? _trueIcons : _falseIcons; }
		}

		public bool State
		{
			get { return _state; }
			set
			{
				if (_state != value)
				{
					_state = value;
					EventsHelper.Fire(_stateChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler StateChanged
		{
			add { _stateChangedEvent += value; }
			remove { _stateChangedEvent -= value; }
		}

		public void Toggle()
		{
			this.State = !this.State;
		}
	}
}