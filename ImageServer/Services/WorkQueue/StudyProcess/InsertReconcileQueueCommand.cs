using System;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
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
            IInsertReconcileQueue broker = updateContext.GetBroker<IInsertReconcileQueue>();
            InsertReconcileQueueParameters parameters = new InsertReconcileQueueParameters();
            
            parameters.FilesystemKey = _context.StudyLocation.FilesystemKey;
            parameters.ServerPartitionKey = _context.StudyLocation.ServerPartitionKey;
            parameters.FilesystemKey = _context.StudyLocation.FilesystemKey;
            parameters.StudyInstanceUid = _context.StudyInstanceUid;
            parameters.StudyStorageKey = _context.StudyLocation.GetKey();
            parameters.SeriesInstanceUid = _context.SeriesInstanceUid;
            parameters.SopInstanceUid = _context.SopInstanceUid;
            
            if (_context.Partition.MatchAccesssionNumber)
                parameters.AccessionNumber = _context.AccessionNumber;
            if (_context.Partition.MatchIssuerOfPatientId)
                parameters.IssuerOfPatientId = _context.IssuerOfPatientId;
            if (_context.Partition.MatchPatientId)
                parameters.PatientId = _context.PatientId;
            if (_context.Partition.MatchPatientsBirthDate)
                parameters.PatientsBirthDate = _context.PatientsBirthDate;
            if (_context.Partition.MatchPatientsName)
                parameters.PatientsName = _context.PatientsName;
            if (_context.Partition.MatchPatientsSex)
                parameters.PatientsSex = _context.PatientsSex;
            
            _context.ReconcileRecord= broker.FindOne(parameters);

            if (_context.ReconcileRecord.InsertTime >= Platform.Time.Subtract(TimeSpan.FromSeconds(15)))
            {
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Informational, "StudyProcess", 10000,
                                     "Suspicious image was received. Manual reconciliation is required. Ref#={0}", _context.ReconcileRecord.GetKey().Key.ToString());
            }

        }
    }
}