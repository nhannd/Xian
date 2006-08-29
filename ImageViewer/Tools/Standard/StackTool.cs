using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
    [MenuAction("activate", "global-menus/MenuTools/MenuToolsStandard/MenuToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardStack")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.StackMedium.png", "Icons.StackLarge.png")]
    
    /// <summary>
	/// 
	/// </summary>
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class StackTool : DynamicActionMouseTool
	{
		private StackCommand _command;
		private int _initialPresentationImageIndex;

		public StackTool()
            :base(XMouseButtons.Left, true)
		{
		}

		#region IUIEventHandler Members

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.SelectedTile == null)
				return true;

			_command = new StackCommand(e.SelectedImageBox);
			_command.Name = SR.CommandStack;

			// Capture state before stack
			_command.BeginState = e.SelectedImageBox.CreateMemento();

			_initialPresentationImageIndex = e.SelectedTile.PresentationImageIndex;

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_command == null)
				return true;

			int increment;

			if (base.DeltaY > 0)
				increment = 1;
			else
				increment = -1;

			e.SelectedTile.PresentationImageIndex += increment;
			//e.SelectedTile.PresentationImage.Draw(true);

			int tileIndex = e.SelectedImageBox.Tiles.IndexOf(e.SelectedTile);
			int topLeftPresentationIndex = e.SelectedTile.PresentationImageIndex - tileIndex;

			e.SelectedImageBox.TopLeftPresentationImageIndex = topLeftPresentationIndex;
			e.SelectedImageBox.Draw(true);

			return true;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			base.OnMouseUp(e); 
			
			e.SelectedImageBox.Draw(true);

			if (_command == null)
				return true;

			// If nothing's changed then just return
			if (_initialPresentationImageIndex == e.SelectedTile.PresentationImageIndex)
			{
				_command = null;
				return true;
			}

			//int tileIndex = e.SelectedImageBox.Tiles.IndexOf(e.SelectedTile);
			//int topLeftPresentationIndex = e.SelectedTile.PresentationImageIndex - tileIndex;

			//e.SelectedImageBox.TopLeftPresentationImageIndex = topLeftPresentationIndex;
			//e.SelectedImageBox.Draw(false);

			// Capture state after stack
			_command.EndState = e.SelectedImageBox.CreateMemento();

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
