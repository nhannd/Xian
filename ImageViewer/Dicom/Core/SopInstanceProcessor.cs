using System;
using System.Collections.Generic;
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
        #region Subclass
        public class ProcessorFile
        {
            public ProcessorFile(DicomFile file, WorkItemUid uid)
            {
                File = file;
                ItemUid = uid;
            }

            public DicomFile File { get; set; }
            public WorkItemUid ItemUid { get; set; }
        }
        #endregion

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
            var processFile = new ProcessorFile(file, uid);
            InsertBatch(new List<ProcessorFile> {processFile}, stream);
        }

        public void ProcessBatch(IList<ProcessorFile> list, StudyXml stream )
        {
            Platform.CheckTrue(list.Count > 0,"list");
            InsertBatch(list, stream);
        }

        #endregion

        #region Private Methods

        private void InsertBatch(IList<ProcessorFile> list, StudyXml studyXml)
        {
            using (var processor = new ViewerCommandProcessor("Processing WorkItem DICOM file(s)"))
            {
                try
                {
                    foreach (var file in list)
                    {
                        String seriesUid = file.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                        String sopUid = file.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                        String finalDest = StudyLocation.GetSopInstancePath(seriesUid, sopUid);

                        if (file.File.Filename != finalDest)
                        {
                            // Duplicate handler here

                            // Have to be careful here about failure on exists vs. not failing on exists
                            // because of the different use cases of the importer.
                            // save the file in the study folder and remove the source filename
                            processor.AddCommand(new SaveDicomFileCommand(finalDest, file.File, false));

                            processor.AddCommand(new FileDeleteCommand(file.File.Filename, true));
                        }

                        // Update the StudyStream object
                        var insertStudyXmlCommand = new InsertStudyXmlCommand(file.File, studyXml, StudyLocation, false);
                        processor.AddCommand(insertStudyXmlCommand);

                        // Insert into the database, but only if its not a duplicate so the counts don't get off
                        var insertStudyCommand = new InsertOrUpdateStudyCommand(file.File, studyXml);
                        processor.AddCommand(insertStudyCommand);

                        if (file.ItemUid != null)
                            processor.AddCommand(new CompleteWorkItemUidCommand(file.ItemUid));
                    }

                    // Now save the batched updates to the StudyXml file.
                    processor.AddCommand(new SaveStudyXmlCommand(studyXml, StudyLocation));

                    // Do the actual processing
                    if (!processor.Execute())
                    {
                        Platform.Log(LogLevel.Error, "Failure processing {0} for Study: {1}",
                                     processor.Description, StudyLocation.Study.StudyInstanceUid);
                        throw new ApplicationException(
                            "Unexpected failure (" + processor.FailureReason + ") executing command for Study: " +
                            StudyLocation.Study.StudyInstanceUid, processor.FailureException);
                    }
                    Platform.Log(LogLevel.Info, "Processed {0} SOPs for Study {1}", list.Count,StudyLocation.Study.StudyInstanceUid );

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
