using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout.Basic
{

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ContextMenuLayoutTool : Tool<IImageViewerToolContext>
	{
        /// <summary>
        /// Constructor
        /// </summary>
		public ContextMenuLayoutTool()
		{
        }

        /// <summary>
        /// Overridden to subscribe to workspace activation events
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

			this.Context.Viewer.ContextMenuBuilding += new EventHandler<ContextMenuEventArgs>(OnImageViewerContextMenuBuilding);
        }

		void OnImageViewerContextMenuBuilding(object sender, ContextMenuEventArgs e)
		{
			e.ContextMenuModel.InsertActions(GetDisplaySetActions());
		}

		private IImageViewer ImageViewer
		{
			get { return this.Context.Viewer; }
		}

		/// <summary>
		/// Gets an array of <see cref="IAction"/> objects that allow selection of specific display
		/// sets for display in the currently selected image box.
		/// </summary>
		/// <returns></returns>
		private IAction[] GetDisplaySetActions()
		{

			List<IAction> actions = new List<IAction>();
			int i = 0;
			foreach (DisplaySet displaySet in this.ImageViewer.LogicalWorkspace.DisplaySets)
			{
				actions.Add(CreateDisplaySetAction(displaySet, ++i));
			}
			return actions.ToArray();
		}

		/// <summary>
		/// Creates an <see cref="IClickAction"/> that displays the specified display set when clicked.  The index
		/// parameter is used to generate a label for the action.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private IClickAction CreateDisplaySetAction(DisplaySet displaySet, int index)
		{
			ActionPath path = new ActionPath(string.Format("imageviewer-contextmenu/{0}", displaySet.Name), null);
			MenuAction action = new MenuAction(string.Format("display{0}", index), path, ClickActionFlags.None, null);
			action.Label = displaySet.Name;
			action.SetClickHandler(
				delegate()
				{
					this.ImageViewer.SelectedImageBox.DisplaySet = displaySet;
					this.ImageViewer.SelectedImageBox.Draw();
				}
			);
			return action;
		}
	}
}
