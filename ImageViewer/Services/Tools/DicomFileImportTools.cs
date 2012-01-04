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

	[MenuAction("cancel", "dicom-file-import-contextmenu/MenuCancel", "Cancel")]
	[Tooltip("cancel", "TooltipCancel")]
	[EnabledStateObserver("cancel", "CancelEnabled", "CancelEnabledChanged")]

	[MenuAction("clear", "dicom-file-import-contextmenu/MenuClear", "Clear")]
	[Tooltip("clear", "TooltipClear")]
	[EnabledStateObserver("clear", "ClearEnabled", "ClearEnabledChanged")]

	[ButtonAction("clearInactive", "dicom-file-import-toolbar/ClearInactive", "ClearInactive")]
	[Tooltip("clearInactive", "TooltipClearInactive")]
	[IconSet("clearInactive", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[EnabledStateObserver("clearInactive", "ClearInactiveEnabled", "ClearInactiveEnabledChanged")]

	[ButtonAction("showBackground", "dicom-file-import-toolbar/ShowBackgroundImports", "ToggleShowBackground", Flags = ClickActionFlags.CheckAction)]
	[Tooltip("showBackground", "TooltipShowBackgroundImports")]
	[IconSet("showBackground", "Icons.ShowBackgroundImportsToolSmall.png", "Icons.ShowBackgroundImportsToolSmall.png", "Icons.ShowBackgroundImportsToolSmall.png")]
	[CheckedStateObserver("showBackground", "ShowBackground", "ShowBackgroundChanged")]

	[ExtensionOf(typeof(DicomFileImportComponentToolExtensionPoint))]
	public class DicomFileImportTools : Tool<IDicomFileImportComponentToolContext>
	{
		private bool _cancelEnabled;
		private bool _clearEnabled;
		private bool _clearInactiveEnabled;
		private event EventHandler _cancelEnabledChanged;
		private event EventHandler _clearEnabledChanged;
		private event EventHandler _clearInactiveEnabledChanged;
		private event EventHandler _showBackgroundChanged;

		public DicomFileImportTools()
		{
			_clearEnabled = false;
			_cancelEnabled = false;
			_clearInactiveEnabled = false;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectionUpdated += new EventHandler(OnSelectionUpdated);
		}

		private void OnSelectionUpdated(object sender, EventArgs e)
		{
			this.CancelEnabled = this.Context.CanCancelSelected();
			this.ClearEnabled = this.Context.CanClearSelected();
			this.ClearInactiveEnabled = this.Context.CanClearInactive();
		}

		public bool CancelEnabled
		{
			get { return _cancelEnabled; }
			protected set
			{
				if (_cancelEnabled != value)
				{
					_cancelEnabled = value;
					EventsHelper.Fire(_cancelEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ClearEnabled
		{
			get { return _clearEnabled; }
			protected set
			{
				if (_clearEnabled != value)
				{
					_clearEnabled = value;
					EventsHelper.Fire(_clearEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ClearInactiveEnabled
		{
			get { return _clearInactiveEnabled; }
			protected set
			{
				if (_clearInactiveEnabled != value)
				{
					_clearInactiveEnabled = value;
					EventsHelper.Fire(_clearInactiveEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ShowBackground
		{
			get { return base.Context.ShowBackgroundImports; }
			set
			{
				if (base.Context.ShowBackgroundImports != value)
				{
					base.Context.ShowBackgroundImports = value;
					EventsHelper.Fire(_showBackgroundChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler CancelEnabledChanged
		{
			add { _cancelEnabledChanged += value; }
			remove { _cancelEnabledChanged -= value; }
		}

		public event EventHandler ClearEnabledChanged
		{
			add { _clearEnabledChanged += value; }
			remove { _clearEnabledChanged -= value; }
		}

		public event EventHandler ClearInactiveEnabledChanged
		{
			add { _clearInactiveEnabledChanged += value; }
			remove { _clearInactiveEnabledChanged -= value; }
		}

		public event EventHandler ShowBackgroundChanged
		{
			add { _showBackgroundChanged += value; }
			remove { _showBackgroundChanged -= value; }
		}

		private void Cancel()
		{
			try
			{
				this.Context.CancelSelected();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageCancelFailed, this.Context.DesktopWindow);
			}
		}

		private void Clear()
		{
			try
			{
				this.Context.ClearSelected();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClearFailed, this.Context.DesktopWindow);
			}
		}

		private void ClearInactive()
		{
			try
			{
				this.Context.ClearInactive();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClearInactiveFailed, this.Context.DesktopWindow);
			}
		}

		private void ToggleShowBackground()
		{
			ShowBackground = !ShowBackground;
		}
	}
}
