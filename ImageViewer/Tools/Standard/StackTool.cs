using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("activate", "MenuTools/MenuToolsStandard/MenuToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardStack", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    [Tooltip("activate", "ToolbarToolsStandardStack")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.StackMedium.png", "Icons.StackLarge.png")]
    
    /// <summary>
	/// 
	/// </summary>
    [ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceToolExtensionPoint))]
    public class StackTool : MouseTool
	{
		private StackCommand _Command;
		private int _InitialPresentationImageIndex;

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

			_Command = new StackCommand(e.SelectedImageBox);
			_Command.Name = SR.CommandStack;

			// Capture state before stack
			_Command.BeginState = e.SelectedImageBox.CreateMemento();

			_InitialPresentationImageIndex = e.SelectedTile.PresentationImageIndex;

			return true;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_Command == null)
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
			if (_Command == null)
				return true;

			// If nothing's changed then just return
			if (_InitialPresentationImageIndex == e.SelectedTile.PresentationImageIndex)
			{
				_Command = null;
				return true;
			}

			//int tileIndex = e.SelectedImageBox.Tiles.IndexOf(e.SelectedTile);
			//int topLeftPresentationIndex = e.SelectedTile.PresentationImageIndex - tileIndex;

			//e.SelectedImageBox.TopLeftPresentationImageIndex = topLeftPresentationIndex;
			//e.SelectedImageBox.Draw(false);

			// Capture state after stack
			_Command.EndState = e.SelectedImageBox.CreateMemento();

            this.Workspace.CommandHistory.AddCommand(_Command);

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
