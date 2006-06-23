using System;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ExtensionPoint()]
    public class LayoutToolViewExtensionPoint : ExtensionPoint<IToolView>
    {
    }

    [MenuAction("show", "MenuLayout/MenuLayoutLayoutManager", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("show", "ToolbarStandard/MenuLayoutLayoutManager", Flags = ClickActionFlags.CheckAction)]
    [ClickHandler("show", "ShowHide")]
    [CheckedStateObserver("show", "IsViewActive", "ViewActivationChanged")]
    [IconSet("show", IconScheme.Colour, "", "Icons.LayoutMedium.png", "Icons.LayoutLarge.png")]
    [Tooltip("show", "MenuLayoutLayoutManager")]

    [ToolView(typeof(LayoutToolViewExtensionPoint), "Layout Manager", ToolViewDisplayHint.DockLeft | ToolViewDisplayHint.DockAutoHide, "IsViewActive", "ViewActivationChanged")]
    
    /// <summary>
	/// Summary description for LayoutCentre.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
	public class LayoutTool : Tool
	{
        // by making these members static we cause the view activation state to be shared across all
        // instances of LayoutTool (hence across all workspaces)
        private static bool _showView;
        private static event EventHandler _viewActivationChangedEvent;

        private event EventHandler _layoutChanged;
        
        public LayoutTool()
		{
        }

		private ImageWorkspace Workspace
		{
			get { return (this.Context as ImageWorkspaceToolContext).Workspace; }
		}
		
		public bool IsViewActive
		{
			get { return _showView; }
			set
			{
				if (_showView != value)
				{
					_showView = value;
					EventsHelper.Fire(_viewActivationChangedEvent, this, new EventArgs());
				}
			}
		}

		public int ImageBoxRows
		{
			get { return this.Workspace.PhysicalWorkspace.Rows; }
		}

		public int ImageBoxColumns
		{
			get { return this.Workspace.PhysicalWorkspace.Columns; }
		}

		public int TileRows
		{
			get { return this.Workspace.PhysicalWorkspace.SelectedImageBox.Rows; }
		}

		public int TileColumns
		{
			get { return this.Workspace.PhysicalWorkspace.SelectedImageBox.Columns; }
		}

		public event EventHandler ViewActivationChanged
		{
			add { _viewActivationChangedEvent += value; }
			remove { _viewActivationChangedEvent -= value; }
		}

		public event EventHandler LayoutChanged
		{
			add { _layoutChanged += value; }
			remove { _layoutChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Workspace.EventBroker.ImageBoxSelected += new EventHandler<ImageBoxSelectedEventArgs>(OnImageBoxSelected);
		}

		public void ShowHide()
		{
            this.IsViewActive = !this.IsViewActive;
		}

        public void LayoutImageBoxes(int imageBoxRows, int imageBoxCols, int tileRows, int tileCols)
        {
			ImageWorkspace imageWorkspace = this.Workspace as ImageWorkspace;

			if (imageWorkspace == null)
				return;

            PhysicalWorkspace physicalWorkspace = imageWorkspace.PhysicalWorkspace;
            ImageBoxLayoutCommand command = new ImageBoxLayoutCommand(physicalWorkspace);
			command.Name = SR.CommandLayoutImageBoxes;
            command.BeginState = physicalWorkspace.CreateMemento();

            physicalWorkspace.SetImageBoxGrid(imageBoxRows, imageBoxCols);

            foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
                imageBox.SetTileGrid(tileRows, tileCols);

            LayoutManager.FillPhysicalWorkspace(physicalWorkspace, physicalWorkspace.LogicalWorkspace);
            physicalWorkspace.Draw(false);

            command.EndState = physicalWorkspace.CreateMemento();

            this.Workspace.CommandHistory.AddCommand(command);
        }

        public void LayoutTiles(int tileRows, int tileCols)
        {
			ImageWorkspace imageWorkspace = this.Workspace as ImageWorkspace;

			if (imageWorkspace == null)
				return;

			ImageBox imageBox = imageWorkspace.PhysicalWorkspace.SelectedImageBox;
            TileLayoutCommand command = new TileLayoutCommand(imageBox);
			command.Name = SR.CommandLayoutTiles;
            command.BeginState = imageBox.CreateMemento();

            int index = imageBox.TopLeftPresentationImageIndex;

            imageBox.SetTileGrid(tileRows, tileCols);
            imageBox.TopLeftPresentationImageIndex = index;
            imageBox.Draw(false);

            command.EndState = imageBox.CreateMemento();

            this.Workspace.CommandHistory.AddCommand(command);
        }

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			EventsHelper.Fire(_layoutChanged, this, new EventArgs());
		}
	}
}
