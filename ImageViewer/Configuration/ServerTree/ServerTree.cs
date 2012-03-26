#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    // TODO (CR Mar 2012): One day, maybe merge this into the ServerTreeComponent, but it's probably not worth it ... it works.
    public partial class ServerTree
    {
        public ServerTree()
            : this(ServerTreeSettings.Default.GetSharedServers(), GetServersFromDirectory())
        {
        }

        internal ServerTree(StoredServerGroup rootGroup, List<ApplicationEntity> directoryServers)
        {
            LocalServer = new ServerTreeLocalServer();

            //Create a copy because we will modify it.
            directoryServers = directoryServers == null
                ? new List<ApplicationEntity>()
                : new List<ApplicationEntity>(directoryServers);

            var directoryServerCount = directoryServers.Count;

            if (rootGroup == null)
            {
                InitializeRootGroup();
            }
            else
            {
                //ToServerTreeGroup eliminates the items from the passed in list of
                //servers as the references are found in the tree.
                RootServerGroup = rootGroup.ToServerTreeGroup(directoryServers);
            }

            if (directoryServerCount == 0)
            {
                AddExamples();
            }
            else
            {
                //rootGroup.ToServerTreeGroup above deletes the entries from the list of servers,
                //so if there are any left, that means there was no match in the tree. So,
                //we will just add those servers to the root.
                foreach (ApplicationEntity server in directoryServers)
                    RootServerGroup.Servers.Add(new ServerTreeDicomServer(server));
            }

            InitializePaths();
            CurrentNode = LocalServer;
        }

        internal ServerTree(ServerTreeRoot legacyRoot)
        {
            LocalServer = new ServerTreeLocalServer();
            
            if (legacyRoot == null)
            {
                InitializeRootGroup();
                AddExamples();
            }
            else
            {
                RootServerGroup = legacyRoot.ServerGroupNode.ToServerTreeGroup();
            }

            InitializePaths();
            CurrentNode = LocalServer;
        }

        private void InitializeRootGroup()
        {
            RootServerGroup = new ServerTreeGroup(@"My Servers");
        }

        private void InitializePaths()
        {
            ((ServerTreeLocalServer)LocalServer).ChangeParentPath(_rootPath);
            ((ServerTreeGroup)RootServerGroup).ChangeParentPath(_rootPath);
        }

        private void AddExamples()
        {
            RootServerGroup.ChildGroups.Add(new ServerTreeGroup(SR.ExampleGroup));
            var exampleServer = new ServerTreeDicomServer(SR.ExampleServer, "", "localhost", "SAMPLE", 104, false, 50221, 1000);
            RootServerGroup.Servers.Add(exampleServer);
        }

        internal static List<ApplicationEntity> GetServersFromDirectory()
        {
            try
            {
                List<ApplicationEntity> servers = null;
                Platform.GetService<IServerDirectory>(
                    directory => servers = directory.GetServers(new GetServersRequest()).Servers);
                return servers.OfType<ApplicationEntity>().ToList();
            }
            catch (Exception e)
            {
                //TODO (Marmot): Should this throw?
                Platform.Log(LogLevel.Warn, e, "Failed to load servers from directory.");
            }

            return new List<ApplicationEntity>();
        }

        public void DeleteCurrentNode()
        {
            if (CurrentNode.IsServer)
                DeleteServer();
            else if (CurrentNode.IsServerGroup)
                DeleteGroup();
        }

        public void DeleteServer()
        {
            if (!CurrentNode.IsServer)
                return;

            var parentGroup = FindParentGroup(CurrentNode);
            if (parentGroup == null)
                return;

            for (int i = 0; i < parentGroup.Servers.Count; i++)
            {
                if (parentGroup.Servers[i].Name == CurrentNode.Name)
                {
                    parentGroup.Servers.RemoveAt(i);
                    CurrentNode = parentGroup;
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void DeleteGroup()
        {
            if (!CurrentNode.IsServerGroup)
				return;
			
			var parentGroup = FindParentGroup(CurrentNode);
            if (null == parentGroup)
                return;	


            for (int i = 0; i < parentGroup.ChildGroups.Count; ++i)
            {
                if (parentGroup.ChildGroups[i].Name == CurrentNode.Name)
                {
                    parentGroup.ChildGroups.RemoveAt(i);
                    CurrentNode = parentGroup;
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void ReplaceDicomServerInCurrentGroup(IServerTreeDicomServer newServer)
        {
            if (!CurrentNode.IsServer)
                return;

            var serverGroup = FindParentGroup(CurrentNode);
            if (serverGroup == null)
                return;

            for (int i = 0; i < serverGroup.Servers.Count; i++)
            {
                if (serverGroup.Servers[i].Name == CurrentNode.Name)
                {
                    serverGroup.Servers[i] = newServer;
                    ((ServerTreeDicomServer)newServer).ChangeParentPath(serverGroup.Path);
                    CurrentNode = newServer;
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void Save()
        {
            SaveServersToDirectory();
            SaveToSettings();
        }

        internal void SaveServersToDirectory()
        {
            Platform.GetService<IServerDirectory>(SaveServersToDirectory);
        }

        internal void SaveServersToDirectory(IServerDirectory directory)
        {
            //Get all the DICOM servers from the directory.
            var directoryServers = directory.GetServers(new GetServersRequest()).Servers
                                        .OfType<ApplicationEntity>().ToList();

            //Convert the tree items to data contracts.
            var treeServers = RootServerGroup.GetAllServers()
                                    .OfType<IServerTreeDicomServer>().Select(a => a.ToDataContract());
            
            //Figure out which items have been deleted.
            var deleted = from d in directoryServers where !treeServers.Any(t => t.Name == d.Name) select d;
            //Figure out which items are new.
            var added = (from t in treeServers where !directoryServers.Any(d => t.Name == d.Name) select t);
            //Figure out which items have changed.
            var changed = (from t in treeServers where directoryServers.Any(d => t.Name == d.Name && !t.Equals(d)) select t);

            //Most updates are done one server at a time, anyway, so we'll just do this.
            //Could implement bulk update methods on the service, too.
            foreach (var d in deleted)
            {
                try
                {
                    directory.DeleteServer(new DeleteServerRequest { Server = d });
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Server being deleted ('{0}') does not exist in directory.", d.Name);
                }
            }

            foreach (var c in changed)
              directory.UpdateServer(new UpdateServerRequest{Server = c});
            foreach (var a in added)
              directory.AddServer(new AddServerRequest { Server = a });
        }

        private void SaveToSettings()
        {
            ServerTreeSettings.Default.UpdateSharedServers(RootServerGroup.ToStoredServerGroup());
        }
    }
}