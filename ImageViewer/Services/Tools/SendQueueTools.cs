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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("clearSelected", "send-queue-toolbar/ClearSelected", "ClearSelected")]
	[MenuAction("clearSelected", "send-queue-contextmenu/MenuClear", "ClearSelected")]
	[Tooltip("clearSelected", "TooltipClearSelected")]
	[IconSet("clearSelected", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[EnabledStateObserver("clearSelected", "ClearSelectedEnabled", "ClearSelectedEnabledChanged")]

	[ButtonAction("clearAll", "send-queue-toolbar/ClearAll", "ClearAll")]
	[Tooltip("clearAll", "TooltipClearAll")]
	[IconSet("clearAll", "Icons.DeleteAllToolSmall.png", "Icons.DeleteAllToolSmall.png", "Icons.DeleteAllToolSmall.png")]
	[EnabledStateObserver("clearAll", "ClearAllEnabled", "ClearAllEnabledChanged")]

	[ButtonAction("showBackground", "send-queue-toolbar/ShowBackgroundSends", "ToggleShowBackground", Flags = ClickActionFlags.CheckAction)]
	[Tooltip("showBackground", "TooltipShowBackgroundSends")]
	[IconSet("showBackground", "Icons.ShowBackgroundSendsToolSmall.png", "Icons.ShowBackgroundSendsToolSmall.png", "Icons.ShowBackgroundSendsSmall.png")]
	[CheckedStateObserver("showBackground", "ShowBackground", "ShowBackgroundChanged")]

	[ExtensionOf(typeof(SendQueueApplicationComponentToolExtensionPoint))]
	public class SendQueueTools : Tool<ISendQueueApplicationComponentToolContext>
	{
		private bool _clearSelectedEnabled;
		private bool _clearAllEnabled;

		private event EventHandler _clearSelectedEnabledChanged;
		private event EventHandler _clearAllEnabledChanged;
		private event EventHandler _showBackgroundChanged;
		
		public SendQueueTools()
		{
			_clearSelectedEnabled = false;
			_clearAllEnabled = false;
		}

		private void OnSelectionUpdated(object sender, EventArgs e)
		{
			this.ClearAllEnabled = this.Context.AnyItems;
			this.ClearSelectedEnabled = this.Context.ItemsSelected;
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.Updated += new EventHandler(OnSelectionUpdated);
		}

		public bool ClearSelectedEnabled
		{
			get { return _clearSelectedEnabled; }
			protected set
			{
				if (_clearSelectedEnabled != value)
				{
					_clearSelectedEnabled = value;
					EventsHelper.Fire(_clearSelectedEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ClearAllEnabled
		{
			get { return _clearAllEnabled; }
			protected set
			{
				if (_clearAllEnabled != value)
				{
					_clearAllEnabled = value;
					EventsHelper.Fire(_clearAllEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ShowBackground
		{
			get { return base.Context.ShowBackgroundSends; }
			set
			{
				if (base.Context.ShowBackgroundSends != value)
				{
					base.Context.ShowBackgroundSends = value;
					EventsHelper.Fire(_showBackgroundChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ClearSelectedEnabledChanged
		{
			add { _clearSelectedEnabledChanged += value; }
			remove { _clearSelectedEnabledChanged -= value; }
		}

		public event EventHandler ClearAllEnabledChanged
		{
				add { _clearAllEnabledChanged += value; }
			remove { _clearAllEnabledChanged -= value; }
		}

		public event EventHandler ShowBackgroundChanged
		{
			add { _showBackgroundChanged += value; }
			remove { _showBackgroundChanged -= value; }
		}

		private void ClearSelected()
		{
			try
			{
				this.Context.ClearSelected();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageCancelFailed, this.Context.DesktopWindow);
			}
		}

		private void ClearAll()
		{
			try
			{
				this.Context.ClearAll();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageCancelFailed, this.Context.DesktopWindow);
			}
		}

		private void ToggleShowBackground()
		{
			ShowBackground = !ShowBackground;
		}
	}
}
