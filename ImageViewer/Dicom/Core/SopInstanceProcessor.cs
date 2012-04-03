#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// Represents a file to be processed by <see cref="SopInstanceProcessor"/>
        /// </summary>
        public class ProcessorFile
        {
            public ProcessorFile(DicomFile file, WorkItemUid uid)
            {
                File = file;
                ItemUid = uid;
            }

            public ProcessorFile(string path, WorkItemUid uid)
            {
                FilePath = path;
                ItemUid = uid;
            }

            /// <summary>
            /// Path to the <see cref="DicomFile"/> to process.  Can be used instead of <see cref="File"/>.
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// The DICOM File to process.  Can be used instead of <see cref="FilePath"/>.
            /// </summary>
            public DicomFile File { get; set; }

            /// <summary>
            /// An optional <see cref="WorkItemUid"/> associated with the file to be processed.  Will be updated appropriately.
            /// </summary>
            public WorkItemUid ItemUid { get; set; }
        }
        #endregion

        #region Public Properties
        
        /// <summary>
        /// The <see cref="StudyLocation"/> for the study being processed.
        /// </summary>
        public StudyLocation StudyLocation { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that all SOP Instances processed must be from the same study.
        /// </para>
        /// </remarks>
        /// <param name="location">The StudyLocation for the study being processed</param>
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
        /// On success and if <see cref="uid"/> is set, the <see cref="WorkItemUid"/> field is marked as complete.  If processing fails, 
        /// the FailureCount field is incremented.
        /// </para>
        /// </remarks>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to update with information from the file.</param>
        /// <param name="file">The file to process.</param>
        /// <param name="uid">An optional WorkQueueUid associated with the entry, that will be deleted upon success or failed on failure.</param>
        /// <exception cref="ApplicationException"/>
        /// <exception cref="DicomDataException"/>
        public void ProcessFile(DicomFile file, StudyXml studyXml, WorkItemUid uid)
        {
            Platform.CheckForNullReference(file, "file");
            Platform.CheckForNullReference(studyXml, "studyXml");
            var processFile = new ProcessorFile(file, uid);
            InsertBatch(new List<ProcessorFile> {processFile}, studyXml);
        }

        /// <summary>
        /// Process a batch of DICOM Files related to a specific Study.  Updates for all the files will be processed together.
        /// </summary>
        /// <param name="list">The list of files to batch together.</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to update with information from the file.</param>
        public void ProcessBatch(IList<ProcessorFile> list, StudyXml studyXml)
        {
            Platform.CheckTrue(list.Count > 0,"list");
            Platform.CheckForNullReference(studyXml, "studyXml");
            InsertBatch(list, studyXml);
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
                        if(!string.IsNullOrEmpty(file.FilePath) && file.File == null)
                        {
                            try
                            {
                                file.File = new DicomFile(file.FilePath);

                                // WARNING:  If we ever do anything where we update files and save them,
                                // we may have to change this.
                                file.File.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
                            }
                            catch (FileNotFoundException)
                            {
                                Platform.Log(LogLevel.Warn, "File to be processed is not found, ignoring: {0}",
                                             file.FilePath);

                                if (file.ItemUid != null)
                                    processor.AddCommand(new CompleteWorkItemUidCommand(file.ItemUid));

                                continue;
                            }
                        }
                        else
                        {
                            file.FilePath = file.File.Filename;
                        }
                        
                        String seriesUid = file.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                        String sopUid = file.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

                        String finalDest = StudyLocation.GetSopInstancePath(seriesUid, sopUid);

                        if (file.FilePath != finalDest)
                        {
                            processor.AddCommand(new RenameFileCommand(file.FilePath, finalDest, false));
                        }

                        // Update the StudyStream object
                        var insertStudyXmlCommand = new InsertStudyXmlCommand(file.File, studyXml, StudyLocation, false);
                        processor.AddCommand(insertStudyXmlCommand);

                        var insertStudyCommand = new InsertOrUpdateStudyCommand(StudyLocation, file.File, studyXml);
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
