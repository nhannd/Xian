using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Clipboard;
using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ButtonAction("edit", KeyImageClipboard.ToolbarSite + "/ToolbarEditKeyImageInformation", "Edit")]
	[Tooltip("edit", "TooltipEditKeyImageInformation")]
	[IconSet("edit", IconScheme.Colour, "Icons.EditKeyImageInformationToolSmall.png", "Icons.EditKeyImageInformationToolMedium.png", "Icons.EditKeyImageInformationToolLarge.png")]
	[EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	internal class EditKeyImageInformationTool : Tool<IClipboardToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public EditKeyImageInformationTool()
		{
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }	
			remove { _enabledChanged -= value; }	
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.ClipboardItemsChanged += OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged += OnSelectionChanged;
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			this.Context.ClipboardItemsChanged -= OnClipboardItemsChanged;
			this.Context.SelectedClipboardItemsChanged -= OnSelectionChanged;

			base.Dispose(disposing);
		}

		private void OnClipboardItemsChanged(object sender, EventArgs e)
		{
			Enabled = Context.ClipboardItems.Count > 0;
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Enabled = Context.ClipboardItems.Count > 0;
		}

		public void Edit()
		{
			//TODO: can we use an override to add actions?
			KeyImageInformationEditorComponent.Launch(this.Context.DesktopWindow);
		}
	}
}
