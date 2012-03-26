#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public interface IServerTreeLocalServer : IServerTreeNode
    {
        string AETitle { get; }
        string HostName { get; }
        int? Port { get; }
        string FileStoreLocation { get; }

        event EventHandler ConfigurationChanged;
    }
    
    public interface IServerTreeGroup : IServerTreeNode
    {
        new string Name { get; set; }
        IList<IServerTreeGroup> ChildGroups { get; }
        IList<IServerTreeNode> Servers { get; }

        void AddChild(IServerTreeNode child);

        List<IServerTreeNode> GetAllServers();
        List<IServerTreeNode> GetCheckedServers(bool recursive);

        bool IsEntireGroupChecked();
        bool IsEntireGroupUnchecked();
    }

    public interface IServerTreeDicomServer : IServerTreeNode
    {
        string AETitle { get; set; }
        string Location { get; set; }
        string HostName { get; set; }
        int Port { get; set; }
        bool IsStreaming { get; set; }
        int HeaderServicePort { get; set; }
        int WadoServicePort { get; set; }
    }

    public interface IServerTreeNode
    {
        //TODO: move IsChecked to the server tree level.
        bool IsChecked { get; set; }

        bool IsLocalDataStore { get; }
        bool IsServer { get; }
        bool IsServerGroup { get; }
        
        string ParentPath { get; }
        string Path { get; }
        string Name { get; }
        string DisplayName { get; }
    }
}