using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public abstract class ServerDirectory : IServerDirectory
    {
        static ServerDirectory()
        {
            try
            {
                //TODO (Marmot): This really the best way to do this?
                var service = Platform.GetService<IServerDirectory>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch(EndpointNotFoundException)
            {
                //This doesn't mean it's not supported, it means it's not running.
                IsSupported = true;
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Server Directory is not supported.");
            }
            catch (UnknownServiceException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Server Directory is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Server Directory is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

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

        public abstract GetServersResult GetServers(GetServersRequest request);
        public abstract AddServerResult AddServer(AddServerRequest request);
        public abstract UpdateServerResult UpdateServer(UpdateServerRequest request);
        public abstract DeleteServerResult DeleteServer(DeleteServerRequest request);
        public abstract DeleteAllServersResult DeleteAllServers(DeleteAllServersRequest request);
    }
}
