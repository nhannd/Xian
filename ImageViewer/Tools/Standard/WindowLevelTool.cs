using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardWindowLevel", Flags = ClickActionFlags.CheckAction)]
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsStandard/MenuToolsStandardWindowLevel", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardWindowLevel", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardWindowLevel")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.WindowLevelMedium.png", "Icons.WindowLevelLarge.png")]
    
    /// <summary>
	/// Summary description for WindowLevelTool.
	/// </summary>
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class WindowLevelTool : DynamicActionMouseTool
	{
		private UndoableCommand _command;
		private WindowLevelApplicator _applicator;

		public WindowLevelTool()
            :base(XMouseButtons.Right, true)
		{
        }

		public override void Initialize()
        {
            base.Initialize();
		}

		#region IUIEventHandler Members

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.SelectedTile == null)
				return true;

			DicomPresentationImage dicomImage = e.SelectedPresentationImage as DicomPresentationImage;

			if (dicomImage == null ||
				dicomImage.LayerManager.SelectedImageLayer == null ||
				dicomImage.LayerManager.SelectedImageLayer.IsColor ||
				dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline == null)
				return true;

			IGrayscaleLUT lut = dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.VoiLUT;

			// If the VOILUT of the image is not linear anymore, install a linear one
			if (!(lut is VOILUTLinear))
				WindowLevelOperator.InstallVOILUTLinear(e.SelectedPresentationImage as DicomPresentationImage);

			_applicator = new WindowLevelApplicator(e.SelectedPresentationImage);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandWindowLevel;
			_command.BeginState = _applicator.CreateMemento();

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_command == null)
				return true;

            CodeClock counter = new CodeClock();
			counter.Start();

			DicomPresentationImage dicomImage = e.SelectedPresentationImage as DicomPresentationImage;

			if (dicomImage == null ||
				dicomImage.LayerManager.SelectedImageLayer == null ||
				dicomImage.LayerManager.SelectedImageLayer.IsColor ||
				dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline == null)
				return true;

			GrayscaleLUTPipeline pipeline = dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline;
			VOILUTLinear voiLUT = pipeline.VoiLUT as VOILUTLinear;

			// This should never happens since we insure that linear VOILUT is
			// installed in OnMouseDown
			if (voiLUT == null)
				return true;

			voiLUT.WindowWidth += (double)base.DeltaX * 10;
			voiLUT.WindowCenter += (double)base.DeltaY * 10;

			e.SelectedPresentationImage.Draw(true);

			counter.Stop();

			string str = String.Format("WindowLevel: {0}\n", counter.ToString());
			Trace.Write(str);

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
			return true;
		}

		public override bool OnKeyUp(XKeyEventArgs e)
		{
			return true;
		}

		#endregion
    }
}
