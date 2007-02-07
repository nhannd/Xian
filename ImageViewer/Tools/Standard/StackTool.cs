using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardStack/Activate", KeyStroke = XKeys.S)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardStack")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.StackMedium.png", "Icons.StackLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Standard")]

	[MouseWheelControl("StackDown", "StackUp")]
	[MouseToolButton(XMouseButtons.Left, true)]

	[KeyboardAction("stackup", "imageviewer-keyboard/ToolsStandardStack/StackUp", KeyStroke = XKeys.PageUp)]
	[ClickHandler("stackup", "StackUp")]

	[KeyboardAction("stackdown", "imageviewer-keyboard/ToolsStandardStack/StackDown", KeyStroke = XKeys.PageDown)]
	[ClickHandler("stackdown", "StackDown")]

	[KeyboardAction("jumptobeginning", "imageviewer-keyboard/ToolsStandardStack/JumpToBeginning", KeyStroke = XKeys.Home)]
	[ClickHandler("jumptobeginning", "JumpToBeginning")]

	[KeyboardAction("jumptoend", "imageviewer-keyboard/ToolsStandardStack/JumpToEnd", KeyStroke = XKeys.End)]
	[ClickHandler("jumptoend", "JumpToEnd")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class StackTool : MouseImageViewerTool
	{
		private UndoableCommand _command;
		private int _initialPresentationImageIndex;
		private IImageBox _currentImageBox;

		public StackTool()
		{
			this.CursorToken = new CursorToken("Icons.StackMedium.png", this.GetType().Assembly);
		}

		private void CaptureBeginState(IImageBox imageBox)
		{
			_command = new UndoableCommand(imageBox);
			_command.Name = SR.CommandStack;

			// Capture state before stack
			_command.BeginState = imageBox.CreateMemento();
			_currentImageBox = imageBox;

			_initialPresentationImageIndex = imageBox.SelectedTile.PresentationImageIndex;
		}

		private void CaptureEndState()
		{
			if (_command == null || _currentImageBox == null)
			{
				_currentImageBox = null;
				return;
			}

			// If nothing's changed then just return
			if (_initialPresentationImageIndex == _currentImageBox.SelectedTile.PresentationImageIndex)
			{
				_command = null;
				_currentImageBox = null;
				return;
			}

			// Capture state after stack
			_command.EndState = _currentImageBox.CreateMemento();
			this.Context.Viewer.CommandHistory.AddCommand(_command);

			_command = null;
			_currentImageBox = null;
		}

		private void JumpToBeginning()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = 0;
			imageBox.Draw();
			CaptureEndState();
		}

		private void JumpToEnd()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;

			if (imageBox.DisplaySet == null)
				return;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count - 1;
			imageBox.Draw();
			CaptureEndState();
		}

		private void StackUp()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(-1, imageBox);
			CaptureEndState();
		}

		private void StackDown()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(+1, imageBox);
			CaptureEndState();
		}

		private void AdvanceImage(int increment, IImageBox selectedImageBox)
		{
			selectedImageBox.TopLeftPresentationImageIndex += increment;
			selectedImageBox.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			CaptureBeginState(mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			if (base.DeltaY == 0)
				return true;

			int increment;

			if (base.DeltaY > 0)
				increment = 1;
			else
				increment = -1;

			AdvanceImage(increment, mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}
	}
}
