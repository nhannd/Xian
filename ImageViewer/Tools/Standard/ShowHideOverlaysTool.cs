using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

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
				return _mainDropDownActionModel;
			}
		}
	}
}