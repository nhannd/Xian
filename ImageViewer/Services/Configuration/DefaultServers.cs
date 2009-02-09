using System.Collections.Generic;
using System.Collections.Specialized;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration
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
			List<Server> allServers = serverTree.RootNode.ServerGroupNode.ChildServers;
			return SelectFrom(allServers);
		}

		public static List<Server> GetAll()
		{
			ImageViewer.Services.ServerTree.ServerTree tree = new ImageViewer.Services.ServerTree.ServerTree();
			return SelectFrom(tree);
		}
	}
}
