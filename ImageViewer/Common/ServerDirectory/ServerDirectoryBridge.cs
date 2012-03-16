using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public class ServerDirectoryBridge : IDisposable
    {
        private IServerDirectory _serverDirectory;

        public ServerDirectoryBridge()
            : this(Platform.GetService<IServerDirectory>())
        {
        }

        public ServerDirectoryBridge(IServerDirectory serverDirectory)
        {
            Platform.CheckForNullReference(serverDirectory, "serverDirectory");
            _serverDirectory = serverDirectory;
        }

        public List<IServiceNode> GetServers()
        {
            var servers = _serverDirectory.GetServers(new GetServersRequest()).ServerEntries;
            return servers.Select(s => s.ToServiceNode()).ToList();
        }

        public List<IServiceNode> GetServerByAETitle(string aeTitle)
        {
            var servers = _serverDirectory.GetServers(new GetServersRequest{AETitle = aeTitle}).ServerEntries;
            return servers.Select(s => s.ToServiceNode()).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!(_serverDirectory is IDisposable))
                return;

            ((IDisposable)_serverDirectory).Dispose();
            _serverDirectory = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
            }
        }

        #endregion
    }
}
