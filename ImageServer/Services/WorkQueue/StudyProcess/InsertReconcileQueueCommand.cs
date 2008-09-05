using System;
using System.Diagnostics;
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
            Platform.CheckForNullReference(_context.StudyLocation, "_context.StudyLocation");
            Platform.CheckForNullReference(_context.File, "_context.File");

            ImageSetDescriptor desc = ImageSetDescriptor.Parse(_context.File);
            XmlDocument descXml = new XmlDocument();
            descXml.AppendChild(descXml.ImportNode(XmlUtils.Serialize(desc), true));
            

            IInsertReconcileQueue broker = updateContext.GetBroker<IInsertReconcileQueue>();
            InsertReconcileQueueParameters parameters = new InsertReconcileQueueParameters();
            parameters.ServerPartitionKey = _context.Partition.GetKey();
            parameters.StudyStorageKey = _context.StudyLocation.GetKey();
            parameters.ReconcileReasonEnum = ReconcileReasonEnum.InconsistentData;
            parameters.SeriesInstanceUid = _context.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            parameters.SopInstanceUid = _context.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            parameters.StudyData = descXml;

            ReconcileQueue item = broker.FindOne(parameters);
            if (item==null)
            {
                throw new ApplicationException("Unable to update reconcile queue");
            }

            _context.ReconcileQueue = item;
        }
    }
}