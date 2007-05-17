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
	[ButtonAction("clearSelected", "receive-queue-toolbar/ClearSelected")]
	[MenuAction("clearSelected", "receive-queue-contextmenu/Clear")]
	[Tooltip("clearSelected", "TooltipClearSelected")]
	[IconSet("clearSelected", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[ClickHandler("clearSelected", "ClearSelected")]
	[EnabledStateObserver("clearSelected", "ClearSelectedEnabled", "ClearSelectedEnabledChanged")]

	[ButtonAction("clearAll", "receive-queue-toolbar/ClearAll")]
	[Tooltip("clearAll", "TooltipClearAll")]
	[IconSet("clearAll", IconScheme.Colour, "Icons.DeleteAllToolSmall.png", "Icons.DeleteAllToolSmall.png", "Icons.DeleteAllToolSmall.png")]
	[ClickHandler("clearAll", "ClearAll")]
	[EnabledStateObserver("clearAll", "ClearAllEnabled", "ClearAllEnabledChanged")]

	[ExtensionOf(typeof(ReceiveQueueApplicationComponentToolExtensionPoint))]
	public class ReceiveQueueTools : Tool<IReceiveQueueApplicationComponentToolContext>
	{
		private bool _clearSelectedEnabled;
		private bool _clearAllEnabled;

		private event EventHandler _clearSelectedEnabledChanged;
		private event EventHandler _clearAllEnabledChanged;

		public ReceiveQueueTools()
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
	}
}
