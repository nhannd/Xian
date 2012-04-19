using System;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
    //TODO (Marmot): Find places where IApplicationEntity is used and see if we can switch to using these.
    public interface IDicomServiceNode : IServiceNode, IApplicationEntity
    {
        bool IsLocal { get; }
        bool SupportsStreaming { get; }
    }

    public interface IServiceNode
    {
        bool IsSupported<T>() where T : class;
        void GetService<T>(Action<T> service) where T : class;
        T GetService<T>() where T : class;
    }

    public abstract class ServiceNode : IServiceNode
    {
        #region IServiceNode Members

        public abstract bool IsSupported<T>() where T : class;
        public void GetService<T>(Action<T> withService) where T : class
        {
            var service = GetService<T>();

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

        #endregion

        public abstract T GetService<T>() where T : class;
    }

    public static class ServiceNodeExtensions
    {
        public static ApplicationEntity ToDataContract(this IDicomServiceNode serviceNode)
        {
            return new ApplicationEntity(serviceNode);
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
