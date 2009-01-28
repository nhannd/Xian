using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration
{
	public static class DefaultServers
	{
		public static List<Server> GetServers()
		{
			ImageViewer.Services.ServerTree.ServerTree tree = new ImageViewer.Services.ServerTree.ServerTree();
			List<Server> allServers = tree.RootNode.ServerGroupNode.ChildServers;
			StringCollection defaultServerPaths = DefaultServerSettings.Default.DefaultServerPaths;

			if (defaultServerPaths == null)
				return new List<Server>();

			return CollectionUtils.Select(allServers, delegate(Server node) { return defaultServerPaths.Contains(node.Path); });
		}
	}

}
