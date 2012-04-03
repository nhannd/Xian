using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public abstract class ServerDirectory : IServerDirectory
    {
        public abstract GetServersResult GetServers(GetServersRequest request);
        public abstract AddServerResult AddServer(AddServerRequest request);
        public abstract UpdateServerResult UpdateServer(UpdateServerRequest request);
        public abstract DeleteServerResult DeleteServer(DeleteServerRequest request);
        public abstract DeleteAllServersResult DeleteAllServers(DeleteAllServersRequest request);

        public static IDicomServiceNode GetLocalServer()
        {
            using (var bridge = new ServerDirectoryBridge())
            {
                return bridge.GetLocalServer();
            }
        }

        public static IDicomServiceNode GetRemoteServerByName(string name)
        {
            using (var bridge = new ServerDirectoryBridge())
            {
                return bridge.GetServerByName(name);
            }
        }

        public static IList<IDicomServiceNode> GetRemoteServersByAETitle(string aeTitle)
        {
            using (var bridge = new ServerDirectoryBridge())
            {
                return bridge.GetServersByAETitle(aeTitle);
            }
        }

        public static IList<IDicomServiceNode> GetRemoteServers()
        {
            using (var bridge = new ServerDirectoryBridge())
            {
                return bridge.GetServers();
            }
        }
    }
}
