#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration.ServerTree
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

		ImageViewer.Services.ServerTree.ServerTree ServerTree { get; }

		AEServerGroup SelectedServers { get; }
		event EventHandler SelectedServerChanged;
        
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

			public ImageViewer.Services.ServerTree.ServerTree ServerTree
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

		private ImageViewer.Services.ServerTree.ServerTree _serverTree;
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

		public ImageViewer.Services.ServerTree.ServerTree ServerTree
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

		public ServerTreeComponent()
		{
			_selectedServers = new AEServerGroup();
			_serverTree = new ImageViewer.Services.ServerTree.ServerTree();

			if (_serverTree.CurrentNode != null && _serverTree.CurrentNode.IsServer || _serverTree.CurrentNode.IsLocalDataStore)
			{
				_selectedServers.Servers.Add(_serverTree.CurrentNode);
				_selectedServers.Name = _serverTree.CurrentNode.Name;
				_selectedServers.GroupID = _serverTree.CurrentNode.Path;
			}
		}

		public void SetSelection(IServerTreeNode dataNode)
		{
			if (dataNode.IsServer || dataNode.IsLocalDataStore)
			{
				_selectedServers = new AEServerGroup();
				_selectedServers.Servers.Add(dataNode);
				_selectedServers.Name = dataNode.Name;
				_selectedServers.GroupID = dataNode.Path;
				_serverTree.CurrentNode = dataNode;
				FireSelectedServerChangedEvent();
			}
			else if (dataNode.IsServerGroup)
			{
				_selectedServers = new AEServerGroup();
				_selectedServers.Servers = _serverTree.FindChildServers(dataNode as ServerGroup);
				_selectedServers.GroupID = dataNode.Path;
				_selectedServers.Name = dataNode.Name;
				_serverTree.CurrentNode = dataNode;
				FireSelectedServerChangedEvent();
			}

		}

		public bool NodeMoved(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
		{
			if (!CanMoveOrAdd(destinationNode, movingDataNode))
				return false;

			if (movingDataNode.IsServer)
			{
				Server movingServer = (Server)movingDataNode;
				_serverTree.CurrentNode = movingDataNode;
				_serverTree.DeleteDicomServer();

				((ServerGroup)destinationNode).AddChild(movingDataNode);
				SetSelection(movingDataNode);
			}
			else if (movingDataNode.IsServerGroup)
			{
				ServerGroup movingGroup = (ServerGroup)movingDataNode;
				_serverTree.CurrentNode = movingGroup;
				_serverTree.DeleteServerGroup();

				((ServerGroup)destinationNode).AddChild(movingGroup);
				SetSelection(movingGroup);
			}
			_serverTree.Save();
			return true;
		}

		public bool CanMoveOrAdd(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
		{
			if (IsReadOnly)
				return false;

			return _serverTree.CanMoveOrAdd(destinationNode, movingDataNode);
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