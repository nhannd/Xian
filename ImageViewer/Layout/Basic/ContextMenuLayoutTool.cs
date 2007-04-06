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
					actions.Add(
						CreateDisplaySetAction(
							this.ImageViewer.LogicalWorkspace,
							imageSet, 
							displaySet, 
							++i));
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
		private IClickAction CreateDisplaySetAction(
			ILogicalWorkspace logicalWorkspace,
			IImageSet imageSet, 
			IDisplaySet displaySet, 
			int index)
		{
			string pathString;

			if (logicalWorkspace.ImageSets.Count == 1)
			{
				pathString = string.Format("imageviewer-contextmenu/display{0}", index);
			}
			else
			{
				//string imageSetName = imageSet.Name.Replace("/", "\\");
				string imageSetName = imageSet.Name.Replace("/", "-");

				if (IsMoreThanOnePatient(logicalWorkspace.ImageSets))
					pathString = string.Format("imageviewer-contextmenu/{0}/{1}/display{2}", imageSet.PatientInfo, imageSetName, index);
				else
					pathString = string.Format("imageviewer-contextmenu/{0}/display{1}", imageSetName, index);
			}

			ActionPath path = new ActionPath(pathString, null);
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
				this.ImageViewer.SelectedImageBox.DisplaySet.Uid == displaySet.Uid;
			action.CheckParents = action.Checked;

			return action;
		}

		private bool IsMoreThanOnePatient(ImageSetCollection imageSetCollection)
		{
			string patientInfo = String.Empty;
			int numPatients = 0;

			foreach (IImageSet imageSet in imageSetCollection)
			{
				if (imageSet.PatientInfo != patientInfo)
				{
					patientInfo = imageSet.PatientInfo;
					numPatients++;
				}
			}

			return numPatients > 1;
		}

		private void AssignDisplaySetToImageBox(IDisplaySet displaySet)
		{
			UndoableCommand command = new UndoableCommand(this.ImageViewer.SelectedImageBox);
			command.BeginState = this.ImageViewer.SelectedImageBox.CreateMemento();

			// If the display set is already visible, then we make a copy;
			// otherwise, we use the original
			if (displaySet.Visible)
				this.ImageViewer.SelectedImageBox.DisplaySet = displaySet.Clone();
			else
				this.ImageViewer.SelectedImageBox.DisplaySet = displaySet;

			this.ImageViewer.SelectedImageBox.Draw();
			//this.ImageViewer.SelectedImageBox[0, 0].Select();

			command.EndState = this.ImageViewer.SelectedImageBox.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);
		}
	}
}
