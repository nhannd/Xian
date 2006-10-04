using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
    [MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardPan", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardPan")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.PanMedium.png", "Icons.PanLarge.png")]
    
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PanTool : DynamicActionMouseTool
	{
		private UndoableCommand _command;
		private SpatialTransformApplicator _applicator;

		public PanTool()
            :base(XMouseButtons.Left, false)
		{
		}

		#region IUIEventHandler Members

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.SelectedPresentationImage == null)
				return true;

			_applicator = new SpatialTransformApplicator(e.SelectedPresentationImage);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandPan;
			_command.BeginState = _applicator.CreateMemento();

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_command == null)
				return true;

			SpatialTransform spatialTransform = e.SelectedPresentationImage.LayerManager.SelectedLayerGroup.SpatialTransform;
			float scale = spatialTransform.Scale;
			Platform.CheckPositive(scale, "spatialTransform.Scale");

			spatialTransform.TranslationX += (float)base.DeltaX / scale;
			spatialTransform.TranslationY += (float)base.DeltaY / scale;
			spatialTransform.Calculate();
			e.SelectedPresentationImage.Draw(true);

			return true;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (_command == null)
				return true;

			_command.EndState = _applicator.CreateMemento();

			// If the state hasn't changed since MouseDown just return
			if (_command.EndState.Equals(_command.BeginState))
			{
				_command = null;
				return true;
			}

			// Apply the final state to all linked images
			_applicator.SetMemento(_command.EndState);

            this.Context.Viewer.CommandHistory.AddCommand(_command);

			return true;
		}

		public override bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return true;
		}

		public override bool OnKeyDown(XKeyEventArgs e)
		{
			return false;
		}


		public override bool OnKeyUp(XKeyEventArgs e)
		{
			return false;
		}

		#endregion

    }
}
