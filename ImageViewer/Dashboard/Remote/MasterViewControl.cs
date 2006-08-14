namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;
    using System.Threading;
    using ClearCanvas.Common;
    using ClearCanvas.Dicom.Network;
    using ClearCanvas.Dicom;

	public partial class MasterViewControl : UserControl
	{
		public MasterViewControl()
		{
			InitializeComponent();
            Initialize();
		}

        /// <summary>
        /// We have to use an initialize method, because the deserialization requires
        /// that the assembly that contains the object's meta to be deserialized must
        /// already be loaded into memory.
        /// </summary>
        public void Initialize()
        {
            // initialize the database of a pool of servers
            _serverPool = new LocalServerDatabase();

            // add event handler to the node's contex menu event
            // since the context menu's event  should not have
            // more than event handler instance in the 
            // invocation list. Basically, set up event handlers
            // only once

            // event handler for when a server node context menu is activated
            (GetNodeContextMenu()).ItemClicked += new ToolStripItemClickedEventHandler(nodeContextMenu_ItemClicked);

            // event handler for when the top node is activated
            (GetTreeContextMenu()).ItemClicked += new ToolStripItemClickedEventHandler(nodeContextMenu_ItemClicked);

            // event handler for when any click occurs on the treeview, to catch the 
            // right-click that would normally open up the context menu
            _serverTree.Click += new EventHandler(ServerTree_Click);

            InitializeTreeView();
        }

        public Server SelectedServer
        {
            get { return (_serverTree.SelectedNode.Tag as Server); }
        }

        /// <summary>
        /// Save the node at which the last mouse click occurred on the tree
        /// If it was a node, then we can determine correctly which node 
        /// was clicked on to, say, bring up the context menu. _lastClickedNode
        /// should be checked against null, because the click may not have 
        /// corresponded with any node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerTree_Click(object sender, EventArgs e)
        {
            _lastClickedNode = _serverTree.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
        }

        private void ResetTreeView()
        {
            if (null == _serverTree.TopNode)
                return;

            _serverTree.TopNode.Nodes.Clear();
        }

        private void InitializeTreeView()
        {
            // Create the root node in the tree view
            if (null == _serverTree.TopNode)
                _serverTree.Nodes.Add(new TreeNode("Servers"));

            // make sure we can show tool tips
            _serverTree.ShowNodeToolTips = true;

            // attach the context menu
            _serverTree.ContextMenuStrip = GetTreeContextMenu();

            
            // process the list of servers
            _serverList = _serverPool.ServerList;
            foreach (Server ae in _serverList)
            {
                TreeNode node = new TreeNode(ae.Name);

                // set the tooltip text so that the details for the server
                // is accessible to the user
                StringBuilder aeLongText = new StringBuilder();
                aeLongText.AppendFormat("{0}\r\nAE Title: {1}\r\nHost: {2}\r\nListening Port: {3}",
                    ae.Description,
                    ae.AE,
                    ae.Host,
                    ae.Port);

                node.ToolTipText = aeLongText.ToString();
                node.ContextMenuStrip = GetNodeContextMenu();

                // connect the node to the Server object
                node.Tag = ae;

                _serverTree.Nodes[0].Nodes.Add(node);
            }
        }

        #region Handler for event of context menu being activated on a TreeView node
        void ModifyServer()
        {
            Server storedAE = _lastClickedNode.Tag as Server;
            ServerModifyForm form = new ServerModifyForm();

            form.Initialize(MenuItemFunction.Modify, _serverTree, _lastClickedNode);
            DialogResult result = form.ShowDialog();

            if (DialogResult.OK == result)
            {
                // save changes
                // first build new list of ServerNodes
                List<Server> list = new List<Server>();
                foreach (TreeNode iterationNode in _serverTree.TopNode.Nodes)
                {
                    if (iterationNode != _lastClickedNode)
                    {
                        Server ae = iterationNode.Tag as Server;
                        list.Add(new Server(ae.Name,
                            ae.Description,
                            ae.Host,
                            ae.AE,
                            ae.ListeningPort));
                    }
                    else
                    {
                        list.Add(form.ModifiedServer);
                    }
                }

                _serverPool.SaveNewDatabase(list);
                ResetTreeView();
                InitializeTreeView();
            }
        }

        void RemoveServer()
        {
            Server storedAE = _lastClickedNode.Tag as Server;
            ServerModifyForm form = new ServerModifyForm();

            form.Initialize(MenuItemFunction.Remove, _serverTree, _lastClickedNode);
            DialogResult result = form.ShowDialog();

            // this means go ahead and remove it
            if (DialogResult.OK == result)
            {
                // save changes
                // first build new list of ServerNodes
                List<Server> list = new List<Server>();
                foreach (TreeNode iterationNode in _serverTree.TopNode.Nodes)
                {
                    // omit the one that was clicked on
                    if (iterationNode != _lastClickedNode)
                    {
                        Server ae = iterationNode.Tag as Server;
                        list.Add(new Server(ae.Name,
                            ae.Description,
                            ae.Host,
                            ae.AE,
                            ae.ListeningPort));
                    }
                }

                _serverPool.SaveNewDatabase(list);
                ResetTreeView();
                InitializeTreeView();
            }
        }

        void AddNewServer()
        {
            Server storedAE = _lastClickedNode.Tag as Server;
            ServerModifyForm form = new ServerModifyForm();

            form.Initialize(MenuItemFunction.AddNewServer, _serverTree, _lastClickedNode);
            DialogResult result = form.ShowDialog();

            // this means go ahead and remove it
            if (DialogResult.OK == result)
            {
                // save changes
                // first build new list of ServerNodes
                List<Server> list = new List<Server>();
                foreach (TreeNode iterationNode in _serverTree.TopNode.Nodes)
                {
                    Server ae = iterationNode.Tag as Server;
                    list.Add(new Server(ae.Name,
                        ae.Description,
                        ae.Host,
                        ae.AE,
                        ae.ListeningPort));
                }

                // add the new server
                list.Add(form.ModifiedServer);

                _serverPool.SaveNewDatabase(list);
                ResetTreeView();
                InitializeTreeView();
            }
        }

        void VerifyServer()
        {
            Server storedAE = _lastClickedNode.Tag as Server;
            ServerModifyForm form = new ServerModifyForm();
            form.Initialize(MenuItemFunction.VerifyServer, _serverTree, _lastClickedNode);
            form.ShowDialog();
        }

        public struct MenuItem
        {
            public MenuItem(String name, MenuItemFunction function)
            {
                _name = name;
                _function = function;
            }

            public String _name;
            public MenuItemFunction _function;
        }

        void nodeContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            TreeView tree = menu.SourceControl as TreeView;

            // TODO
            if (null == tree)
                throw new System.ArgumentException("Sender object is not a TreeView as expected");

            // there was no node or the click was on the root node (that says "Servers")
            if (null == _lastClickedNode)
                return;

            switch ((MenuItemFunction) e.ClickedItem.Tag)
            {
                case MenuItemFunction.Modify:
                    ModifyServer();
                    break;
                case MenuItemFunction.Remove:
                    RemoveServer();
                    break;
                case MenuItemFunction.AddNewServer:
                    AddNewServer();
                    break;
                case MenuItemFunction.VerifyServer:
                    VerifyServer();
                    break;
                default:            // TODO
                    throw new System.Exception("Unexpected condition in figuring out which context menu function was activated");
            }
        }

        private ContextMenuStrip GetNodeContextMenu()
        {
            MenuItem[] menuItemList = 
            {
                new MenuItem("Modify", MenuItemFunction.Modify),
                new MenuItem("Remove", MenuItemFunction.Remove),
                new MenuItem("Verify", MenuItemFunction.VerifyServer)
            };

            if (null != _nodeContextMenu)
                return _nodeContextMenu;

            _nodeContextMenu = new ContextMenuStrip();

            foreach (MenuItem mitem in menuItemList)
            {
                ToolStripItem titem = new ToolStripMenuItem(mitem._name);
                titem.Tag = mitem._function;

                _nodeContextMenu.Items.Add(titem);
            }

            return _nodeContextMenu;
        }

        private ContextMenuStrip GetTreeContextMenu()
        {
            MenuItem[] menuItemList = 
            {
                new MenuItem("Add New Server", MenuItemFunction.AddNewServer)
            };

            if (null != _treeContextMenu)
                return _treeContextMenu;

            _treeContextMenu = new ContextMenuStrip();

            foreach (MenuItem mitem in menuItemList)
            {
                ToolStripItem titem = new ToolStripMenuItem(mitem._name);
                titem.Tag = mitem._function;

                _treeContextMenu.Items.Add(titem);
            }

            return _treeContextMenu;
        }
        #endregion 

        #region Private Members
        private ServerPool _serverPool;
        private ReadOnlyServerCollection _serverList;
        private ContextMenuStrip _treeContextMenu;
        private ContextMenuStrip _nodeContextMenu;
        private TreeNode _lastClickedNode;
        #endregion
    }

    public enum MenuItemFunction
    {
        Modify,
        Remove,
        AddNewServer,
        VerifyServer
    }
}
