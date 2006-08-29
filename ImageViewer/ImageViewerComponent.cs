using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using ClearCanvas.ImageViewer.StudyManagement;

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
        // study manager is shared amongst all image workspaces
        private static StudyManager _studyManager;


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
        private MouseToolMap _currentMappableModalTools = new MouseToolMap();
        private string _studyInstanceUID;
        private ToolSet _toolSet;


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

        public override void Stop()
        {
            // TODO: What would be better is if the study tree listened for workspaces
            // being addded/removed then increased/decreased the reference count itself.
            StudyManager.StudyTree.DecrementStudyReferenceCount(_studyInstanceUID);
            
            base.Stop();
        }

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
                IActionSet contextMenuActions = _toolSet.Actions.Select(
                    delegate(IAction action) { return action.Path.Site == "imageviewer-contextmenu"; });

                ActionModelRoot model = ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-contextmenu", contextMenuActions);

                // insert dynamic items into model here
                model.InsertActions(GetDisplaySetActions());

                return model.ChildNodes["imageviewer-contextmenu"];
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
        public MouseToolMap MouseToolMap
        {
            get { return _currentMappableModalTools; }
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
            MenuAction action = new MenuAction(string.Format("display{0}", index), path, this, ClickActionFlags.None);
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
