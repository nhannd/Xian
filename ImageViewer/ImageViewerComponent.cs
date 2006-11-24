using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer
{
    [ExtensionPoint()]
    public class ImageViewerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint()]
    public class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
    {
    }

    [ExtensionPoint()]
    public class ImageViewerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [AssociateView(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponent : ApplicationComponent, IImageViewer
    {
        internal class ImageViewerToolContext : ToolContext, IImageViewerToolContext
        {
            private ImageViewerComponent _component;

            internal ImageViewerToolContext(ImageViewerComponent component)
            {
                _component = component;
            }

            #region IImageViewerToolContext Members

            public StudyManager StudyManager
            {
                get { return ImageViewerComponent.StudyManager; }
            }

            public IImageViewer Viewer
            {
                get { return _component; }
            }

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

            #endregion
		}

		#region Private fields

		// study manager is shared amongst all image workspaces
		private static StudyManager _studyManager;

		// annotation manager is shared amongst all image workspaces
		private static AnnotationManager _annotationManager;

		private ILogicalWorkspace _logicalWorkspace;
        private IPhysicalWorkspace _physicalWorkspace;
        private ILayoutManager _layoutManager;
        private EventBroker _eventBroker;
		private ViewerShortcutManager _shortcutManager;
        private string _studyInstanceUID;
        private ToolSet _toolSet;
		private event EventHandler<ContextMenuEventArgs> _contextMenuBuildingEvent;

		private event EventHandler _closingEvent;

		#endregion

		public ImageViewerComponent(string studyInstanceUID)
        {
            Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

            _studyInstanceUID = studyInstanceUID;
            _logicalWorkspace = new LogicalWorkspace(this);
            _physicalWorkspace = new PhysicalWorkspace(this);
            _eventBroker = new EventBroker();
        }
        
        public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new ImageViewerToolExtensionPoint(), new ImageViewerToolContext(this));

			RegisterShortcuts();

			CreateLayoutManager();
            ApplyLayout();

			_physicalWorkspace.SelectDefaultImageBox();
        }

        public override void Stop()
        {
            // TODO: What would be better is if the study tree listened for workspaces
            // being addded/removed then increased/decreased the reference count itself.
            StudyManager.StudyTree.DecrementStudyReferenceCount(_studyInstanceUID);

			EventsHelper.Fire(_closingEvent, this, EventArgs.Empty);

            base.Stop();
		}

		#region Public Properties

		public override IActionSet ExportedActions
        {
            get
            {
                // we should technically only export the actions that target the global menus
                // or toolbars, but it doesn't matter - the desktop will sort it out
                return _toolSet.Actions;
            }
        }

        public ActionModelNode ContextMenuModel
        {
            get
            {
				ActionModelRoot model = ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-contextmenu", _toolSet.Actions);

				EventsHelper.Fire(_contextMenuBuildingEvent, this, new ContextMenuEventArgs(model));

				return model;
            }
        }

		public ActionModelNode KeyboardModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-keyboard", _toolSet.Actions);
			}
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

        /// <summary>
        /// Gets the command history for this image viewer.
        /// </summary>
        public CommandHistory CommandHistory
        {
            get { return this.Host.CommandHistory; }
        }

        /// <summary>
        /// Gets the <see cref="PhysicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="PhysicalWorkspace"/>.</value>
        public IPhysicalWorkspace PhysicalWorkspace
        {
            get { return _physicalWorkspace; }
        }

        /// <summary>
        /// Gets the <see cref="LogicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="LogicalWorkspace"/>.</value>
        public ILogicalWorkspace LogicalWorkspace
        {
            get { return _logicalWorkspace; }
        }

        public EventBroker EventBroker
        {
            get { return _eventBroker; }
        }

        /// <summary>
        /// Gets the currently selected <see cref="ImageBox"/>
        /// </summary>
        /// <value>The currently selected <see cref="ImageBox"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/> or if the
        /// currently active <see cref="Workspace"/> is not an <see cref="ImageWorkspace"/>.</value>
        public IImageBox SelectedImageBox
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
        public ITile SelectedTile
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
        public IPresentationImage SelectedPresentationImage
        {
            get
            {
                if (this.SelectedTile == null)
                    return null;
                else
                    return this.SelectedTile.PresentationImage;
            }
        }

		public ViewerShortcutManager ShortcutManager
        {
            get 
			{
				if (_shortcutManager == null)
					_shortcutManager = new ViewerShortcutManager();

				return _shortcutManager; 
			}
        }

 		public static AnnotationManager AnnotationManager
		{
			get
			{
				if (_annotationManager == null)
					_annotationManager = new AnnotationManager();

				return _annotationManager;
			}
		}

		#endregion

		#region Events

		public event EventHandler Closing
		{
			add { _closingEvent += value; }
			remove { _closingEvent -= value; }
		}

		public event EventHandler<ContextMenuEventArgs> ContextMenuBuilding
		{
			add { _contextMenuBuildingEvent += value; }
			remove { _contextMenuBuildingEvent -= value; }
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_physicalWorkspace != null)
					_physicalWorkspace.Dispose();

				if (_logicalWorkspace != null)
					_logicalWorkspace.Dispose();

				if (_layoutManager != null)
					_layoutManager.Dispose();

				if (_toolSet != null)
					_toolSet.Dispose();
			}
		}

		#endregion 

		#region Private methods

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



		private void RegisterShortcuts()
		{
			this.ShortcutManager.RegisterKeyboardShortcuts(this.KeyboardModel.ChildNodes);
			
			foreach (ITool tool in _toolSet.Tools)
			{
				this.ShortcutManager.RegisterMouseShortcuts(tool);
			}
		}

		#endregion
	}
}
