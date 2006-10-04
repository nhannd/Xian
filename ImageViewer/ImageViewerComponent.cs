using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Annotations;

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

	/// <summary>
	/// An application component capable of displaying images.
	/// </summary>
    [AssociateView(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponent : ApplicationComponent, IImageViewer
    {
        // study manager is shared amongst all image workspaces
        private static StudyManager _studyManager;
		
		// annotation manager is shared amongst all image workspaces
		private static AnnotationManager _annotationManager;
		
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

            #endregion
        }

        
        private LogicalWorkspace _logicalWorkspace;
        private PhysicalWorkspace _physicalWorkspace;
        private ILayoutManager _layoutManager;
        private EventBroker _eventBroker;
        private MouseButtonToolMap _mouseButtonToolMap;
		private MouseWheelToolMap _mouseWheelToolMap;
        private string _studyInstanceUID;
        private ToolSet _toolSet;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageViewerComponent"/> class.
		/// </summary>
		public ImageViewerComponent(string studyInstanceUID)
        {
            Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

            _studyInstanceUID = studyInstanceUID;
            _logicalWorkspace = new LogicalWorkspace(this);
            _physicalWorkspace = new PhysicalWorkspace(this);
            _eventBroker = new EventBroker();
        }
        
		/// <summary>
		/// Starts this image viewer.
		/// </summary>
        public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new ImageViewerToolExtensionPoint(), new ImageViewerToolContext(this));

            CreateLayoutManager();
            ApplyLayout();

            // Select top left tile by default
            if (this.PhysicalWorkspace.SelectedImageBox == null)
            {
                Tile topLeftTile = this.PhysicalWorkspace.ImageBoxes[0].Tiles[0];

                if (topLeftTile != null)
                    topLeftTile.Selected = true;
            }
        }

		/// <summary>
		/// Stops this image viewer
		/// </summary>
        public override void Stop()
        {
            // TODO: What would be better is if the study tree listened for workspaces
            // being addded/removed then increased/decreased the reference count itself.
            StudyManager.StudyTree.DecrementStudyReferenceCount(_studyInstanceUID);
            
            base.Stop();
        }

		/// <summary>
		/// Gets this image viewer's action set
		/// </summary>
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
		/// Gets this image viewer's context menu action model.
		/// </summary>
        public ActionModelNode ContextMenuModel
        {
            get
            {
				if (!this.PhysicalWorkspace.ContextMenuEnabled)
					return null;

				ActionModelRoot model = ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-contextmenu", _toolSet.Actions);

				// insert dynamic items into model here

				// NY: Disable these for now, since we haven't quite decided yet
				// how we're going to manage display sets on the context menu.
				//model.InsertActions(GetDisplaySetActions());

				return model;
            }
        }

        /// <summary>
        /// Gets this image viewer's <see cref="StudyManager"/>
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
        /// Gets this image viewer's command history
        /// </summary>
        public CommandHistory CommandHistory
        {
            get { return this.Host.CommandHistory; }
        }

        /// <summary>
        /// Gets this image viewer's <see cref="PhysicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="PhysicalWorkspace"/>.</value>
        public PhysicalWorkspace PhysicalWorkspace
        {
            get { return _physicalWorkspace; }
        }

        /// <summary>
		/// Gets this image viewer's <see cref="LogicalWorkspace"/>.
        /// </summary>
        /// <value>The <see cref="LogicalWorkspace"/>.</value>
        public LogicalWorkspace LogicalWorkspace
        {
            get { return _logicalWorkspace; }
        }

		/// <summary>
		/// Gets this image viewer's <see cref="EventBroker"/>
		/// </summary>
        public EventBroker EventBroker
        {
            get { return _eventBroker; }
        }

		/// <summary>
		/// Gets this image viewer's currently selected <see cref="ImageBox"/>
		/// </summary>
		/// <value>The currently selected <see cref="ImageBox"/>, or <b>null</b> if 
		/// no <see cref="ImageBox"/> is currently selected.</value>
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
		/// Gets this image viewer's currently selected <see cref="Tile"/>
		/// </summary>
		/// <value>The currently selected <see cref="Tile"/>, or <b>null</b> if 
		/// no <see cref="Tile"/> is currently selected.</value>
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
		/// Gets this image viewer's currently selected <see cref="PresentationImage"/>
		/// </summary>
		/// <value>The currently selected <see cref="PresentationImage"/>, or <b>null</b> if 
		/// no <see cref="PresentationImage"/> is no currently selected.</value>
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
		/// Gets this image viewer's <see cref="MouseButtonToolMap"/>
		/// </summary>
		/// <value>The <see cref="MouseButtonToolMap"/></value>
		/// <remarks>
		/// A <i>Mouse tool</i> is a tool that when selected
		/// causes a mouse button to be mapped to the tool's function; a mouse tool that
		/// is already mapped to the same button becomes deselected.  Examples
		/// of mouse tools in ClearCanvas include Window/Level, Stack, Zoom, Pan, etc.
		/// </remarks>
		public MouseButtonToolMap MouseButtonToolMap
        {
            get 
			{
				if (_mouseButtonToolMap == null)
					_mouseButtonToolMap = new MouseButtonToolMap();

				return _mouseButtonToolMap; 
			}
        }

		//public MouseWheelToolMap MouseWheelToolMap
		//{
		//    get 
		//    {
		//        if (_mouseWheelToolMap == null)
		//            _mouseWheelToolMap = new MouseWheelToolMap();

		//        return _mouseWheelToolMap; 
		//    }
		//}

		/// <summary>
		/// Gets this image viewer's <see cref="AnnotationManager"/>
		/// </summary>
		public static AnnotationManager AnnotationManager
		{
			get
			{
				if (_annotationManager == null)
					_annotationManager = new AnnotationManager();

				return _annotationManager;
			}
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

        /// <summary>
        /// Gets an array of <see cref="IAction"/> objects that allow selection of specific display
        /// sets for display in the currently selected image box.
        /// </summary>
        /// <returns></returns>
        private IAction[] GetDisplaySetActions()
        {
            List<IAction> actions = new List<IAction>();
            int i = 0;
            foreach(DisplaySet displaySet in this.LogicalWorkspace.DisplaySets)
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
                    this.SelectedImageBox.DisplaySet = displaySet;
                    this.SelectedImageBox.Draw(true);
                }
            );
            return action;
        }

    }
}
