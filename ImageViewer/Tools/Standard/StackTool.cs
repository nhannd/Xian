#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarStack", "Select", "SortMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.S)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.StackToolSmall.png", "Icons.StackToolMedium.png", "Icons.StackToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Standard")]

	[MouseWheelHandler(ModifierFlags.None)]
	[MouseToolButton(XMouseButtons.Left, true)]

	[KeyboardAction("stackup", "imageviewer-keyboard/ToolsStandardStack/StackUp", "StackUp", KeyStroke = XKeys.PageUp)]
	[KeyboardAction("stackdown", "imageviewer-keyboard/ToolsStandardStack/StackDown", "StackDown", KeyStroke = XKeys.PageDown)]
	[KeyboardAction("jumptobeginning", "imageviewer-keyboard/ToolsStandardStack/JumpToBeginning", "JumpToBeginning", KeyStroke = XKeys.Home)]
	[KeyboardAction("jumptoend", "imageviewer-keyboard/ToolsStandardStack/JumpToEnd", "JumpToEnd", KeyStroke = XKeys.End)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class StackTool : MouseImageViewerTool
	{
		private MemorableUndoableCommand _memorableCommand;
		private int _initialPresentationImageIndex;
		private IImageBox _currentImageBox;

		public StackTool()
			: base(SR.TooltipStack)
		{
			CursorToken = new CursorToken("Icons.StackToolSmall.png", GetType().Assembly);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode SortMenuModel
		{
			get
			{
				var actionModel = new SimpleActionModel(new ApplicationThemeResourceResolver(GetType().Assembly));
				foreach (var item in ImageComparerList.Items)
				{
					var itemVar = item;
					var action = actionModel.AddAction(itemVar.Name, itemVar.Description, null, itemVar.Description, () => Sort(itemVar));
					action.Checked = GetSortMenuItemCheckState(itemVar);
				}

				return actionModel;
			}
		}

        private bool GetSortMenuItemCheckState(ImageComparerList.Item item)
		{
			return SelectedPresentationImage != null && SelectedPresentationImage.ParentDisplaySet != null &&
			       item.Comparer.Equals(SelectedPresentationImage.ParentDisplaySet.PresentationImages.SortComparer);
		}

        private void Sort(ImageComparerList.Item item)
		{
			IImageBox imageBox = ImageViewer.SelectedImageBox;
			IDisplaySet displaySet;
			if (imageBox == null || (displaySet = ImageViewer.SelectedImageBox.DisplaySet) == null)
				return;

			if (displaySet.PresentationImages.Count == 0)
				return;

			//try to keep the top-left image the same.
			IPresentationImage topLeftImage = imageBox.TopLeftPresentationImage;

			var command = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};

            displaySet.PresentationImages.Sort(item.Comparer);
			imageBox.TopLeftPresentationImage = topLeftImage;
			imageBox.Draw();

			command.EndState = imageBox.CreateMemento();
			if (!command.BeginState.Equals(command.EndState))
			{
				var historyCommand = new DrawableUndoableCommand(imageBox) {Name = SR.CommandSortImages};
			    historyCommand.Enqueue(command);
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		private void CaptureBeginState(IImageBox imageBox)
		{
			_memorableCommand = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};
			// Capture state before stack
		    _currentImageBox = imageBox;

			_initialPresentationImageIndex = imageBox.SelectedTile.PresentationImageIndex;
		}

		private void CaptureEndState()
		{
			if (_memorableCommand == null || _currentImageBox == null)
			{
				_currentImageBox = null;
				return;
			}

			// If nothing's changed then just return
			if (_initialPresentationImageIndex != _currentImageBox.SelectedTile.PresentationImageIndex)
			{
				// Capture state after stack
				_memorableCommand.EndState = _currentImageBox.CreateMemento();
				if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				{
					var historyCommand = new DrawableUndoableCommand(_currentImageBox) {Name = SR.CommandStack};
				    historyCommand.Enqueue(_memorableCommand);
					Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}

			_memorableCommand = null;
			_currentImageBox = null;
		}

		private void JumpToBeginning()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = 0;
			imageBox.Draw();
			CaptureEndState();
		}

		private void JumpToEnd()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;

			if (imageBox.DisplaySet == null)
				return;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count - 1;
			imageBox.Draw();
			CaptureEndState();
		}

		private void StackUp()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(-imageBox.Tiles.Count, imageBox);
			CaptureEndState();
		}

		private void StackDown()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(+imageBox.Tiles.Count, imageBox);
			CaptureEndState();
		}

		private static void AdvanceImage(int increment, IImageBox selectedImageBox)
		{
		    int prevTopLeftPresentationImageIndex = selectedImageBox.TopLeftPresentationImageIndex;
			selectedImageBox.TopLeftPresentationImageIndex += increment;

            if (selectedImageBox.TopLeftPresentationImageIndex != prevTopLeftPresentationImageIndex)
                selectedImageBox.Draw(); 
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Start(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			CaptureBeginState(mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Track(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			if (DeltaY == 0)
				return true;

			int increment;

			if (DeltaY > 0)
				increment = 1;
			else
				increment = -1;

			AdvanceImage(increment, mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureEndState();
		}

		public override void StartWheel()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			if (imageBox == null)
				return;

			CaptureBeginState(imageBox);
		}

		protected override void WheelBack()
		{
			if (this.SelectedPresentationImage == null)
				return;

			AdvanceImage(1, Context.Viewer.SelectedTile.ParentImageBox);
		}

		protected override void WheelForward()
		{
			if (this.SelectedPresentationImage == null)
				return;

			AdvanceImage(-1, Context.Viewer.SelectedTile.ParentImageBox);
		}

		public override void StopWheel()
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureEndState();
		}
	}
}
