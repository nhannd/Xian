using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    

    /// <summary>
    /// Command for inserting a Reconcile Queue entry for a dicom file.
    /// </summary>
    class InsertReconcileQueueCommand : ServerDatabaseCommand
    {
        private readonly ReconcileImageContext _context;

        public InsertReconcileQueueCommand(ReconcileImageContext context)
            : base("InsertReconcileQueueCommand", true)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        }

        protected override void OnExecute(ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.Partition, "_context.Partition");
            Platform.CheckForNullReference(_context.CurrentStudyLocation, "_context.CurrentStudyLocation");
            Platform.CheckForNullReference(_context.File, "_context.File");

            ImageSetDescriptor desc = ImageSetDescriptor.Parse(_context.File);
            XmlDocument descXml = new XmlDocument();
            descXml.AppendChild(descXml.ImportNode(XmlUtils.Serialize(desc), true));

            ReconcileStudyQueueDescription queueDesc = GetQueueEntryDescription();

            IInsertStudyIntegrityQueue broker = updateContext.GetBroker<IInsertStudyIntegrityQueue>();
            InsertStudyIntegrityQueueParameters parameters = new InsertStudyIntegrityQueueParameters();
            parameters.Description = queueDesc.ToString();
            parameters.StudyInstanceUid = _context.CurrentStudyLocation.StudyInstanceUid;
            parameters.ServerPartitionKey = _context.Partition.GetKey();
            parameters.StudyStorageKey = _context.CurrentStudyLocation.GetKey();
            parameters.StudyIntegrityReasonEnum = StudyIntegrityReasonEnum.InconsistentData;
            parameters.SeriesInstanceUid = _context.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            parameters.SeriesDescription = _context.File.DataSet[DicomTags.SeriesDescription].GetString(0, String.Empty);
            parameters.SopInstanceUid = _context.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            parameters.StudyData = descXml;

            StudyIntegrityQueue item = broker.FindOne(parameters);
            if (item==null)
            {
                throw new ApplicationException("Unable to update reconcile queue");
            }

            _context.ReconcileQueue = item;
            if (item.QueueData == null)
            {
                // this is new entry, need to assign the path and update the entry
                _context.StoragePath = GetInitialReconcileStoragePath();
                
                ReconcileStudyWorkQueueData data = new ReconcileStudyWorkQueueData();
                data.StoragePath = _context.StoragePath;
                XmlDocument xmlQueueData = new XmlDocument();
                xmlQueueData.AppendChild(xmlQueueData.ImportNode(XmlUtils.Serialize(data), true));

                item.QueueData = xmlQueueData;

                IStudyIntegrityQueueEntityBroker updateBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                updateBroker.Update(item);
            }
            else
            {
            	// Need to re-use the path that's already assigned for this entry
                ReconcileStudyWorkQueueData data = XmlUtils.Deserialize < ReconcileStudyWorkQueueData>(item.QueueData);
                _context.StoragePath = data.StoragePath;
            }
        }

        private string GetInitialReconcileStoragePath()
        {
            String path =
                Path.Combine(_context.CurrentStudyLocation.FilesystemPath, _context.CurrentStudyLocation.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, Guid.NewGuid().ToString());
            return path;
        }

        private ReconcileStudyQueueDescription GetQueueEntryDescription()
        {
            ReconcileStudyQueueDescription desc = new ReconcileStudyQueueDescription();
            desc.ExistingPatientId = _context.CurrentStudy.PatientId;
            desc.ExistingPatientName = _context.CurrentStudy.PatientsName;
            desc.ExistingAccessionNumber = _context.CurrentStudy.AccessionNumber;
            desc.ConflictingPatientName = _context.File.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
            desc.ConflictingPatientId = _context.File.DataSet[DicomTags.PatientId].GetString(0, String.Empty);
            desc.ConflictingAccessionNumber = _context.File.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty);
            
            return desc;
        }
    }
}