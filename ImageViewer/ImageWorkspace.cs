using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Application.Tools;
using ClearCanvas.Workstation.Model.StudyManagement;
using ClearCanvas.Common.Application;


namespace ClearCanvas.Workstation.Model
{
    [ExtensionPoint()]
	public class ImageWorkspaceViewExtensionPoint : ExtensionPoint<IWorkspaceView>
	{
	}

	[ExtensionPoint()]
	public class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
	{
	}

    /// <summary>
    /// Defines an extension point for tools that are applicable to an <see cref="ImageWorkspace"/>
    /// </summary>
    [ExtensionPoint()]
    public class ImageWorkspaceToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class ImageWorkspaceToolContext : ToolContext
    {
        private ImageWorkspace _workspace;

        public ImageWorkspaceToolContext(ImageWorkspace workspace)
            : base(new ImageWorkspaceToolExtensionPoint())
        {
            _workspace = workspace;
        }

        public ImageWorkspace Workspace
        {
            get { return _workspace; }
        }
    }
	
	public class ImageWorkspace : Workspace
	{
        // study manager is shared amongst all image workspaces
        private static StudyManager _studyManager;
        
        private LogicalWorkspace _logicalWorkspace;
		private PhysicalWorkspace _physicalWorkspace;
		private ILayoutManager _layoutManager;
		private EventBroker _eventBroker;
		private MouseToolMap _currentMappableModalTools = new MouseToolMap();
		private string _studyInstanceUID;
		private IWorkspaceView _view;

		public ImageWorkspace(string studyInstanceUID)
		{
			Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

			_studyInstanceUID = studyInstanceUID;
			_logicalWorkspace = new LogicalWorkspace(this);
			_physicalWorkspace = new PhysicalWorkspace(this);
			_eventBroker = new EventBroker();
			CreateLayoutManager();
			ApplyLayout();
		}

        /// <summary>
        /// Gets the <see cref="StudyManager"/>
        /// </summary>
        public static StudyManager StudyManager
        {
            get
            {
                if (_studyManager == null)
                    _studyManager = new StudyManager();

                return _studyManager;
            }
        }


		public override bool IsActivated
		{
			set
			{
				base.IsActivated = value;
				
				// Select top left tile by default
				if (_physicalWorkspace.SelectedImageBox == null)
				{
					Tile topLeftTile = _physicalWorkspace.ImageBoxes[0].Tiles[0];

					if (topLeftTile != null)
						topLeftTile.Selected = true;
				}
			}
		}


		/// <summary>
		/// Gets the <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <value>The <see cref="PhysicalWorkspace"/>.</value>
		public PhysicalWorkspace PhysicalWorkspace
		{
			get { return _physicalWorkspace; }
		}

		/// <summary>
		/// Gets the <see cref="LogicalWorkspace"/>.
		/// </summary>
		/// <value>The <see cref="LogicalWorkspace"/>.</value>
		public LogicalWorkspace LogicalWorkspace
		{
			get { return _logicalWorkspace; }
		}

		public EventBroker EventBroker
		{
			get { return _eventBroker; }
		}

		public override IWorkspaceView View
		{
			get 
			{
				if (_view == null)
				{
					_view = (IWorkspaceView)ViewFactory.CreateView(new ImageWorkspaceViewExtensionPoint());
					_view.SetWorkspace(this);
				}

				return _view;
			}
		}

        /// <summary>
        /// Gets the currently selected <see cref="ImageBox"/>
        /// </summary>
        /// <value>The currently selected <see cref="ImageBox"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        public ImageBox SelectedImageBox
        {
            get
            {
                if (this.PhysicalWorkspace == null)
                    return null;
                else
                    return this.PhysicalWorkspace.SelectedImageBox;
            }
        }

        /// <summary>
        /// Gets the currently selected <see cref="Tile"/>
        /// </summary>
        /// <value>The currently selected <see cref="Tile"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        public Tile SelectedTile
        {
            get
            {
                if (this.SelectedImageBox == null)
                    return null;
                else
                    return this.SelectedImageBox.SelectedTile;
            }
        }

        /// <summary>
        /// Gets the currently selected <see cref="PresentationImage"/>
        /// </summary>
        /// <value>The currently selected <see cref="PresentationImage"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        public PresentationImage SelectedPresentationImage
        {
            get
            {
                if (this.SelectedTile == null)
                    return null;
                else
                    return this.SelectedTile.PresentationImage;
            }
        }

		/// <summary>
		/// Gets the workspace's currently selected mappable modal tools.
		/// </summary>
		/// <value>The workspace's current selected mappable modal tools.</value>
		/// <remarks>
		/// A <i>Mappable modal tool</i> or <i>MMT</i> is a tool that when selected
		/// causes a mouse button to be mapped to the tool's function; an MMT that
		/// is already mapped to the same button becomes deselected.  Examples
		/// of MMTs in ClearCanvas include Window/Level, Stack, Zoom, Pan, etc.  This
		/// property gets an index that stores which mouse buttons are currently
		/// mapped to which MMT.
		/// </remarks>
		internal MouseToolMap MouseToolMap
		{
			get { return _currentMappableModalTools; }
		}

        public override void Cleanup()
		{
			// TODO: What would be better is if the study tree listened for workspaces
			// being addded/removed then increased/decreased the reference count itself.
			StudyManager.StudyTree.DecrementStudyReferenceCount(_studyInstanceUID);
		}

        protected override ToolContext CreateToolContext()
        {
            return new ImageWorkspaceToolContext(this);
        }


		private void CreateLayoutManager()
		{
			try
			{
				LayoutManagerExtensionPoint xp = new LayoutManagerExtensionPoint();
				_layoutManager = (ILayoutManager)xp.CreateExtension();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(e, LogLevel.Warn);
			}
		}

		/// <summary>
		/// Applies a layout to the workspace.
		/// </summary>
		/// <remarks>
		/// This method signature is preliminary and will likely change.
		/// </remarks>
		private void ApplyLayout()
		{
			if (_layoutManager == null)
				throw new NotSupportedException(SR.ExceptionLayoutManagerDoesNotExist);

			_layoutManager.ApplyLayout(_logicalWorkspace, _physicalWorkspace, _studyInstanceUID);
			StudyManager.StudyTree.IncrementStudyReferenceCount(_studyInstanceUID);
		}
	}
}
