using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Services;
using ClearCanvas.ImageViewer.StudyManagement;

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
        public RetrieveStudyToolProgressComponent(ApplicationEntity retriever, string dicomStoragePath)
        {
            this.Retriever = retriever;
            this.DicomStoragePath = dicomStoragePath;
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        /// <summary>
        /// Public API function that allows the client to specify a 
        /// list of studies that the client wants to retrieve, 
        /// possibly from different servers.
        /// </summary>
        /// <param name="listOfStudyItems">List of study items</param>
        public void Retrieve(IEnumerable<StudyItem> listOfStudyItems)
        {
            if (null == this.BackgroundWorker)
                this.BackgroundWorker = new BackgroundWorker();

            // use an IEnumerator<T> since we can't use foreach here
            this.ListOfStudyItemsToRetrieve = listOfStudyItems;
            this.StudyItemListEnumerator = listOfStudyItems.GetEnumerator();
            this.StudyItemListEnumerator.MoveNext();

            // calculate study count for progress display purpose
            this.CurrentStudyCount = 1;
            this.TotalStudyCount = 0;
            foreach (StudyItem item in listOfStudyItems)
                ++this.TotalStudyCount;

            this.TotalObjectsReceived = 0;

            InternalRetrieve(this.StudyItemListEnumerator.Current);
        }

        /// <summary>
        /// Utility function that does the actual work of 
        /// setting up the backgroundworker object/thread.
        /// This function can be called multiple times, once
        /// for each study that has to be retrieved.
        /// </summary>
        /// <param name="item">The study item that the caller wants to retrieve</param>
        private void InternalRetrieve(StudyItem item)
        {
            // set up the databinding properties
            this.AETitle = item.Server.AE;
            this.Host = item.Server.Host;
            this.Port = Convert.ToString(item.Server.Port);
            this.Patient = item.FirstName + " " + item.LastName;
            this.Description = item.StudyDescription;
            this.StudyDate = DicomHelper.GetDateStringFromDicomDA(item.StudyDate);

            // set up the overall progress counter display
            this.ProgressGroupBox = "Retrieving Study #" + 
                Convert.ToString(this.CurrentStudyCount++) + 
                " of " + Convert.ToString(this.TotalStudyCount);

            // titles for the group boxes are set appropriately here,
            // but reset when the all the retrieval tasks are completed
            this.StudyDescriptionGroupBox = "Current study being retrieved";
            this.RetrieveSourceGroupBox = "Current retrieval server";

            SignalDetailsChanged();

            BackgroundWorkerInitialize();
            this.BackgroundWorker.RunWorkerAsync(item);
        }

        private void SignalDetailsChanged()
        {
            NotifyPropertyChanged("AETitle");
            NotifyPropertyChanged("Host");
            NotifyPropertyChanged("Port");
            NotifyPropertyChanged("Location");
            NotifyPropertyChanged("Patient");
            NotifyPropertyChanged("Description");
            NotifyPropertyChanged("StudyDate");
            NotifyPropertyChanged("ProgressGroupBox");
            NotifyPropertyChanged("RetrieveSourceGroupBox");
            NotifyPropertyChanged("StudyDescriptionGroupBox");
        }

        private void BackgroundWorkerInitialize()
        {
            this.BackgroundWorker.WorkerReportsProgress = true;
            this.BackgroundWorker.WorkerSupportsCancellation = false;
            this.BackgroundWorker.DoWork += new DoWorkEventHandler(OnRetrieveTasked);
            this.BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnRunWorkerCompleted);
            this.BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
        }

        /// <summary>
        /// When the backgroundworker object's run is completed,
        /// we need to make sure that the object's state is
        /// properly reset. If there are additional studies to
        /// retrieve, the enumerator has to be incremented, or 
        /// if not, the various Group Box titles have to be 
        /// reset to something more appropriate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // reset the backgroundworker object
            this.BackgroundWorker.WorkerReportsProgress = false;
            this.BackgroundWorker.WorkerSupportsCancellation = false;
            this.BackgroundWorker.DoWork -= new DoWorkEventHandler(OnRetrieveTasked);
            this.BackgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(OnRunWorkerCompleted);
            this.BackgroundWorker.ProgressChanged -= new ProgressChangedEventHandler(OnProgressChanged);

            // advance the enumerator of the list of studies that we have to retrieve
            if (this.StudyItemListEnumerator.MoveNext())
            {
                StudyItem item = this.StudyItemListEnumerator.Current;
                InternalRetrieve(item);
            }
            else
            {
                // there are no more study items to retrieve
                this.ProgressGroupBox = "Retrievals completed";
                this.RetrieveSourceGroupBox = "Last retrieval server used";
                this.StudyDescriptionGroupBox = "Last study retrieved";
                SignalDetailsChanged();
                SignalRetrievalComplete();
            }
        }

        private void SignalRetrievalComplete()
        {
            EventsHelper.Fire(AllRetrievalTasksCompleted, this, EventArgs.Empty);
        }

        /// <summary>
        /// Event handler hooked up to the backgroundworker object.
        /// This event handler is triggered by the object itself, not
        /// by our component. This triggering is initiated from a call to
        /// backgroundworker object's ReportProgress, which then asynchronously
        /// fires the event.
        /// 
        /// Whenever we receive a callback call from DicomClient letting us know
        /// that there is new retrieval progress data, i.e. from  C-MOVE-RSP, 
        /// we will use BackgroundWorker.ReportProgress() to asynchronously trigger
        /// an update of the UI, via OnProgressChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RetrieveProgressUpdatedEventArgs args = e.UserState as RetrieveProgressUpdatedEventArgs;

            this.ProgressBarMinimum = 1;
            this.ProgressBarMaximum = args.CompletedSuboperations + args.FailedSuboperations + args.RemainingSuboperations;
            this.ProgressBar = args.CompletedSuboperations + args.FailedSuboperations;
            this.ProgressDetails = "[" + e.ProgressPercentage + "%]" + " Remaining: " + args.RemainingSuboperations +
                " Completed: " + args.CompletedSuboperations + " Failed: " + args.FailedSuboperations;

            // signal that databinding properties have changed, 
            // so that the control may then update itself
            SignalProgressChanged();
        }

        private void SignalProgressChanged()
        {
            NotifyPropertyChanged("ProgressBar");
        }

        /// <summary>
        /// Hooked up to BackgroundWorker.DoWork event. This function
        /// actually does the work of initiating a retrieve using DicomClient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Current used to hold an object of StudyItem</param>
        private void OnRetrieveTasked(object sender, DoWorkEventArgs e)
        {
            StudyItem item = e.Argument as StudyItem;

            using (DicomClient client = new DicomClient(this.Retriever))
            {
                client.SopInstanceReceived += new EventHandler<SopInstanceReceivedEventArgs>(OnSopInstanceReceived);
                client.RetrieveProgressUpdated += new EventHandler<RetrieveProgressUpdatedEventArgs>(OnRetrieveProgressUpdated);
                client.Retrieve(item.Server, new Uid(item.StudyInstanceUID), this.DicomStoragePath);
            }
        }

        #region Properties

        private ApplicationEntity _retriever;
        private string _dicomStoragePath;
        private BackgroundWorker _backgroundWorker;
        private IEnumerator<StudyItem> _studyItemListEnumerator;
        private IEnumerable<StudyItem> _listOfStudyItemsToRetrieve;
        private int _totalStudyCount;
        private int _currentStudyCount;
        // databinding properties
        private string _aeTitle;
        private string _host;
        private string _port;
        private string _patient;
        private string _studyDate;
        private string _description;
        private string _progressGroupBox;
        private int _progressBar;
        private int _progressBarMinimum;
        private int _progressBarMaximum;
        private string _retrieveSourceGroupBox;
        private string _studyDescriptionGroupBox;
        private string _progressDetails;
        private int _totalObjectsReceived;

        public int TotalObjectsReceived
        {
            get { return _totalObjectsReceived; }
            set { _totalObjectsReceived = value; }
        }
	
        public string ProgressDetails
        {
            get { return _progressDetails; }
            set { _progressDetails = value; }
        }
	

        public string StudyDescriptionGroupBox
        {
            get { return _studyDescriptionGroupBox; }
            set { _studyDescriptionGroupBox = value; }
        }
	
        public string RetrieveSourceGroupBox
        {
            get { return _retrieveSourceGroupBox; }
            set { _retrieveSourceGroupBox = value; }
        }
	

        private int CurrentStudyCount
        {
            get { return _currentStudyCount; }
            set { _currentStudyCount = value; }
        }

        private int TotalStudyCount
        {
            get { return _totalStudyCount; }
            set { _totalStudyCount = value; }
        }
	
        public int ProgressBarMaximum
        {
            get { return _progressBarMaximum; }
            set { _progressBarMaximum = value; }
        }
	
        public int ProgressBarMinimum
        {
            get { return _progressBarMinimum; }
            set { _progressBarMinimum = value; }
        }
	
        public int ProgressBar
        {
            get { return _progressBar; }
            set { _progressBar = value; }
        }
	
	    public string ProgressGroupBox
	    {
		    get { return _progressGroupBox;}
		    set { _progressGroupBox = value;}
	    }
	
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
	
        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }
	
        public string Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }
	    
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }
	
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }
	
        public string AETitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }
	
        private IEnumerable<StudyItem> ListOfStudyItemsToRetrieve
        {
            get { return _listOfStudyItemsToRetrieve; }
            set { _listOfStudyItemsToRetrieve = value; }
        }
	
        private IEnumerator<StudyItem> StudyItemListEnumerator
        {
            get { return _studyItemListEnumerator; }
            set { _studyItemListEnumerator = value; }
        }
	
        private BackgroundWorker BackgroundWorker
        {
            get { return _backgroundWorker; }
            set { _backgroundWorker = value; }
        }
	

        public string DicomStoragePath
        {
            get { return _dicomStoragePath; }
            private set { _dicomStoragePath = value; }
        }
	
        public ApplicationEntity Retriever
        {
            get { return _retriever; }
            private set { _retriever = value; }
        }
	
        #endregion
        #region Published Events
        public event EventHandler AllRetrievalTasksCompleted;
        #endregion

        /// <summary>
        /// Hooked up to DicomClient.RetrieveProgressUpdated event.
        /// This will then compiled the C-MOVE-RSP statistics into
        /// an argument to use for calling BackgroundWorker.ReportProgress().
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnRetrieveProgressUpdated(object source, RetrieveProgressUpdatedEventArgs args)
        {
            double percentageComplete = ((double) (args.CompletedSuboperations + args.FailedSuboperations) /
                (args.FailedSuboperations + args.CompletedSuboperations + args.RemainingSuboperations)) * 100;

            this.BackgroundWorker.ReportProgress(
                Convert.ToInt32(percentageComplete),
                args
                );
        }

        /// <summary>
        /// Indirectly hooked up to DicomClient.SopInstanceReceived to 
        /// store the instance object when it arrives.
        /// </summary>
        /// <param name="fileName">Path to the file</param>
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

        /// <summary>
        /// Determine various characteristics to see whether we can actually
        /// store this file. For retrievals this should never be a problem. For
        /// DatabaseRebuild, sometimes objects are stored without their Group 2
        /// tags, which makes them impossible to process, i.e. we'd have to guess
        /// correctly the transfer syntax.
        /// </summary>
        /// <param name="metaInfo">Group 2 (metaheader) tags</param>
        /// <param name="dataset">DICOM header</param>
        /// <returns></returns>
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

        /// <summary>
        /// Hooked up to DicomClient.OnSopInstanceReceived.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSopInstanceReceived(object sender, SopInstanceReceivedEventArgs e)
        {
            InsertSopInstance(e.SopFileName);
            ++this.TotalObjectsReceived;
            NotifyPropertyChanged("TotalObjectsReceived");
        }
    }
}
