using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

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
            if (IsLocal)
            {
                if (typeof(T) == typeof(IWorkItemService) && WorkItemActivityMonitor.IsSupported)
                    return true;

                if (typeof(T) == typeof(IStudyStoreQuery) && StudyStore.IsSupported)
                    return true;

                if (typeof(T) == typeof(IStudyRootQuery) && StudyStore.IsSupported)
                    return true;
            }
            else
            {
                if (typeof(T) == typeof(IStudyRootQuery))
                    return ScpParameters != null;
            }

            return base.IsSupported<T>();
        }

        public override T GetService<T>()
        {
            //TODO (Marmot): Is this weird??
            if (IsLocal)
            {
                if (typeof(T) == typeof(IWorkItemService) && WorkItemActivityMonitor.IsSupported)
                    return Platform.GetService<IWorkItemService>() as T;

                if (typeof(T) == typeof(IStudyStoreQuery) && StudyStore.IsSupported)
                    return Platform.GetService<IStudyStoreQuery>() as T;

                if (typeof(T) == typeof(IStudyRootQuery) && StudyStore.IsSupported)
                    return new StoreStudyRootQuery() as T;
            }
            else
            {
                if (typeof(T) == typeof(IStudyRootQuery) && ScpParameters != null)
                    return new DicomStudyRootQuery(DicomServer.DicomServer.AETitle, this) as T;
            }

            return base.GetService<T>();
        }

        public override string ToString()
        {
            return Real.ToString();
        }
    }
}