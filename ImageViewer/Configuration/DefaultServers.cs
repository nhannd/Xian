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
using System.Collections.Specialized;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.ServerTree;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Configuration
{
    // TODO (Marmot): Gonzo.
	public static class DefaultServers
	{
		public static List<Server> SelectFrom(IEnumerable<Server> candidates)
		{
			StringCollection defaultServerPaths = DefaultServerSettings.Default.DefaultServerPaths;

			if (defaultServerPaths == null)
				return new List<Server>();

			return CollectionUtils.Select(candidates, delegate(Server node) { return defaultServerPaths.Contains(node.Path); });
		}

		public static List<Server> SelectFrom(Common.ServerTree.ServerTree serverTree)
		{
			List<Server> allServers = CollectionUtils.Map(serverTree.FindChildServers(),
				delegate(IServerTreeNode server) { return (Server) server; });
			
			return SelectFrom(allServers);
		}

		public static List<Server> GetAll()
		{
			Common.ServerTree.ServerTree tree = new Common.ServerTree.ServerTree();
			return SelectFrom(tree);
		}

        public static IEnumerable<IStudyRootQuery> GetQueryInterfaces(bool includeLocal)
		{
            if (includeLocal)
            {
                IStudyRootQuery localDataStoreQuery;
                try
                {
                    localDataStoreQuery = (IStudyRootQuery) new LocalStudyRootQueryExtensionPoint().CreateExtension();
                }
                catch (NotSupportedException)
                {
                    localDataStoreQuery = null;
                }

                if (localDataStoreQuery != null)
                    yield return localDataStoreQuery;
            }

            string localAE = DicomServerConfigurationHelper.GetOfflineAETitle(false);

			List<Server> defaultServers = DefaultServers.SelectFrom(new Common.ServerTree.ServerTree());
			List<Server> streamingServers = CollectionUtils.Select(defaultServers, 
				delegate(Server server) { return server.IsStreaming; });

			List<Server> nonStreamingServers = CollectionUtils.Select(defaultServers,
				delegate(Server server) { return !server.IsStreaming; });

			foreach (Server server in streamingServers)
			{
				DicomStudyRootQuery remoteQuery = new DicomStudyRootQuery(localAE, server.AETitle, server.Host, server.Port);
				yield return remoteQuery;
			}

			foreach (Server server in nonStreamingServers)
			{
				DicomStudyRootQuery remoteQuery = new DicomStudyRootQuery(localAE, server.AETitle, server.Host, server.Port);
				yield return remoteQuery;
			}
		}
	}
}
