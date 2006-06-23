using System;
using System.Collections.Generic;
using System.Text;

using Gtk;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Workstation.Model.Actions;

namespace ClearCanvas.Workstation.View.GTK
{
    public class MainWindow : Window
    {
        private MenuBar _mainMenu;
        private Toolbar _toolBar;
		private Tooltips _tooltips;
        private HBox _toolBarBox;
        private Notebook _notebook;
		private VBox _outerBox;
		
		private ToolViewManager _workbenchToolViewManager;
		private Dictionary<Workspace, ToolViewManager> _workspaceToolViewManagers;
		private Workspace _lastActiveWorkspace;
		
		
		private int openedStudyCount = 0;

        public MainWindow()
            : base("ClearCanvas")
        {
             this.SetDefaultSize(Screen.Width, Screen.Height);
			
			_workspaceToolViewManagers = new Dictionary<Workspace, ToolViewManager>();
			
            _mainMenu = new MenuBar();
            _toolBar = new Toolbar();

            _notebook = new Notebook();
			_notebook.TabPos = PositionType.Top;
			_notebook.SwitchPage += OnNotebookSwitchPage;
			
            // this box holds the main menu
            HBox menuBox = new HBox(false, 0);
            menuBox.PackStart(_mainMenu, true, true, 0);

            // this box holds the toolbar
            _toolBarBox = new HBox(false, 0);
            _toolBarBox.PackStart(_toolBar, true, true, 0);

            // this box holds the overall layout
            _outerBox = new VBox(false, 0);
            _outerBox.PackStart(menuBox, false, false, 0);
            _outerBox.PackStart(_toolBarBox, false, false, 0);
            _outerBox.PackStart(_notebook, true, true, 0);

 			WorkstationModel.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
            WorkstationModel.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
            WorkstationModel.WorkspaceManager.WorkspaceActivated += new EventHandler<WorkspaceEventArgs>(OnWorkspaceActivated);
			
			this.Add(_outerBox);
            this.ShowAll();

            BuildMenusAndToolBars(null);
			UpdateToolViews(null);
        }

        private void BuildMenusAndToolBars(Workspace workspace)
        {
            BuildMenus(workspace);
            BuildToolbars(workspace);
        }
		

        private void BuildToolbars(Workspace workspace)
        {
            _toolBarBox.Remove(_toolBar);
			_toolBar.Destroy();		// make sure the old one is cleaned up!
            _toolBar = new Toolbar();
			_toolBar.ToolbarStyle = ToolbarStyle.Icons;
            _toolBarBox.PackStart(_toolBar, true, true, 0);
			_tooltips = new Tooltips();
			
			ActionModelRoot model = new ActionModelRoot("");
			model.Merge(WorkstationModel.ToolManager.ToolbarModel);
			if(workspace != null) {
				model.Merge(workspace.ToolManager.ToolbarModel);
			}
            GtkToolbarBuilder.BuildToolbar(_toolBar, _tooltips, model);
			
            _toolBar.ShowAll();
        }

        private void BuildMenus(Workspace workspace)
        {
            foreach(Widget w in _mainMenu) {
                _mainMenu.Remove(w);
				w.Destroy();
            }
			
			ActionModelRoot model = new ActionModelRoot("");
			model.Merge(WorkstationModel.ToolManager.MenuModel);
			if(workspace != null) {
				model.Merge(workspace.ToolManager.MenuModel);
			}

            GtkMenuBuilder.BuildMenu(_mainMenu, model);
            _mainMenu.ShowAll();
        }
		
		private void UpdateToolViews(Workspace workspace)
		{
			if(_workbenchToolViewManager == null)
			{
				_workbenchToolViewManager = new ToolViewManager(WorkstationModel.ToolManager, this);
				_workbenchToolViewManager.Activate(true);	// always active
			}
			
			if(_lastActiveWorkspace != null && _workspaceToolViewManagers.ContainsKey(_lastActiveWorkspace))
			{
				_workspaceToolViewManagers[_lastActiveWorkspace].Activate(false);
			}
			
			if(workspace != null)
			{
				if(!_workspaceToolViewManagers.ContainsKey(workspace))
				{
					_workspaceToolViewManagers.Add(workspace, new ToolViewManager(workspace.ToolManager, this));
				}
				
				_workspaceToolViewManagers[workspace].Activate(true);
			}
			
			_lastActiveWorkspace = workspace;
		}
		
		private void OnNotebookSwitchPage(object sender, SwitchPageArgs args)
		{
			WorkspaceDrawingArea wda = (WorkspaceDrawingArea)_notebook.GetNthPage((int)args.PageNum);
			wda.Active = true;
		}
		
 		private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
        {
            try
            {
				AddWorkspace(e.Workspace);
            }
            catch (Exception ex)
            {
                Platform.Log(ex);
            }
        }
		
        // This is the event handler for when a workspace is removed from the
        // WorkspaceManager.  Not to be confused with OnWorkspaceClosed.
        private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
        {
            try
            {
				RemoveWorkspace(e.Workspace);
            }
            catch (Exception ex)
            {
                Platform.Log(ex);
            }
        }
		
		private void OnWorkspaceActivated(object sender, WorkspaceEventArgs e)
		{
            try
            {
 	            BuildMenusAndToolBars(e.Workspace);
				UpdateToolViews(e.Workspace);
			}
            catch (Exception ex)
            {
                Platform.Log(ex);
            }
		}
		
		public Workspace ActiveWorkspace
		{
			get
			{
				WorkspaceDrawingArea wda = (WorkspaceDrawingArea)_notebook.CurrentPageWidget;
				return wda == null ? null : wda.Workspace;
			}
		}

        public void AddWorkspace(Workspace workspace)
        {
			WorkspaceDrawingArea wda = new WorkspaceDrawingArea(workspace);
			Label label = new Label(string.Format("Study {0}", ++openedStudyCount));
			_notebook.AppendPage(wda, label);
			wda.Show();
 
			_notebook.CurrentPage = _notebook.NPages-1;
		}

        public void RemoveWorkspace(Workspace workspace)
        {
           // Find the form that owns the workspace to be removed and close it
            for (int i = 0; i < _notebook.NPages; i++)
            {
                WorkspaceDrawingArea wda = (WorkspaceDrawingArea)_notebook.GetNthPage(i);
                if (wda.Workspace == workspace)
                {
                    _notebook.RemovePage(i);
                    wda.Destroy();
                    break;
                }
            }
			
			if(_workspaceToolViewManagers.ContainsKey(workspace))
			{
				_workspaceToolViewManagers[workspace].Activate(false);
				_workspaceToolViewManagers.Remove(workspace);
			}
 			
            if (_notebook.NPages == 0)
            {
                BuildMenusAndToolBars(null);
				UpdateToolViews(null);
            }
		}
     }
}
