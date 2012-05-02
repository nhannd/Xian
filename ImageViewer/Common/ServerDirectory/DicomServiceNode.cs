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
            Server = new ApplicationEntity(localConfiguration.AETitle, "<local>", "", "")
                       {
                           ScpParameters = new ScpParameters(localConfiguration.HostName, localConfiguration.Port)
                       };

            IsLocal = true;
            ExtendedData = new ServerData {IsPriorsServer = true};
        }

        internal DicomServiceNode(ServerDirectoryEntry directoryEntry)
        {
            Platform.CheckForNullReference(directoryEntry, "directoryEntry");
            Server = directoryEntry.Server;
            ExtendedData = directoryEntry.Data ?? new ServerData();
        }

        internal DicomServiceNode(IApplicationEntity applicationEntity)
            : this(new ServerDirectoryEntry(new ApplicationEntity(applicationEntity)))
        {
        }

        protected internal ServerData ExtendedData { get; set; }
        //TODO (Marmot): Don't hold on to it, just look it up via the directory?
        protected internal ApplicationEntity Server { get; private set; }

        #region Implementation of IDicomServiceNode

        public bool IsLocal { get; private set; }

        #endregion

        #region Implementation of IApplicationEntity

        public string AETitle
        {
            get { return Server.AETitle; }
        }

        public string Name
        {
            get { return Server.Name; }
        }

        public string Description
        {
            get { return Server.Description; }
        }

        public string Location
        {
            get { return Server.Location; }
        }

        public IScpParameters ScpParameters
        {
            get { return Server.ScpParameters; }
        }

        public IStreamingParameters StreamingParameters
        {
            get { return Server.StreamingParameters; }
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
            return Server.ToString();
        }
    }
}