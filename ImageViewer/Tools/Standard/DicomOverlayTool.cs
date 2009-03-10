using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideDicomOverlay", "ShowHide")]
	[EnabledStateObserver("showHide", "Available", "AvailableChanged")]
	[Tooltip("showHide", "TooltipShowHideDicomOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	//[IconSet("showHide", IconScheme.Colour, "Icons.ScaleOverlayToolSmall.png", "Icons.ScaleOverlayToolMedium.png", "Icons.ScaleOverlayToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarDicomOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[VisibleStateObserver("toggle", "Available", "AvailableChanged")]
	[Tooltip("toggle", "TooltipDicomOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	//[IconSet("toggle", IconScheme.Colour, "Icons.ScaleOverlayToolSmall.png", "Icons.ScaleOverlayToolMedium.png", "Icons.ScaleOverlayToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class DicomOverlayTool : ImageViewerTool
	{
		private event EventHandler _availableChanged;
		private event EventHandler _checkedChanged;
		private bool _checked;

		public DicomOverlayTool()
		{
			_checked = true;
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked != value)
				{
					_checked = value;
					OnCheckedChanged();
				}
			}
		}

		public bool Available
		{
			get { return base.SelectedPresentationImage is IDicomPresentationImage; }
		}

		public event EventHandler CheckedChanged
		{
			add { _checkedChanged += value; }
			remove { _checkedChanged -= value; }
		}

		public event EventHandler AvailableChanged
		{
			add { _availableChanged += value; }
			remove { _availableChanged -= value; }
		}

		public void ShowHide()
		{
			this.Checked = !this.Checked;
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			EventsHelper.Fire(_availableChanged, this, new EventArgs());
		}

		private void OnCheckedChanged()
		{
			IDicomPresentationImage image = base.SelectedPresentationImage as IDicomPresentationImage;
			if (image != null)
			{
				foreach (DicomOverlayPlane overlay in image.DicomOverlayPlanes)
				{
					overlay.Visible = this.Checked;
					overlay.DrawGraphic();
				}
			}

			EventsHelper.Fire(_checkedChanged, this, new EventArgs());
		}
	}
}