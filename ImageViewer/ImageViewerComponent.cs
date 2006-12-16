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
    public class ImageViewerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

	public abstract class ImageViewerComponent : ApplicationComponent, IImageViewer, IContextMenuProvider
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
        private EventBroker _eventBroker;
		private ViewerShortcutManager _shortcutManager;
        private ToolSet _toolSet;

		private event EventHandler _closingEvent;

		#endregion

		public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new ImageViewerToolExtensionPoint(), new ImageViewerToolContext(this));

			RegisterShortcuts();

			this.PhysicalWorkspace.SelectDefaultImageBox();
        }

        public override void Stop()
        {
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
            get
			{
				if (_physicalWorkspace == null)
					_physicalWorkspace = new PhysicalWorkspace(this);

				return _physicalWorkspace; 
			}
        }

        /// <summary>
        /// Gets the <see cref="LogicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="LogicalWorkspace"/>.</value>
        public ILogicalWorkspace LogicalWorkspace
        {
            get 
			{
				if (_logicalWorkspace == null)
					_logicalWorkspace = new LogicalWorkspace(this);

				return _logicalWorkspace; 
			}
        }

        public EventBroker EventBroker
        {
            get
			{
				if (_eventBroker == null)
					_eventBroker = new EventBroker();

				return _eventBroker; 
			}
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

		public IViewerShortcutManager ShortcutManager
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

		#endregion

		#region Protected properties

		protected ToolSet ToolSet
		{
			get { return _toolSet; }
		}

		#endregion


		#region Private properties

		private ActionModelNode ContextMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-contextmenu", _toolSet.Actions);
			}
		}

		private ActionModelNode KeyboardModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-keyboard", _toolSet.Actions);
			}
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
				if (this.PhysicalWorkspace != null)
					this.PhysicalWorkspace.Dispose();

				if (this.LogicalWorkspace != null)
					this.LogicalWorkspace.Dispose();

				if (this.ToolSet != null)
					this.ToolSet.Dispose();
			}
		}

		#endregion 

		#region Private methods

		private void RegisterShortcuts()
		{
			(ShortcutManager as ViewerShortcutManager).RegisterKeyboardShortcuts(this.KeyboardModel.ChildNodes);
			(ShortcutManager as ViewerShortcutManager).RegisterMouseShortcuts(_toolSet.Tools);
		}

		#endregion

		#region IContextMenuProvider Members

		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			return this.ContextMenuModel;
		}

		#endregion
	}
}
