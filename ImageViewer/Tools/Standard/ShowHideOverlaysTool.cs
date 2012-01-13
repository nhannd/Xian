#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[DropDownButtonAction("dropdown", "global-toolbars/ToolbarStandard/ToolbarShowHideOverlays", "ToggleAll", "DropDownActionModel", KeyStroke = XKeys.O)]
	[Tooltip("dropdown", "TooltipShowHideOverlays")]
	[GroupHint("dropdown", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("dropdown", "Icons.ShowHideOverlaysToolSmall.png", "Icons.ShowHideOverlaysToolMedium.png", "Icons.ShowHideOverlaysToolLarge.png")]
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

		public void ToggleAll()
		{
			// get the current check state
			bool currentCheckState = true;
			foreach (OverlayToolBase tool in OverlayToolBase.EnumerateTools(this.ImageViewer))
			{
				if (!tool.Checked)
				{
					currentCheckState = false;
					break;
				}
			}

			// invert the check state
			currentCheckState = !currentCheckState;

			// apply new check state to all
			foreach (OverlayToolBase tool in OverlayToolBase.EnumerateTools(this.ImageViewer))
			{
				tool.Checked = currentCheckState;
			}

			this.Context.Viewer.PhysicalWorkspace.Draw();
		}
	}
}