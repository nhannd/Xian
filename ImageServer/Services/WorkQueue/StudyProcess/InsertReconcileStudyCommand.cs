using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Command for inserting or updating 'ReconcileStudy' work queue entries.
    /// </summary>
    class InsertReconcileStudyCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly ReconcileImageContext _context;
        #endregion

        #region Constructors
        public InsertReconcileStudyCommand(ReconcileImageContext context)
            : base("InsertReconcileStudyCommand", true)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        }
        #endregion

        #region Overridden Protected Methods
        protected override void OnExecute(ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
        {
            Platform.CheckForNullReference(updateContext, "updateContext");
            Platform.CheckForNullReference(_context, "_context");
            
            WorkQueueSettings settings = WorkQueueSettings.Instance;
			IInsertWorkQueue broker = updateContext.GetBroker<IInsertWorkQueue>();
			InsertWorkQueueParameters parameters = new InsertWorkQueueParameters();
        	parameters.WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess;
            parameters.ServerPartitionKey = _context.Partition.GetKey();
            parameters.StudyStorageKey = _context.DestinationStudyLocation!=null? _context.DestinationStudyLocation.GetKey():_context.CurrentStudyLocation.GetKey();
            parameters.StudyHistoryKey = _context.History != null ? _context.History.GetKey() : null;
            parameters.SeriesInstanceUid = _context.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            parameters.SopInstanceUid = _context.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
			parameters.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;

            ReconcileStudyWorkQueueData data = new ReconcileStudyWorkQueueData();
            data.StoragePath = _context.TempStoragePath; 
            XmlDocument xmlData = new XmlDocument();
            XmlNode node = xmlData.ImportNode(XmlUtils.Serialize(data), true);
            xmlData.AppendChild(node);
            parameters.WorkQueueData = xmlData;

            DateTime now = Platform.Time;
            parameters.ScheduledTime = now;
            parameters.ExpirationTime = now.AddSeconds(settings.WorkQueueExpireDelaySeconds);
            if (!broker.Execute(parameters))
                throw new ApplicationException("Unable to insert ReconcileStudy work queue entry");

			_context.TempStoragePath = _context.TempStoragePath;

        }

        #endregion

    }
}
