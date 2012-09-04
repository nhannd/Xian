using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public class ServerDirectoryBridge : IDisposable
    {
        private IServerDirectory _real;

        public ServerDirectoryBridge()
        {
        }

        public ServerDirectoryBridge(IServerDirectory serverDirectory)
        {
            Platform.CheckForNullReference(serverDirectory, "serverDirectory");
            _real = serverDirectory;
        }

        private IServerDirectory Real
        {
            get { return _real ?? (_real = Platform.GetService<IServerDirectory>()); }
        }

        public IDicomServiceNode GetLocalServer()
        {
            return new DicomServiceNode(DicomServer.DicomServer.GetConfiguration());
        }

        public List<IDicomServiceNode> GetServers()
        {
            var servers = Real.GetServers(new GetServersRequest()).ServerEntries;
            return servers.Select(s => s.ToServiceNode()).ToList();
        }

        public List<IDicomServiceNode> GetServersByAETitle(string aeTitle)
        {
            var servers = Real.GetServers(new GetServersRequest { AETitle = aeTitle }).ServerEntries;
            return servers.Select(s => s.ToServiceNode()).ToList();
        }

        public IDicomServiceNode GetServerByName(string name)
        {
            var servers = Real.GetServers(new GetServersRequest { Name = name }).ServerEntries;
            return servers.Select(s => s.ToServiceNode()).FirstOrDefault();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!(_real is IDisposable))
                return;

            ((IDisposable)_real).Dispose();
            _real = null;
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
