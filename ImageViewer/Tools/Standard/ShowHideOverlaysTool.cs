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
					ActionModelRoot root = new ActionModelRoot();
					ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

					IDicomPresentationImage image = (IDicomPresentationImage)base.SelectedPresentationImage;
					foreach (DicomOverlayPlane overlay in image.DicomOverlayPlanes)
					{
						// temporary actions to toggle view
						MenuAction action = new MenuAction(Guid.NewGuid().ToString(), new ActionPath("overlays/" + overlay.Name, null), ClickActionFlags.None, resolver);
						action.SetClickHandler(delegate
						                       	{
						                       		overlay.Visible = !overlay.Visible;
						                       		overlay.Graphic.Draw();
						                       	});
						root.InsertAction(action);
					}
					model.Merge(root);
				}
				return model;
			}
		}
	}
}