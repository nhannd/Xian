#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    public class ServerTreeGroup : IServerTreeGroup
    {
        #region Private Fields
        private const string _rootServersGroupPath = @"./My Servers/";
        private string _parentPath;
        private string _path;
        private string _name;
        #endregion
        
        public ServerTreeGroup(string name)
        {
            ChildGroups = new List<IServerTreeGroup>();
            Servers = new List<IServerTreeNode>();

            _name = name;
        }

        #region IServerTreeNode Members

        public bool IsChecked { get; set; }

        public bool IsLocalDataStore
        {
            get { return false; }
        }

        public bool IsServer
        {
            get { return false; }
        }

        public bool IsServerGroup
        {
            get { return true; }
        }

        public string Path
        {
            get { return _path; }
        }

        public bool IsRoot
        {
            get { return Path == _rootServersGroupPath; }
        }

        public String Name
        {
            get { return _name; }
            set
            {
                if (IsRoot)
                    throw new InvalidOperationException("Cannot rename the root node.");

                _name = value;
                ChangeParentPath(_parentPath);
            }
        }

        public string DisplayName
        {
            // if this group is the default "My Servers" node (and not manually customized by user), display the localized name
            get { return Path == _rootServersGroupPath ? SR.MyServersTitle : Name; }
        }

        public string ParentPath
        {
            get { return _parentPath; }
        }

        #endregion

        #region IServerTreeGroup Members

        public IList<IServerTreeNode> Servers { get; private set; }
        public IList<IServerTreeGroup> ChildGroups { get; private set; }

        #endregion

        internal void ChangeParentPath(string newParentPath)
        {
            // change the parent path of myself and all my children
            _parentPath = newParentPath ?? "";
            _path = _parentPath + Name + "/";

            foreach (ServerTreeDicomServer server in Servers)
                server.ChangeParentPath(_path);

            foreach (ServerTreeGroup serverGroup in ChildGroups)
                serverGroup.ChangeParentPath(_path);
        }

        public void AddChild(IServerTreeNode child)
        {
            if (child.IsServer)
            {
                var childServer = (ServerTreeDicomServer)child;
                Servers.Add(childServer);
                childServer.ChangeParentPath(_path);
            }
            else if (child.IsServerGroup)
            {
                var groups = (ServerTreeGroup)child;
                ChildGroups.Add(groups);
                groups.ChangeParentPath(_path);
            }
        }

        public bool IsEntireGroupChecked()
        {
            return IsEntireGroupChecked(this);
        }

        public bool IsEntireGroupUnchecked()
        {
            return IsEntireGroupUnchecked(this);
        }

        public List<IServerTreeNode> GetAllServers()
        {
            var servers = new List<IServerTreeNode>();
            foreach (ServerTreeGroup childGroup in ChildGroups)
                servers.AddRange(childGroup.GetAllServers());

            servers.AddRange(Servers);
            return servers;
        }

        public List<IServerTreeNode> GetCheckedServers(bool recursive)
        {
            var checkedServers = new List<IServerTreeNode>();
            if (recursive)
            {
                foreach (ServerTreeGroup childGroup in ChildGroups)
                    checkedServers.AddRange(childGroup.GetCheckedServers(true));
            }

            checkedServers.AddRange(CollectionUtils.Select(Servers, server => server.IsChecked));
            return checkedServers;
        }

        private static bool IsEntireGroupChecked(IServerTreeGroup group)
        {
            foreach (var server in group.Servers)
            {
                if (!server.IsChecked)
                    return false;
            }

            foreach (IServerTreeGroup subGroup in group.ChildGroups)
            {
                if (!IsEntireGroupChecked(subGroup))
                    return false;
            }

            return true;
        }

        private static bool IsEntireGroupUnchecked(IServerTreeGroup group)
        {
            foreach (IServerTreeNode server in group.Servers)
            {
                if (server.IsChecked)
                    return false;
            }

            foreach (IServerTreeGroup subGroup in group.ChildGroups)
            {
                if (!IsEntireGroupUnchecked(subGroup))
                    return false;
            }

            return true;
        }
        
        public override string ToString()
        {
            return Path.StartsWith(_rootServersGroupPath) 
                ? string.Format(@"./{0}/{1}", SR.MyServersTitle, Path.Substring(_rootServersGroupPath.Length)) 
                : Path;
        }
    }
}