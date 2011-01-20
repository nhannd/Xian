#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Collections.Specialized;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class DefaultServers
	{
		public static List<Server> SelectFrom(IEnumerable<Server> candidates)
		{
			StringCollection defaultServerPaths = DefaultServerSettings.Default.DefaultServerPaths;

			if (defaultServerPaths == null)
				return new List<Server>();

			return CollectionUtils.Select(candidates, delegate(Server node) { return defaultServerPaths.Contains(node.Path); });
		}

		public static List<Server> SelectFrom(Services.ServerTree.ServerTree serverTree)
		{
			List<Server> allServers = CollectionUtils.Map<IServerTreeNode, Server>(serverTree.FindChildServers(),
				delegate(IServerTreeNode server) { return (Server) server; });
			
			return SelectFrom(allServers);
		}

		public static List<Server> GetAll()
		{
			ImageViewer.Services.ServerTree.ServerTree tree = new ImageViewer.Services.ServerTree.ServerTree();
			return SelectFrom(tree);
		}
	}
}
