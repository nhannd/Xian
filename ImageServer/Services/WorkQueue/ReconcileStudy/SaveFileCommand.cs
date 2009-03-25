using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Save a dicom file
    /// </summary>
    class SaveFileCommand : ServerCommand<ReconcileStudyProcessorContext, DicomFile>
    {
        private ServerCommandProcessor _processor;

        public SaveFileCommand(ReconcileStudyProcessorContext context, DicomFile file)
            : base("SaveFileCommand", true, context, file)
        {
        }

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context.DestStorageLocation, "Context.DestStorageLocation");
            DicomFile file = Parameters;
            String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

            String destPath = Path.Combine(Context.DestStorageLocation.GetStudyPath(), seriesInstanceUid);
            destPath = Path.Combine(destPath, sopInstanceUid);
            String extension = "dcm";
            destPath += "."+ extension;

            _processor = new ServerCommandProcessor("SaveFileCommand Processor");
        
            if (File.Exists(destPath))
            {
                #region Duplicate SOP

                // TODO: Add code to handle duplicate sop here
                Platform.Log(LogLevel.Warn, "Image {0} cannot be processed because of duplicate in {1}", sopInstanceUid, destPath);
                
                #endregion

                return;
            }

            _processor.AddCommand(new SaveDicomFileCommand(destPath, file, false));

            if (!_processor.Execute())
            {
                throw new ApplicationException(_processor.FailureReason);
            }
            
        }

        protected override void OnUndo()
        {
            if (_processor!=null)
            {
                _processor.Rollback();
                _processor = null;
            }
        }
    }
}