#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Common.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ExtensionPoint()]
	public sealed class ServerTreeToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint()]
	public sealed class ServerTreeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface IServerTreeToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }

		Common.ServerTree.ServerTree ServerTree { get; }

		AEServerGroup SelectedServers { get; }
		event EventHandler SelectedServerChanged;

		bool IsReadOnly { get; }
		int UpdateType { get; set; }
	}

	[AssociateView(typeof(ServerTreeComponentViewExtensionPoint))]
	public class ServerTreeComponent : ApplicationComponent
	{
		public class ServerTreeToolContext : ToolContext, IServerTreeToolContext
		{
			ServerTreeComponent _component;

			public ServerTreeToolContext(ServerTreeComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			#region IServerTreeToolContext Members

			public Common.ServerTree.ServerTree ServerTree
			{
				get { return _component._serverTree; }
			}

			public AEServerGroup SelectedServers
			{
				get { return _component.SelectedServers; }
			}

			public event EventHandler SelectedServerChanged
			{
				add { _component.SelectedServerChanged += value; }
				remove { _component.SelectedServerChanged -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public bool IsReadOnly
			{
				get { return _component.IsReadOnly; }
			}

			public int UpdateType
			{
				get { return _component.UpdateType; }
				set { _component.UpdateType = value; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			#endregion
		}

		#region Fields

		private Common.ServerTree.ServerTree _serverTree;
		private event EventHandler _selectedServerChanged;
		private AEServerGroup _selectedServers;
		private int _updateType;
		private ToolSet _toolSet;
		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;
		private ClickHandlerDelegate _defaultActionHandler;
		private bool _showTools = true;
		private bool _showTitlebar = true;
		private bool _showLocalDataStoreNode = true;
		private bool _isReadOnly = false;
		private bool _showCheckBoxes = false;
		private bool _isEnabled = true;

		#endregion


		public ServerTreeComponent()
		{
			_selectedServers = new AEServerGroup();
			_serverTree = new Common.ServerTree.ServerTree();

			if (_serverTree.CurrentNode != null && (_serverTree.CurrentNode.IsServer || _serverTree.CurrentNode.IsLocalDataStore))
			{
				_selectedServers.Servers.Add(_serverTree.CurrentNode);
				_selectedServers.Name = _serverTree.CurrentNode.DisplayName;
				_selectedServers.GroupID = _serverTree.CurrentNode.Path;
			}
		}

		#region Presentation Model

		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				if(value != _isEnabled)
				{
					_isEnabled = value;
					NotifyPropertyChanged("IsEnabled");
				}
			}
		}

		public Common.ServerTree.ServerTree ServerTree
		{
			get { return _serverTree; }
		}

		public AEServerGroup SelectedServers
		{
			get { return _selectedServers; }
		}

		public int UpdateType
		{
			get { return _updateType; }
			set { _updateType = value; }
		}

		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public bool ShowTools
		{
			get { return _showTools; }
			set { _showTools = value; }
		}

		public bool ShowTitlebar
		{
			get { return _showTitlebar; }
			set { _showTitlebar = value; }
		}

		public bool ShowCheckBoxes
		{
			get { return _showCheckBoxes; }
			set { _showCheckBoxes = value; }
		}

		public bool ShowLocalDataStoreNode
		{
			get { return _showLocalDataStoreNode; }
			set { _showLocalDataStoreNode = value; }
		}

		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set { _isReadOnly = value; }
		}

		#endregion

		public void SetSelection(IServerTreeNode dataNode)
		{
			if (dataNode.IsServer || dataNode.IsLocalDataStore)
			{
				_selectedServers = new AEServerGroup();
				_selectedServers.Servers.Add(dataNode);
				_selectedServers.Name = dataNode.DisplayName;
				_selectedServers.GroupID = dataNode.Path;
				_serverTree.CurrentNode = dataNode;
				FireSelectedServerChangedEvent();
			}
			else if (dataNode.IsServerGroup)
			{
				_selectedServers = new AEServerGroup();
				_selectedServers.Servers = _serverTree.FindChildServers(dataNode as IServerTreeGroup);
				_selectedServers.GroupID = dataNode.Path;
				_selectedServers.Name = dataNode.DisplayName;
				_serverTree.CurrentNode = dataNode;
				FireSelectedServerChangedEvent();
			}

		}

		public bool NodeMoved(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
		{
			if (!CanMove(destinationNode, movingDataNode))
				return false;

			if (movingDataNode.IsServer)
			{
				_serverTree.CurrentNode = movingDataNode;
				_serverTree.DeleteServer();

				((IServerTreeGroup)destinationNode).AddChild(movingDataNode);
				SetSelection(movingDataNode);
			}
			else if (movingDataNode.IsServerGroup)
			{
                var movingGroup = (IServerTreeGroup)movingDataNode;
				_serverTree.CurrentNode = movingGroup;
				_serverTree.DeleteGroup();
                _serverTree.Save();

                ((IServerTreeGroup)destinationNode).AddChild(movingGroup);
				SetSelection(movingGroup);
			}
			_serverTree.Save();
			return true;
		}

		public bool CanMove(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
		{
			if (IsReadOnly)
				return false;

			return _serverTree.CanMove(destinationNode, movingDataNode);
		}
		
		public event EventHandler SelectedServerChanged
		{
			add { _selectedServerChanged += value; }
			remove { _selectedServerChanged -= value; }
		}

		private void FireSelectedServerChangedEvent()
		{
			EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
		}

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new ServerTreeToolExtensionPoint(), new ServerTreeToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "servertree-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "servertree-contextmenu", _toolSet.Actions);
		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		public void NodeDoubleClick()
		{
			//Could be something other than edit that does no harm, but we might as well just disallow unconditionally.
			if (IsReadOnly)
				return;
        	
			// according to the framework architecture, the default action handler
			// for this component is set up by the ServerEditTool
			// however, since the tool is used for both Server and ServerGroup
			// and we want to retain the behaviour of expanding the tree node when
			// a ServerGroup is d-clicked, we only want the edit tool invoked if
			// the node is a Server
			if (!_serverTree.CurrentNode.IsLocalDataStore && 
			    !_serverTree.CurrentNode.IsServerGroup && 
			    null != _defaultActionHandler &&
			    _showTools)
				_defaultActionHandler();
		}
	}

	public enum ServerUpdateType
	{
		None = 0,
		Add,
		Edit,
		Delete
	}
}