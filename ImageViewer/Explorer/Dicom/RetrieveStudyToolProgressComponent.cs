using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="RetrieveStudyToolProgressComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RetrieveStudyToolProgressComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RetrieveStudyToolProgressComponent class
    /// </summary>
    [AssociateView(typeof(RetrieveStudyToolProgressComponentViewExtensionPoint))]
    public class RetrieveStudyToolProgressComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RetrieveStudyToolProgressComponent(ApplicationEntity retriever, ApplicationEntity server,
            Uid retrieveObjectUid, string dicomStoragePath)
        {
            this.Retriever = retriever;
            this.Server = server;
            this.RetrieveObjectUid = retrieveObjectUid;
            this.DicomStoragePath = dicomStoragePath;
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();

            using (DicomClient client = new DicomClient(this.Retriever))
            {
                client.SopInstanceReceived += new EventHandler<SopInstanceReceivedEventArgs>(OnSopInstanceReceived);
                //client.RetrieveProgressUpdated += delegate(object source, RetrieveProgressUpdatedEventArgs args)
                client.Retrieve(this.Server, this.RetrieveObjectUid, this.DicomStoragePath);
            }
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Properties
        private string _retrieveSourceDescription;
        private string _studyDescription;
        private string _progressText;
        private ApplicationEntity _retriever;
        private ApplicationEntity _server;
        private Uid _retrieveObjectUid;
        private string _dicomStoragePath;

        public string DicomStoragePath
        {
            get { return _dicomStoragePath; }
            private set { _dicomStoragePath = value; }
        }
	
        public Uid RetrieveObjectUid
        {
            get { return _retrieveObjectUid; }
            private set { _retrieveObjectUid = value; }
        }
	
        public ApplicationEntity Server
        {
            get { return _server; }
            private set { _server = value; }
        }
	
        public ApplicationEntity Retriever
        {
            get { return _retriever; }
            private set { _retriever = value; }
        }
	
        public string ProgressText
        {
            get { return _progressText; }
            set { _progressText = value; }
        }
	
        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }
	
        public string RetrieveSourceDescription
        {
            get { return _retrieveSourceDescription; }
            set { _retrieveSourceDescription = value; }
        }
	
        #endregion

        private void InsertSopInstance(string fileName)
        {
            DcmFileFormat file = new DcmFileFormat();
            OFCondition condition = file.loadFile(fileName);
            if (!condition.good())
            {
                // there was an error reading the file, possibly it's not a DICOM file
                return;
            }

            DcmMetaInfo metaInfo = file.getMetaInfo();
            DcmDataset dataset = file.getDataset();

            if (ConfirmProcessableFile(metaInfo, dataset))
            {
                IDicomPersistentStore dicomStore = DataAccessLayer.GetIDicomPersistentStore();
                dicomStore.InsertSopInstance(metaInfo, dataset, fileName);
                dicomStore.Flush();
            }

            // keep the file object alive until the end of this scope block
            // otherwise, it'll be GC'd and metaInfo and dataset will be gone
            // as well, even though they are needed in the InsertSopInstance
            // and sub methods
            GC.KeepAlive(file);
        }

        private bool ConfirmProcessableFile(DcmMetaInfo metaInfo, DcmDataset dataset)
        {
            StringBuilder stringValue = new StringBuilder(1024);
            OFCondition cond;
            cond = metaInfo.findAndGetOFString(Dcm.MediaStorageSOPClassUID, stringValue);
            if (cond.good())
            {
                // we want to skip Media Storage Directory Storage (DICOMDIR directories)
                if ("1.2.840.10008.1.3.10" == stringValue.ToString())
                    return false;
            }

            return true;
        }

        void OnSopInstanceReceived(object sender, SopInstanceReceivedEventArgs e)
        {
            InsertSopInstance(e.SopFileName);
        }

    }
}
