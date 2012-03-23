using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Dicom.Core.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Processor for Sop Instances being inserted into the database.
    /// </summary>
    public class SopInstanceProcessor
    {        
        #region Public Properties
        
        public StudyLocation StudyLocation { get; private set; }

        #endregion

        #region Constructors

        public SopInstanceProcessor(StudyLocation location)
        {
            Platform.CheckForNullReference(location, "location");
            StudyLocation = location;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Process a specific DICOM file related to a <see cref="WorkItem"/> request.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On success and if <see cref="uid"/> is set, the <see cref="WorkItemUid"/> field is marked as complete.
        /// </para>
        /// </remarks>
        /// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
        /// <param name="file">The file to process.</param>
        /// <param name="uid">An optional WorkQueueUid associated with the entry, that will be deleted upon success or failed on failure.</param>
        /// <exception cref="ApplicationException"/>
        /// <exception cref="DicomDataException"/>
        public void ProcessFile(DicomFile file, StudyXml stream, WorkItemUid uid)
        {
            Platform.CheckForNullReference(file, "file");

            InsertInstance(file, stream, uid);
        }

        #endregion

        #region Private Methods

        private void InsertInstance(DicomFile file, StudyXml studyXml, WorkItemUid uid)
        {
            using (var processor = new ViewerCommandProcessor("Processing WorkItem DICOM file"))
            {
                String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
                
                try
                {
                    String seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                    String sopUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                    String finalDest = StudyLocation.GetSopInstancePath(seriesUid, sopUid);

                    if (file.Filename != finalDest)
                    {
                        // Duplicate handler here

                        // Have to be careful here about failure on exists vs. not failing on exists
                        // because of the different use cases of the importer.
                        // save the file in the study folder and remove the source filename
                        processor.AddCommand(new SaveDicomFileCommand(finalDest, file, false));

                        processor.AddCommand(new FileDeleteCommand(file.Filename, true));
                    }

                    // Update the StudyStream object
                    var insertStudyXmlCommand = new InsertStudyXmlCommand(file, studyXml, StudyLocation, true);
                    processor.AddCommand(insertStudyXmlCommand);

                    // Insert into the database, but only if its not a duplicate so the counts don't get off
                    var insertInstanceCommand = new InsertOrUpdateStudyCommand(file, studyXml);
                    processor.AddCommand(insertInstanceCommand);

                    if (uid != null)
                        processor.AddCommand(new CompleteWorkItemUidCommand(uid));

                    // Do the actual processing
                    if (!processor.Execute())
                    {
                        Platform.Log(LogLevel.Error, "Failure processing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
                        Platform.Log(LogLevel.Error, "File that failed processing: {0}", file.Filename);
                        throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid, processor.FailureException);
                    }
                    Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
                                 processor.Description);
                    processor.Rollback();
                    throw new ApplicationException("Unexpected exception when processing file.", e);
                }
            }
        }
        #endregion
    }
}
