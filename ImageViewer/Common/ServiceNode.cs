using System;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common
{
    //public interface IPersistedServiceNode : IServiceNode
    //{
    //    Int64 Oid { get; }
    //}

    public interface IDicomServiceNode : IServiceNode, IDicomServerApplicationEntity
    {
        bool IsLocal { get; }
    }

    public interface IStreamingDicomServiceNode : IServiceNode, IStreamingServerApplicationEntity
    {
    }

    public interface IServiceNode
    {
        bool IsSupported<T>() where T : class;
        T GetService<T>() where T : class;
    }

    public abstract class ServiceNode : IServiceNode
    {
        #region IServiceNode Members

        public abstract bool IsSupported<T>() where T : class;
        public abstract T GetService<T>() where T : class;

        #endregion
    }

    public static class ServiceNodeExtensions
    {
        public static DicomServerApplicationEntity ToDataContract(this IDicomServiceNode serviceNode)
        {
            if (serviceNode.IsStreaming)
            {
                var ae = (IStreamingServerApplicationEntity) serviceNode;
                return new StreamingServerApplicationEntity(ae);
            }
            else
            {
                var ae = (IDicomServerApplicationEntity)serviceNode;
                return new DicomServerApplicationEntity(ae);
            }
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
