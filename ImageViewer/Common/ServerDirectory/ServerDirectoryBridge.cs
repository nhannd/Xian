using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

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

        public IDicomServiceNode GetLocalServer()
        {
            DicomServerConfigurationHelper.Refresh(false);
            return new DicomServiceNode(DicomServerConfigurationHelper.DicomServerConfiguration);
        }

        public List<IDicomServiceNode> GetServers()
        {
            var servers = _serverDirectory.GetServers(new GetServersRequest()).Servers;
            return servers.Select(s => s.ToServiceNode()).ToList();
        }

        public List<IDicomServiceNode> GetServersByAETitle(string aeTitle)
        {
            var servers = _serverDirectory.GetServers(new GetServersRequest{AETitle = aeTitle}).Servers;
            return servers.Select(s => s.ToServiceNode()).ToList();
        }

        public IDicomServiceNode GetServerByName(string name)
        {
            var servers = _serverDirectory.GetServers(new GetServersRequest { Name = name}).Servers;
            return servers.Select(s => s.ToServiceNode()).FirstOrDefault();
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
