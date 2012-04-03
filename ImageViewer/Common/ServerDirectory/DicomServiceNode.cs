using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    internal class DicomServiceNode : ServiceNode, IDicomServiceNode
    {
        internal DicomServiceNode(DicomServerConfiguration localConfiguration)
        {
            Real = new ApplicationEntity(localConfiguration.AETitle, "<local>", "", "")
                       {
                           ScpParameters = new ScpParameters(localConfiguration.HostName, localConfiguration.Port)
                       };

            IsLocal = true;
        }

        internal DicomServiceNode(IApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            Real = (ApplicationEntity) server;
        }

        protected ApplicationEntity Real { get; private set; }

        #region Implementation of IDicomServiceNode

        public bool IsLocal { get; private set; }
        public bool SupportsStreaming { get { return StreamingParameters != null; } }
        
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

        public IScpParameters ScpParameters
        {
            get { return Real.ScpParameters; }
        }

        public IStreamingParameters StreamingParameters
        {
            get { return Real.StreamingParameters; }
        }

        #endregion

        public override bool IsSupported<T>()
        {
            if (typeof(T) == typeof(IStudyStore))
                return IsLocal && StudyStore.IsSupported;

            if (typeof(T) == typeof(IStudyRootQuery))
                return IsLocal || ScpParameters != null;

            //if (typeof(T).Equals(typeof(IHeaderStreamingService)))
            //    return StreamingParameters != null;

            return false;
        }

        protected override T GetService<T>()
        {
            if (!IsSupported<T>())
                throw new NotSupportedException(String.Format("DICOM Service node doesn't support service '{0}'", typeof(T).FullName));

            if (typeof(T) == typeof(IStudyStore) && IsLocal)
                return Platform.GetService<IStudyStore>() as T;

            //TODO (Marmot): Add an extension mechanism.
            if (typeof(T) == typeof(IStudyRootQuery))
            {
                if (IsLocal)
                    return Platform.GetService<IStudyStore>() as T;

                return new DicomStudyRootQuery(DicomServerConfigurationHelper.AETitle,
                                    AETitle, ScpParameters.HostName, ScpParameters.Port) as T;
            }

            throw new NotSupportedException(String.Format("DICOM Service node doesn't support service '{0}'", typeof(T).FullName));
            //if (typeof(T).Equals(typeof(IHeaderStreamingService)))
            //    return new HeaderStreamingClient
        }
    }
}