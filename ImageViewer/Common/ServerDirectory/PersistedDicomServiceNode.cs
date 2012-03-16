using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public interface IPersistedStreamingDicomServiceNode : IPersistedServiceNode, IStreamingDicomServiceNode
    {
    }

    public interface IPersistedDicomServiceNode : IPersistedServiceNode, IDicomServiceNode
    {
    }

    internal class PersistedStreamingDicomServiceNode : PersistedDicomServiceNode, IPersistedStreamingDicomServiceNode
    {
        internal PersistedStreamingDicomServiceNode(PersistedServerEntry entry)
            : base(entry)
        {
            Platform.CheckExpectedType(entry.Server, typeof(StreamingServerApplicationEntity));
        }

        #region Implementation of IStreamingServerApplicationEntity

        public int HeaderServicePort
        {
            get { return ((StreamingServerApplicationEntity)Real).HeaderServicePort; }
        }

        public int WadoServicePort
        {
            get { return ((StreamingServerApplicationEntity)Real).WadoServicePort; }
        }

        #endregion
    }

    internal class PersistedDicomServiceNode : ServiceNode, IPersistedDicomServiceNode
    {
        private readonly Int64 _oid;

        internal PersistedDicomServiceNode(PersistedServerEntry entry)
        {
            Platform.CheckForNullReference(entry, "entry");
            Platform.CheckExpectedType(entry.Server, typeof(DicomServerApplicationEntity));

            _oid = entry.Oid;
            Real = (DicomServerApplicationEntity)entry.Server;
        }

        protected DicomServerApplicationEntity Real { get; private set; }
        public Int64 Oid { get { return _oid; } }

        #region Implementation of IApplicationEntity

        public string AETitle
        {
            get { return Real.AETitle; }
        }

        public string Name
        {
            get { return Real.Name; }
        }

        public string Description
        {
            get { return Real.Description; }
        }

        public string Location
        {
            get { return Real.AETitle; }
        }

        #endregion

        #region Implementation of IDicomServerApplicationEntity

        public string HostName
        {
            get { return Real.HostName; }
        }

        public int Port
        {
            get { return Real.Port; }
        }

        // TODO (Marmot): try to get rid of this.
        public bool IsStreaming
        {
            get { return Real.IsStreaming; }
        }

        #endregion

        public override bool IsSupported<T>()
        {
            if (typeof(T).Equals(typeof(IStudyRootQuery)))
                return true;

            return false;
        }

        public override T GetService<T>()
        {
            // TODO (Marmot): Fix this.
            string localAE = "Whatever";
            if (typeof(T).Equals(typeof(IStudyRootQuery)))
                return new DicomStudyRootQuery(localAE, AETitle, HostName, Port) as T;

            throw new NotSupportedException();
        }
    }
}