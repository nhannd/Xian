using System;
using System.Linq;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Common
{
    public interface IDicomServiceNode : IServiceNode, IApplicationEntity
    {
        bool IsLocal { get; }
    }

    public interface IServiceNode
    {
        bool IsSupported<T>() where T : class;
        void GetService<T>(Action<T> withService) where T : class;
        T GetService<T>() where T : class;
    }

    public abstract class ServiceNode : IServiceNode
    {
        #region IServiceNode Members

        public abstract bool IsSupported<T>() where T : class;
        public void GetService<T>(Action<T> withService) where T : class
        {
            WithService(GetService<T>(), withService);
        }

        #endregion

        public abstract T GetService<T>() where T : class;

        public static void WithService<T>(T service, Action<T> withService) where T : class
        {
            try
            {
                withService(service);
            }
            catch (Exception)
            {
                var disposable = service as IDisposable;
                if (disposable != null)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Warn, ex, "Error disposing service object of type '{0}'", typeof(T).FullName);
                    }
                }

                throw;
            }
        }
    }

    public static class ServiceNodeExtensions
    {
        public static IDicomServiceNode ToServiceNode(this DicomServerConfiguration serverConfiguration)
        {
            Platform.CheckForNullReference(serverConfiguration, "serverConfiguration");
            return new DicomServiceNode(serverConfiguration);
        }

        public static IDicomServiceNode ToServiceNode(this IApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            var dicomServiceNode = server as IDicomServiceNode;
            if (dicomServiceNode != null)
                return dicomServiceNode;
            
            return new DicomServiceNode(server);
        }

        public static ApplicationEntity ToDataContract(this IDicomServiceNode serviceNode)
        {
            return new ApplicationEntity(serviceNode);
        }

        public static bool ResolveServer(this Identifier identifier, bool defaultToLocal)
        {
            var server = FindServer(identifier, defaultToLocal);
            if (server != null)
            {
                identifier.RetrieveAE = server;
                return true;
            }

            return false;
        }

        public static IDicomServiceNode FindServer(this IIdentifier identifier, bool defaultToLocal)
        {
            IDicomServiceNode server = null;
            if (identifier.RetrieveAE != null)
                server = identifier.RetrieveAE.ToServiceNode();

            if (!String.IsNullOrEmpty(identifier.RetrieveAeTitle))
                server = ServerDirectory.ServerDirectory.GetRemoteServersByAETitle(identifier.RetrieveAeTitle).FirstOrDefault();

            if (server == null && defaultToLocal)
                server = ServerDirectory.ServerDirectory.GetLocalServer();

            return server;
        }
    }

    // TODO (CR Mar 2012): Later, so services provided by service nodes can be extended.
    //public interface IServerNodeServiceProvider
    //{
    //    bool Supports(Type type, IServerNode serverNode);
    //    object GetService(Type type, IServerNode serverNode);
    //}

    //public class ServiceNodeServiceProviderExtensionPoint : ExtensionPoint<IServerNodeServiceProvider>
    //{
    //}
}
