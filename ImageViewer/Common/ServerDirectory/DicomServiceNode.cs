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

        //TODO (Marmot): Don't hold on to it, just look it up via the directory?
        protected ApplicationEntity Real { get; private set; }

        #region Implementation of IDicomServiceNode

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
            get { return Real.Location; }
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
            if (typeof(T) == typeof(IStudyStoreQuery))
                return IsLocal && StudyStore.IsSupported;

            if (typeof(T) == typeof(IStudyRootQuery))
                return IsLocal || ScpParameters != null;

            return base.IsSupported<T>();
        }

        public override T GetService<T>()
        {
            if (typeof(T) == typeof(IStudyStoreQuery) && IsLocal)
                return Platform.GetService<IStudyStoreQuery>() as T;

            if (typeof(T) == typeof(IStudyRootQuery))
            {
                if (IsLocal)
                    return Platform.GetService<IStudyStoreQuery>() as T;

                return new DicomStudyRootQuery(DicomServerConfigurationHelper.AETitle,
                                    AETitle, ScpParameters.HostName, ScpParameters.Port) as T;
            }

            return base.GetService<T>();
        }

        public override string ToString()
        {
            return Real.ToString();
        }
    }
}