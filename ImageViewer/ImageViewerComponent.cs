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
using ClearCanvas.ImageViewer.Imaging;

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

		private StudyTree _studyTree;
		private ILogicalWorkspace _logicalWorkspace;
        private IPhysicalWorkspace _physicalWorkspace;
        private EventBroker _eventBroker;
		private ViewerShortcutManager _shortcutManager;
        private ToolSet _toolSet;

		private static StudyFinderMap _studyFinders;
		private static StudyLoaderMap _studyLoaders;

		private event EventHandler _closingEvent;

		#endregion

		public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new ImageViewerToolExtensionPoint(), new ImageViewerToolContext(this));

			RegisterShortcuts();
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

		public StudyTree StudyTree
		{
			get
			{
				if (_studyTree == null)
					_studyTree = new StudyTree();

				return _studyTree;
			}
		}

		public static StudyFinderMap StudyFinders
		{
			get
			{
				if (_studyFinders == null)
					_studyFinders = new StudyFinderMap();

				return _studyFinders;
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

		#region Private/internal properties

		private StudyLoaderMap StudyLoaders
		{
			get
			{
				if (_studyLoaders == null)
					_studyLoaders = new StudyLoaderMap();

				return _studyLoaders;
			}
		}

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

		#region Public methods

		public void LoadStudy(string studyInstanceUID, string source)
		{
			IStudyLoader studyLoader = this.StudyLoaders[source];

			try
			{
				studyLoader.Start(studyInstanceUID);
			}
			catch (Exception e)
			{
				OpenStudyException ex = new OpenStudyException(SR.ExceptionLoadCompleteFailure, e);
				throw ex;
			}

			int totalImages = 0;
			int failedImages = 0;

			while (true)
			{
				ImageSop image = studyLoader.LoadNextImage();

				if (image == null)
					break;

				try
				{
					this.StudyTree.AddImage(image);
				}
				catch (Exception e)
				{
					failedImages++;
					Platform.Log(e);
				}

				totalImages++;
			}

			studyLoader.Stop();

			int successfulImages = totalImages - failedImages;

			// Only bother to tell someone if at least one image loaded
			if (successfulImages > 0)
			{
				this.EventBroker.OnStudyLoaded(
					new StudyEventArgs(this.StudyTree.GetStudy(studyInstanceUID)));
			}

			VerifyLoad(totalImages, failedImages);
		}

		public void LoadImage(string path)
		{
			LocalImageLoader loader = new LocalImageLoader(this);
			loader.Load(path);

			VerifyLoad(loader.TotalImages, loader.FailedImages);
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

				if (this.StudyTree != null)
					this.StudyTree.Dispose();
			}
		}

		#endregion 

		#region Private methods

		private void RegisterShortcuts()
		{
			(ShortcutManager as ViewerShortcutManager).RegisterKeyboardShortcuts(this.KeyboardModel.ChildNodes);
			(ShortcutManager as ViewerShortcutManager).RegisterMouseShortcuts(_toolSet.Tools);
		}

		private void VerifyLoad(int totalImages, int failedImages)
		{
			if (failedImages == totalImages)
			{
				OpenStudyException ex = new OpenStudyException(SR.ExceptionLoadCompleteFailure);
				ex.TotalImages = totalImages;
				ex.FailedImages = failedImages;
				throw ex;
			}
			else if (failedImages > 0 || totalImages == 0)
			{
				string msg = String.Format(SR.ExceptionLoadPartialFailure, failedImages, totalImages);
				OpenStudyException ex = new OpenStudyException(msg);
				ex.TotalImages = totalImages;
				ex.FailedImages = failedImages;
				throw ex;
			}
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
