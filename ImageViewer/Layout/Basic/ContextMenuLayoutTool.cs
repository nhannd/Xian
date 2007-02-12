using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Layout.Basic
{

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ContextMenuLayoutTool : ImageViewerTool
	{
		private IImageBox _lastActivatedImageBox;

		/// <summary>
        /// Constructor
        /// </summary>
		public ContextMenuLayoutTool()
		{
        }

		public override IActionSet Actions
		{
			get
			{
				return GetDisplaySetActions();
			}
		}
		
		/// <summary>
        /// Overridden to subscribe to workspace activation events
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

			this.Context.Viewer.EventBroker.TileActivated +=
				delegate(object sender, TileActivatedEventArgs e) { _lastActivatedImageBox = e.ActivatedTile.ParentImageBox; };
        }

		/// <summary>
		/// Gets an array of <see cref="IAction"/> objects that allow selection of specific display
		/// sets for display in the currently selected image box.
		/// </summary>
		/// <returns></returns>
		private IActionSet GetDisplaySetActions()
		{
			List<IAction> actions = new List<IAction>();
			int i = 0;

			foreach (IImageSet imageSet in this.ImageViewer.LogicalWorkspace.ImageSets)
			{
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
				{
					actions.Add(CreateDisplaySetAction(displaySet, ++i));
				}
			}

			return new ActionSet(actions);
		}

		/// <summary>
		/// Creates an <see cref="IClickAction"/> that displays the specified display set when clicked.  The index
		/// parameter is used to generate a label for the action.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private IClickAction CreateDisplaySetAction(IDisplaySet displaySet, int index)
		{
			ActionPath path = new ActionPath(string.Format("imageviewer-contextmenu/display{0}", index), null);
			MenuAction action = new MenuAction(string.Format("{0}:display{1}", this.GetType().FullName, index), path, ClickActionFlags.None, null);
			action.GroupHint = new GroupHint("DisplaySets");
			action.Label = displaySet.Name;
			action.SetClickHandler(
				delegate()
				{
					AssignDisplaySetToImageBox(displaySet);
				}
			);

			action.Checked = this.ImageViewer.SelectedImageBox != null &&
				this.ImageViewer.SelectedImageBox.DisplaySet != null && 
				this.ImageViewer.SelectedImageBox.DisplaySet.Name == displaySet.Name;

			return action;
		}

		private void AssignDisplaySetToImageBox(IDisplaySet displaySet)
		{
			if (_lastActivatedImageBox == null)
				return;

			UndoableCommand command = new UndoableCommand(_lastActivatedImageBox);
			command.BeginState = _lastActivatedImageBox.CreateMemento();

			// If the display set is already visible, then we make a copy;
			// otherwise, we use the original
			if (displaySet.Visible)
				_lastActivatedImageBox.DisplaySet = displaySet.Clone();
			else
				_lastActivatedImageBox.DisplaySet = displaySet;

			_lastActivatedImageBox.Draw();
			_lastActivatedImageBox[0, 0].Select();

			command.EndState = _lastActivatedImageBox.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);
		}
	}
}
