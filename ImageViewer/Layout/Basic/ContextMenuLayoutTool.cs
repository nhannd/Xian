using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{

    /// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ContextMenuLayoutTool : Tool<IImageViewerToolContext>
	{
		public class BooleanPropertyBinding : IObservablePropertyBinding<bool>
		{
			private bool _property;
			private event EventHandler _propertyChangedEvent;

			#region IObservablePropertyBinding<bool> Members

			public event EventHandler PropertyChanged
			{
				add { _propertyChangedEvent += value; }
				remove { _propertyChangedEvent -= value; }
			}
        
			public bool PropertyValue
			{
				get { return _property; }
				set
				{
					_property = value;
					EventsHelper.Fire(_propertyChangedEvent, this, EventArgs.Empty);
				}
			}

			#endregion
		}

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

			foreach (IImageSet imageSet in this.ImageViewer.LogicalWorkspace.ImageSets)
			{
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
				{
					actions.Add(CreateDisplaySetAction(displaySet, ++i));
				}
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
		private IClickAction CreateDisplaySetAction(IDisplaySet displaySet, int index)
		{
			ActionPath path = new ActionPath(string.Format("imageviewer-contextmenu/{0}", displaySet.Name), null);
			MenuAction action = new MenuAction(string.Format("display{0}", index), path, ClickActionFlags.None, null);
			action.SetDefaultLabel(displaySet.Name);
			action.SetClickHandler(
				delegate()
				{
					AssignDisplaySetToImageBox(displaySet);
				}
			);

			BooleanPropertyBinding binding = new BooleanPropertyBinding();
			action.SetCheckedObservable(binding);

			if (this.ImageViewer.SelectedImageBox.DisplaySet.Name == displaySet.Name)
				binding.PropertyValue = true;
			else
				binding.PropertyValue = false;

			return action;
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
			this.ImageViewer.SelectedImageBox[0, 0].Select();

			command.EndState = this.ImageViewer.SelectedImageBox.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);
		}
	}
}
