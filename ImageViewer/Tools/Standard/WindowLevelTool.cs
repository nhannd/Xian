using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Tools.Standard;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MouseToolButton(XMouseButtons.Right, true)]

	[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardWindowLevel", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardWindowLevel/Activate", KeyStroke = XKeys.W)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardWindowLevel", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardWindowLevel", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardWindowLevel")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.WindowLevelMedium.png", "Icons.WindowLevelLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Lut.WindowLevel")]

	[KeyboardAction("incrementwindowwidth", "imageviewer-keyboard/ToolsStandardWindowLevel/IncrementWindowWidth", KeyStroke = XKeys.Right)]
	[ClickHandler("incrementwindowwidth", "IncrementWindowWidth")]

	[KeyboardAction("decrementwindowwidth", "imageviewer-keyboard/ToolsStandardWindowLevel/DecrementWindowWidth", KeyStroke = XKeys.Left)]
	[ClickHandler("decrementwindowwidth", "DecrementWindowWidth")]

	[KeyboardAction("incrementwindowcenter", "imageviewer-keyboard/ToolsStandardWindowLevel/IncrementWindowCenter", KeyStroke = XKeys.Up)]
	[ClickHandler("incrementwindowcenter", "IncrementWindowCenter")]

	[KeyboardAction("decrementwindowcenter", "imageviewer-keyboard/ToolsStandardWindowLevel/DecrementWindowCenter", KeyStroke = XKeys.Down)]
	[ClickHandler("decrementwindowcenter", "DecrementWindowCenter")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class WindowLevelTool : MouseTool
	{
		private UndoableCommand _command;
		private WindowLevelApplicator _applicator;

		public WindowLevelTool()
		{
			this.CursorToken = new CursorToken("Icons.WindowLevelMedium.png", this.GetType().Assembly);
        }

		public override void Initialize()
        {
            base.Initialize();
		}

		private void CaptureBeginState(IPresentationImage image)
		{
			if (image == null)
				return;

			_applicator = new WindowLevelApplicator(image);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandWindowLevel;
			_command.BeginState = _applicator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (_command == null)
				return;

			_command.EndState = _applicator.CreateMemento();

			// If the state hasn't changed since MouseDown just return
			if (_command.EndState.Equals(_command.BeginState))
			{
				_command = null;
				return;
			}

			// Apply the final state to all linked images
			_applicator.SetMemento(_command.EndState);

			this.Context.Viewer.CommandHistory.AddCommand(_command);
		}

		private void InitImage(IPresentationImage image)
		{
			DicomPresentationImage dicomImage = image as DicomPresentationImage;

			if (dicomImage == null ||
				dicomImage.LayerManager.SelectedImageLayer == null ||
				dicomImage.LayerManager.SelectedImageLayer.IsColor ||
				dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline == null)
				return;

			IGrayscaleLUT lut = dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.VoiLUT;

			// If the VOILUT of the image is not linear anymore, install a linear one
			if (!(lut is VOILUTLinear))
				WindowLevelOperator.InstallVOILUTLinear(dicomImage);
		}

		private void IncrementWindowWidth()
		{
			IncrementWindow(10, 0);
		}

		private void DecrementWindowWidth()
		{
			IncrementWindow(-10, 0);
		}

		private void IncrementWindowCenter()
		{
			IncrementWindow(0, 10);
		}

		private void DecrementWindowCenter()
		{
			IncrementWindow(0, -10);
		}

		public void IncrementWindow(double windowIncrement, double levelIncrement)
		{
			IPresentationImage image = this.Context.Viewer.SelectedPresentationImage;

			if (!IsImageValid(image))
				return;

			InitImage(image);
			this.CaptureBeginState(image);
			this.IncrementWindow(image, windowIncrement, levelIncrement);
			this.CaptureEndState();
		}

		private void IncrementWindow(IPresentationImage image, double windowIncrement, double levelIncrement)
		{
			CodeClock counter = new CodeClock();
			counter.Start();

			DicomPresentationImage dicomImage = image as DicomPresentationImage;

			if (dicomImage == null ||
				dicomImage.LayerManager.SelectedImageLayer == null ||
				dicomImage.LayerManager.SelectedImageLayer.IsColor ||
				dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline == null)
				return;

			GrayscaleLUTPipeline pipeline = dicomImage.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline;
			VOILUTLinear voiLUT = pipeline.VoiLUT as VOILUTLinear;

			// This should never happens since we insure that linear VOILUT is
			// installed in OnMouseDown
			if (voiLUT == null)
				return;

			voiLUT.WindowWidth += windowIncrement;
			voiLUT.WindowCenter += levelIncrement;

			dicomImage.Draw();

			counter.Stop();

			string str = String.Format("WindowLevel: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (!IsImageValid(mouseInformation.Tile.PresentationImage))
				return false;

			InitImage(mouseInformation.Tile.PresentationImage);

			CaptureBeginState(mouseInformation.Tile.PresentationImage);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			if (!IsImageValid(mouseInformation.Tile.PresentationImage))
				return false;

			if (_command == null)
				return false;

			this.IncrementWindow(mouseInformation.Tile.PresentationImage, this.DeltaX * 10, this.DeltaY * 10);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			if (!IsImageValid(mouseInformation.Tile.PresentationImage))
				return true;

			this.CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}
	}
}
