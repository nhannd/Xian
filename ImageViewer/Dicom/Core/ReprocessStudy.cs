using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class for reprocessing Study.  Primarily used by <see cref="ReindexProcessor"/>.
    /// </summary>
    public class ReprocessStudy
    {
        #region Public Properties

        public StudyLocation Location { get; private set; }
        public bool StudyStoredInDatabase { get; private set; }

        #endregion

        #region Constructors

        public ReprocessStudy(StudyLocation location )
        {
            Location = location;
            StudyStoredInDatabase = CheckIfStudyExists();
        }

        #endregion

        #region Public Methods

        public void Process()
        {
            if (!StudyStoredInDatabase)
                ReprocessEntireStudy();
            else
                RebuildStudyXml();
        }

        #endregion

        #region Private Methods

        private void ReprocessEntireStudy()
        {
            try
            {                        
                var studyXml = Location.LoadStudyXml();
                var fileList = new List<SopInstanceProcessor.ProcessorFile>();

                FileProcessor.Process(Location.StudyFolder, "*.dcm", delegate(string file)
                                                           {
                                                               try
                                                               {
                                                                   fileList.Add(
                                                                       new SopInstanceProcessor.
                                                                           ProcessorFile(file, null));

                                                                   if (fileList.Count > 19)
                                                                   {
                                                                       var p = new SopInstanceProcessor(Location);

                                                                       p.ProcessBatch(fileList, studyXml);

                                                                       fileList.Clear();
                                                                   }
                                                               }
                                                               catch (Exception x)
                                                               {
                                                                   Platform.Log(LogLevel.Error, x);
                                                               }

                                                           }, false);
                if (fileList.Count > 0)
                {
                    var p = new SopInstanceProcessor(Location);

                    p.ProcessBatch(fileList, studyXml);
                }

            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", Location.StudyFolder);
            }

            Platform.Log(LogLevel.Info, "Reprocessed study: {0}", Location.Study.StudyInstanceUid);
        }    

        private void RebuildStudyXml()
        {         
            try
            {                        
                var studyXml = new StudyXml(Location.Study.StudyInstanceUid);

                DicomFile lastFile = null;
                FileProcessor.Process(Location.StudyFolder, "*.dcm", delegate(string file)
                                                           {
                                                               try
                                                               {
                                                                   lastFile = new DicomFile(file);
                                                                   lastFile.Load(DicomReadOptions.Default |
                                                                                  DicomReadOptions.
                                                                                      StorePixelDataReferences);

                                                                   studyXml.AddFile(lastFile);
                                                                   
                                                               }
                                                               catch (Exception x)
                                                               {
                                                                   Platform.Log(LogLevel.Error, x,
                                                                                "Failed to load file for reprocessing: {0}", file);
                                                               }

                                                           }, false);

                
                if (lastFile !=null)
                {
                    var p = new SopInstanceProcessor(Location);

                    p.ProcessFile(lastFile, studyXml, null);
                }
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", Location.StudyFolder);
            }

            Platform.Log(LogLevel.Info, "Rebuilt Study XML: {0}", Location.Study.StudyInstanceUid);
        }

        private bool CheckIfStudyExists()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();
                var study = broker.GetStudy(Location.Study.StudyInstanceUid);

                if (study != null)
                {
                    Location.Study = study;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
