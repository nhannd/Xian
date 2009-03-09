using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[DropDownAction("dropdown", "global-toolbars/ToolbarStandard/ToolbarShowHideOverlays", "DropDownActionModel")]
	[Tooltip("dropdown", "TooltipShowHideOverlays")]
	[GroupHint("dropdown", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("dropdown", IconScheme.Colour, "Icons.ShowHideOverlaysToolSmall.png", "Icons.ShowHideOverlaysToolMedium.png", "Icons.ShowHideOverlaysToolLarge.png")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ShowHideOverlaysTool : ImageViewerTool
	{
		private ActionModelNode _mainDropDownActionModel;

		public ShowHideOverlaysTool() {}

		public ActionModelNode DropDownActionModel
		{
			get
			{
				if (_mainDropDownActionModel == null)
				{
					_mainDropDownActionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "overlays-dropdown", this.ImageViewer.ExportedActions);
				}

				ActionModelRoot model = new ActionModelRoot();
				model.Merge(_mainDropDownActionModel);
				if (base.SelectedPresentationImage is IDicomPresentationImage)
				{
					IDicomPresentationImage image = (IDicomPresentationImage) base.SelectedPresentationImage;
					if (image.DicomOverlayPlanes.Count > 0)
					{
						ActionModelRoot overlaysModel = new ActionModelRoot();
						overlaysModel.InsertSeparator(new ActionPath("overlays/separator", null));
						foreach (DicomOverlayPlane overlay in image.DicomOverlayPlanes)
						{
							overlaysModel.InsertAction(new OverlayToggleMenuAction(overlay));
						}
						model.Merge(overlaysModel);
					}
				}
				return model;
			}
		}

		private class OverlayToggleMenuAction : MenuAction
		{
			private static readonly ResourceResolver _resourceResolver = new ResourceResolver(typeof (OverlayToggleMenuAction).Assembly);

			private readonly DicomOverlayPlane _overlay;

			public OverlayToggleMenuAction(DicomOverlayPlane overlay)
				: base(Guid.NewGuid().ToString(), new ActionPath("overlays/" + overlay.Name, null), ClickActionFlags.CheckAction, _resourceResolver)
			{
				_overlay = overlay;

				this.Checked = overlay.Visible;
				this.Label = overlay.Name;
				this.Persistent = false;
				this.GroupHint = new GroupHint("Tools.Image.Overlays.DicomOverlayPlanes.ShowHide");
				this.SetClickHandler(this.Toggle);
			}

			private void Toggle()
			{
				_overlay.Visible = this.Checked = !this.Checked;
				_overlay.DrawGraphic();
			}
		}
	}
}