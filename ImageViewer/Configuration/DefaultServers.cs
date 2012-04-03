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
using System.Linq;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration
{
    // TODO (Marmot): Gonzo.
	public static class DefaultServers
	{
        internal static List<IDicomServiceNode> SelectFrom(IEnumerable<IServerTreeDicomServer> candidates)
		{
			StringCollection defaultServerPaths = DefaultServerSettings.Default.DefaultServerPaths;
			if (defaultServerPaths == null)
                return new List<IDicomServiceNode>();

            var matches = candidates.OfType<IServerTreeDicomServer>().Where(s => defaultServerPaths.Contains(s.Path)).Cast<IServerTreeDicomServer>();
            var results = matches.Select(m => m.ToDataContract().ToServiceNode());
            return results.ToList();
		}

        internal static List<IDicomServiceNode> SelectFrom(ServerTree.ServerTree serverTree)
		{
            var allServers = serverTree.FindChildServers().OfType<IServerTreeDicomServer>();
			return SelectFrom(allServers);
		}

        public static List<IDicomServiceNode> GetAll()
		{
            var tree = new ServerTree.ServerTree();
            return SelectFrom(tree);
		}

        public static IEnumerable<IStudyRootQuery> GetQueryInterfaces(bool includeLocal)
		{
            if (includeLocal)
            {
                IStudyRootQuery localDataStoreQuery = LocalStudyRootQuery.Create();
                if (localDataStoreQuery != null)
                    yield return localDataStoreQuery;
            }

            string localAE = DicomServerConfigurationHelper.AETitle;

            var defaultServers = SelectFrom(new ServerTree.ServerTree());
            var streamingServers = defaultServers.Where(s => s.StreamingParameters != null).ToList();
            var nonStreamingServers = defaultServers.Where(s => s.StreamingParameters == null).ToList();

            foreach (var server in streamingServers)
			{
				var remoteQuery = new DicomStudyRootQuery(localAE, server.AETitle, server.ScpParameters.HostName, server.ScpParameters.Port);
				yield return remoteQuery;
			}

            foreach (var server in nonStreamingServers)
			{
                var remoteQuery = new DicomStudyRootQuery(localAE, server.AETitle, server.ScpParameters.HostName, server.ScpParameters.Port);
				yield return remoteQuery;
			}
		}
	}
}
