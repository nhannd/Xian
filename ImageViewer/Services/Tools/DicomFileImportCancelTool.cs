using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{

	[MenuAction("cancel", "dicom-file-import-contextmenu/Cancel")]
	[Tooltip("cancel", "Cancel")]
	[ClickHandler("cancel", "Cancel")]
	[EnabledStateObserver("cancel", "CancelEnabled", "CancelEnabledChanged")]

	[MenuAction("clear", "dicom-file-import-contextmenu/Clear")]
	[Tooltip("clear", "Clear")]
	[ClickHandler("clear", "Clear")]
	[EnabledStateObserver("clear", "ClearEnabled", "ClearEnabledChanged")]

	[ButtonAction("clearInactive", "dicom-file-import-toolbar/ClearInactive")]
	[Tooltip("clearInactive", "TooltipClearInactive")]
	[IconSet("clearInactive", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[ClickHandler("clearInactive", "ClearInactive")]
	[EnabledStateObserver("clearInactive", "ClearInactiveEnabled", "ClearInactiveEnabledChanged")]

	[ExtensionOf(typeof(DicomFileImportComponentToolExtensionPoint))]
	public class DicomFileImportCancelTool : Tool<IDicomFileImportComponentToolContext>
	{
		private bool _cancelEnabled;
		private bool _clearEnabled;
		private bool _clearInactiveEnabled;
		private event EventHandler _cancelEnabledChanged;
		private event EventHandler _clearEnabledChanged;
		private event EventHandler _clearInactiveEnabledChanged;

		public DicomFileImportCancelTool()
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
			this.ClearInactiveEnabled = this.Context.NumberOfEntries > 0;
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
	}
}
