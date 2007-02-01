using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
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

		private void CaptureBeginState(IVOILUTLinearProvider image)
		{
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
			IVOILUTLinearProvider image = this.Context.Viewer.SelectedPresentationImage as IVOILUTLinearProvider;

			if (image == null)
				return;

			//InitImage(image);
			this.CaptureBeginState(image);
			this.IncrementWindow(image, windowIncrement, levelIncrement);
			this.CaptureEndState();
		}

		private void IncrementWindow(IVOILUTLinearProvider image, double windowIncrement, double levelIncrement)
		{
			CodeClock counter = new CodeClock();
			counter.Start();

			image.VoiLut.WindowWidth += windowIncrement;
			image.VoiLut.WindowCenter += levelIncrement;
			image.Draw();

			counter.Stop();

			string str = String.Format("WindowLevel: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			IVOILUTLinearProvider image = this.Context.Viewer.SelectedPresentationImage as IVOILUTLinearProvider;

			if (image == null)
				return false;

			CaptureBeginState(image);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			IVOILUTLinearProvider image = this.Context.Viewer.SelectedPresentationImage as IVOILUTLinearProvider;

			if (image == null)
				return false;

			IncrementWindow(image, this.DeltaX * 10, this.DeltaY * 10);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			this.CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}
	}
}
