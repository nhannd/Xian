using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    internal class StreamingDicomServiceNode : DicomServiceNode, IStreamingDicomServiceNode
    {
        internal StreamingDicomServiceNode(IStreamingServerApplicationEntity server)
            : base(server)
        {
            Platform.CheckExpectedType(server, typeof(IStreamingServerApplicationEntity));
        }

        #region Implementation of IStreamingServerApplicationEntity

        public int HeaderServicePort
        {
            get { return ((IStreamingServerApplicationEntity)Real).HeaderServicePort; }
        }

        public int WadoServicePort
        {
            get { return ((IStreamingServerApplicationEntity)Real).WadoServicePort; }
        }

        #endregion
    }

    internal class DicomServiceNode : ServiceNode, IDicomServiceNode
    {
        internal DicomServiceNode(DicomServerConfiguration localConfiguration)
        {
            Real = new DicomServerApplicationEntity(
                localConfiguration.AETitle, localConfiguration.HostName, localConfiguration.Port, "<local>", "", "");
            IsLocal = true;
        }

        internal DicomServiceNode(IDicomServerApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            Platform.CheckExpectedType(server, typeof(DicomServerApplicationEntity));

            Real = (DicomServerApplicationEntity) server;
        }

        protected DicomServerApplicationEntity Real { get; private set; }

        #region Implementation of IApplicationEntity
        
        public bool IsLocal { get; private set; }
        #endregion

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