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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class for reprocessing Study.  Primarily used by <see cref="ReindexUtility"/>.
    /// </summary>
    public class ReprocessStudyFolder
    {
        #region Public Properties

        public StudyLocation Location { get; private set; }
        public bool StudyStoredInDatabase { get; private set; }

        #endregion

        #region Constructors

        public ReprocessStudyFolder(StudyLocation location )
        {
            Location = location;
            StudyStoredInDatabase = CheckIfStudyExists();
        }

        #endregion

        #region Public Methods

        public void Process()
        {
            if (!StudyStoredInDatabase)
                ReprocessFolder();
            else
                RebuildStudyXml();
        }

        #endregion

        #region Private Methods

        private void ReprocessFolder()
        {
            try
            {                        
                var studyXml = Location.LoadStudyXml();
                var fileList = new List<ProcessStudyUtility.ProcessorFile>();

                // This code will cleanup a folder and move images around to the proper location.
                // It in essence allows you to just copy a bunch of files into the filestore, and reindex will clean them up and organize them.
                FileProcessor.Process(Location.StudyFolder, "*.dcm", delegate(string file)
                                                           {
                                                               try
                                                               {
                                                                   var dicomFile = new DicomFile(file);

                                                                   dicomFile.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
                                                                   String studyInstanceUid = dicomFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);

                                                                   if (!Location.Study.StudyInstanceUid.Equals(studyInstanceUid))
                                                                   {
                                                                       Platform.Log(LogLevel.Warn,
                                                                                    "Importing file that was in the wrong study folder: {0}",
                                                                                    file);
                                                                       var context =
                                                                           new ImportStudyContext(
                                                                               dicomFile.SourceApplicationEntityTitle, StudyStore.GetConfiguration());
                                                                       var importer = new ImportFilesUtility(context);
                                                                       var result = importer.Import(dicomFile, BadFileBehaviourEnum.Delete, FileImportBehaviourEnum.Move);
                                                                       if (!result.DicomStatus.Equals(DicomStatuses.Success))
                                                                       {
                                                                           try
                                                                           {
                                                                               Platform.Log(LogLevel.Warn, "Unable to import file: {0}, deleting: {1}", result.ErrorMessage, file);
                                                                               FileUtils.Delete(file);
                                                                           }
                                                                           catch (Exception x)
                                                                           {
                                                                               Platform.Log(LogLevel.Warn, x, "Unexpected exception deleting file: {0}", file);
                                                                           }
                                                                       }
                                                                   }
                                                                   else
                                                                   {
                                                                       fileList.Add(new ProcessStudyUtility.ProcessorFile(dicomFile, null));

                                                                       if (fileList.Count > 19)
                                                                       {
                                                                           var p = new ProcessStudyUtility(Location);

                                                                           p.ProcessBatch(fileList, studyXml);

                                                                           fileList.Clear();
                                                                       }
                                                                   }
                                                               }
                                                               catch (Exception x)
                                                               {
                                                                   Platform.Log(LogLevel.Error, x);
                                                                   fileList.Clear(); // Clear out the failed entries
                                                               }
                                                           }, true);
                if (fileList.Count > 0)
                {
                    var p = new ProcessStudyUtility(Location);

                    p.ProcessBatch(fileList, studyXml);

                    // Now apply Deletion rules
                    var ep = new RulesEngineExtensionPoint();

                    var ruleContext = new RulesEngineContext
                                          {
                                              ApplyDeleteActions = true,
                                              ApplySendStudyActions = false
                                          };
                    StudyEntry studyEntry = p.StudyLocation.Study.ToStoreEntry();
                    foreach (IRulesEngine engine in ep.CreateExtensions())
                    {
                        engine.ApplyStudyRules(ruleContext, studyEntry);
                    }
                }

            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", Location.StudyFolder);
            }

            Platform.Log(LogLevel.Info, "Reprocessed study folder: {0}", Location.Study.StudyInstanceUid);
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
                                                                                  DicomReadOptions.StorePixelDataReferences);

                                                                   if (!studyXml.AddFile(lastFile))
                                                                   {
                                                                       Platform.Log(LogLevel.Warn,
                                                                                    "Importing file that was in the wrong study folder: {0}",
                                                                                    file);
                                                                       var context =
                                                                           new ImportStudyContext(
                                                                               lastFile.SourceApplicationEntityTitle, StudyStore.GetConfiguration());
                                                                       var importer = new ImportFilesUtility(context);
                                                                       var result = importer.Import(lastFile,BadFileBehaviourEnum.Delete, FileImportBehaviourEnum.Move);
                                                                       if (result.DicomStatus != DicomStatuses.Success)
                                                                       {
                                                                           Platform.Log(LogLevel.Warn,"Unable to import file: {0}", result.ErrorMessage);
                                                                       }
                                                                   }
                                                               }
                                                               catch (Exception x)
                                                               {
                                                                   Platform.Log(LogLevel.Error, x,
                                                                                "Failed to load file for reprocessing: {0}", file);
                                                               }

                                                           }, false);

                
                if (lastFile !=null)
                {
                    var p = new ProcessStudyUtility(Location);

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
