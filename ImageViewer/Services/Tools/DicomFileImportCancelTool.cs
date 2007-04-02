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
	[IconSet("cancel", IconScheme.Colour, "Icons.CancelSmall.png", "Icons.CancelSmall.png", "Icons.CancelSmall.png")]
	[ClickHandler("cancel", "Cancel")]
	[EnabledStateObserver("cancel", "CancelEnabled", "CancelEnabledChanged")]

	[MenuAction("clear", "dicom-file-import-contextmenu/Clear")]
	[Tooltip("clear", "Clear")]
	[IconSet("clear", IconScheme.Colour, "Icons.ClearSmall.png", "Icons.ClearSmall.png", "Icons.ClearSmall.png")]
	[ClickHandler("clear", "Clear")]
	[EnabledStateObserver("clear", "ClearEnabled", "ClearEnabledChanged")]

	[ButtonAction("clearInactive", "dicom-file-import-toolbar/ClearInactive")]
	[Tooltip("clearInactive", "Clear Inactive")]
	[IconSet("clearInactive", IconScheme.Colour, "Icons.ClearInactiveSmall.png", "Icons.ClearInactiveSmall.png", "Icons.ClearInactiveSmall.png")]
	[ClickHandler("clearInactive", "ClearInactive")]
	//[EnabledStateObserver("clearInactive", "ClearInactiveEnabled", "ClearInactiveEnabledChanged")]

	[ExtensionOf(typeof(DicomFileImportComponentToolExtensionPoint))]
	public class DicomFileImportCancelTool : Tool<IDicomFileImportComponentToolContext>
	{
		private bool _cancelEnabled;
		private bool _clearEnabled;
		private event EventHandler _cancelEnabledChanged;
		private event EventHandler _clearEnabledChanged;

		public DicomFileImportCancelTool()
		{
			_clearEnabled = false;
			_cancelEnabled = false;
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
