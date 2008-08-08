using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReprocessStudy
{
    /// <summary>
    /// Information about the study being reprocessed
    /// </summary>
    class StudyInfo
    {
        #region Private Members
        private string _studyInstanceUid;
        private Dictionary<string, SeriesInfo> _series = new Dictionary<string, SeriesInfo>();
        #endregion

        #region Constructors
        
        public StudyInfo(string studyInstanceUid)
        {
            _studyInstanceUid = studyInstanceUid;
        }

        #endregion

        #region Public Properties

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public SeriesInfo this[string seriesUid]
        {
            get
            {
                if (!_series.ContainsKey(seriesUid))
                {
                    return null;
                }

                return _series[seriesUid];
            }
            set
            {
                if (_series.ContainsKey(seriesUid))
                    _series[seriesUid] = value;
                else
                    _series.Add(seriesUid, value);
            }
        }

        public IEnumerable<SeriesInfo> Series
        {
            get { return _series.Values; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a series with specified series instance uid into the study
        /// </summary>
        /// <param name="seriesUid"></param>
        /// <returns></returns>
        public SeriesInfo AddSeries(string seriesUid)
        {
            SeriesInfo series = new SeriesInfo(seriesUid);
            _series.Add(seriesUid, series);

            return series;
        }

        #endregion
    }

    /// <summary>
    /// Information about the series of the study being reprocessed
    /// </summary>
    class SeriesInfo
    {
        private string _seriesUid;
        private Dictionary<string, SopInfo> _sopInstances = new Dictionary<string, SopInfo>();

        public SeriesInfo(string seriesUid)
        {
            _seriesUid = seriesUid;
        }

        public string SeriesUid
        {
            get { return _seriesUid; }
            set { _seriesUid = value; }
        }

        public SopInfo this[string sopUid]
        {
            get
            {
                if (_sopInstances.ContainsKey(sopUid))
                    return _sopInstances[sopUid];
                else
                    return null;
            }
            set
            {
                if (_sopInstances.ContainsKey(sopUid))
                    _sopInstances[sopUid] = value;
                else
                    _sopInstances.Add(sopUid, value);
            }
        }


        public IEnumerable<SopInfo> Instances
        {
            get { return _sopInstances.Values; }
        }

        public SopInfo AddInstance(string instanceUid)
        {
            SopInfo sop = new SopInfo(instanceUid);
            _sopInstances.Add(instanceUid, sop);

            return sop;
        }
    }

    /// <summary>
    /// Information about the sop instance being reprocessed.
    /// </summary>
    class SopInfo
    {
        #region Private Members
        private string _sopInstanceUid;
        #endregion

        #region Constructors
        public SopInfo(string instanceUid)
        {
            _sopInstanceUid = instanceUid;
        }
        #endregion

        #region Public Properties

        public string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }

        #endregion

    }

    /// <summary>
    /// Represents the context of the Study Reprocess operation
    /// </summary>
    class ReprocessStudyContext
    {
        #region Private Members
        private Model.WorkQueue _item;
        private IUpdateContext _updateContext;
        #endregion

        #region Public Properties
        public IUpdateContext UpdateContext
        {
            get { return _updateContext; }
            set { _updateContext = value; }
        }

        public Model.WorkQueue Item
        {
            get { return _item; }
            set { _item = value; }
        }
        #endregion

    }

    class ReprocessStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private StudyInfo _studyInfo;
        private StudyStorageLocation _newStorage;
        private readonly IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
        
        #endregion

        #region Overrriden Protected Methods
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.StudyStorageKey, "item.StudyStorageKey");

            
            LoadStorageLocation(item);
            
            using(IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ReprocessStudyContext context = new ReprocessStudyContext();
                context.UpdateContext = ctx;
                context.Item = item;

                CleanupDatabase(context);
                CleanupDirectory();
                ScheduleReprocess(context);

                ctx.Commit();
            }
        }

        #endregion

        #region Private Methods

        private void CleanupDirectory()
        {
            _studyInfo = new StudyInfo(StorageLocation.StudyInstanceUid);

            FileProcessor.Process(StorageLocation.GetStudyPath(), "*.*", 
                delegate(string file)
                    {
                            
                        if (Path.GetExtension(file) == ".dcm")
                        {
                            DicomFile dcmFile = new DicomFile(file);
                            dcmFile.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);


                            string seriesUid = dcmFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
                            string SopInstanceUid = dcmFile.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
                            if (_studyInfo[seriesUid]!=null)
                            {
                                _studyInfo[seriesUid].AddInstance(SopInstanceUid);
                            }
                            else
                            {
                                SeriesInfo series = _studyInfo.AddSeries(seriesUid);
                                series.AddInstance(SopInstanceUid);
                            
                            }
                        }
                        else
                        {
                            // only keep .dcm files
                            File.Delete(file);
                        }

                    }, true);

        }

        
        private void CleanupDatabase(ReprocessStudyContext context)
        {
            IDeleteStudyStorage broker = context.UpdateContext.GetBroker<IDeleteStudyStorage>();
            DeleteStudyStorageParameters parms = new DeleteStudyStorageParameters();

            parms.ServerPartitionKey = context.Item.ServerPartitionKey;
            parms.StudyStorageKey = context.Item.StudyStorageKey;
            if (!broker.Execute(parms))
                throw new ApplicationException("Cannot cleanup database");

            // create new study storage
            IInsertStudyStorage insertBroker = context.UpdateContext.GetBroker<IInsertStudyStorage>();
            StudyStorageInsertParameters insertParams = new StudyStorageInsertParameters();
            insertParams.FilesystemKey = StorageLocation.FilesystemKey;
            insertParams.Folder = StorageLocation.StudyFolder;
            insertParams.ServerPartitionKey = StorageLocation.ServerPartitionKey;
            insertParams.StudyInstanceUid = StorageLocation.StudyInstanceUid;
            insertParams.StudyStatusEnum = StorageLocation.StudyStatusEnum;
            insertParams.TransferSyntaxUid = StorageLocation.TransferSyntaxUid;

            IList<StudyStorageLocation> storages = insertBroker.Execute(insertParams);
           
            if (storages==null || storages.Count==0)
                throw new ApplicationException("Cannot update database");

            Platform.CheckTrue(storages.Count == 1, "Storage Location = 1");

            _newStorage = storages[0];
        }

        private void ScheduleReprocess(ReprocessStudyContext context)
        {
            IInsertWorkQueueStudyProcess broker = context.UpdateContext.GetBroker<IInsertWorkQueueStudyProcess>();
                    
            foreach(SeriesInfo series in _studyInfo.Series)
            {
                foreach(SopInfo sop in series.Instances)
                {
                    WorkQueueStudyProcessInsertParameters parms = new WorkQueueStudyProcessInsertParameters();
                    parms.Duplicate = false;
                    parms.ExpirationTime = Platform.Time.Add(TimeSpan.FromSeconds(WorkQueueSettings.Default.WorkQueueExpireDelaySeconds));
                    parms.Extension = ".dcm";
                    parms.ScheduledTime = Platform.Time;
                    parms.SeriesInstanceUid = series.SeriesUid;
                    parms.ServerPartitionKey = _newStorage.ServerPartitionKey;
                    parms.SopInstanceUid = sop.SopInstanceUid;
                    parms.StudyStorageKey = _newStorage.GetKey();
                    parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.Medium;
                    broker.Execute(parms);
                }
            }
        }

        #endregion

    }
}
