using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Command class to update study files
    /// </summary>
    /// <remarks>
    /// This class updates the contents in the study folder and moves the folder to the
    /// appropriate location if necessary.
    /// </remarks>
    internal class StudyFilesUpdateCommand : EditStudyActionCommandBase
    {
        #region Private Members
        private readonly IActionSet<EditStudyContext> _action = null;
        private bool _originalFilesModified = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="StudyFilesUpdateCommand"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="context"></param>
        /// <param name="actionNode"></param>
        public StudyFilesUpdateCommand(string description, EditStudyContext context, XmlElement actionNode)
            : base(description, context)
        {
            Platform.CheckForNullReference(actionNode, "actionNode");

            XmlActionCompiler<EditStudyContext> actionCompiler = new XmlActionCompiler<EditStudyContext>();
            _action = actionCompiler.Compile(actionNode, false);
        }

        #endregion

        #region Protected Properties

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the temporary DICOM file path
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string GetTemporaryDicomFilePath(DicomFile file)
        {

            string seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            string sopInstanceUid = file.MediaStorageSopInstanceUid;
            if (sopInstanceUid == String.Empty)
                sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

            Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");
            Platform.CheckForEmptyString(sopInstanceUid, "sopInstanceUid");

            string path = Path.Combine(TempStudyFolder, seriesInstanceUid);
            path = Path.Combine(path, sopInstanceUid);

            path = path + ".dcm";

            return path;

        }

        /// <summary>
        /// Commit all changes to the filesystem.
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs that prevents the new study files to be copied to the filesystem</exception>
        private void UpdateOriginalFolder()
        {
            Platform.Log(LogLevel.Debug, "Committing changes to filesystem folder..");

            string originalStudyFolder = Context.OriginalStorageLocation.GetStudyPath();
            string newStudyFolder = Directory.GetParent(DesinationStudyFolder).FullName;

            
            Directory.Move(originalStudyFolder, originalStudyFolder + ".tobedeleted");
            _originalFilesModified = true; 
            
            if (!Directory.Exists(newStudyFolder))
            {
                Directory.CreateDirectory(newStudyFolder);
            }

            //// move the temp study folder to the new folder
            Directory.Move(TempStudyFolder, DesinationStudyFolder);
        }

        private void UpdateXml(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");

            if (!context.NewStudyXml.AddFile(context.CurrentFile))
            {
                throw new Exception("Unable to update study header xml");
            }

        }

        private void CreateNewStudyFiles()
        {
            int _filesCount = 0;
            string folder = Context.OriginalStorageLocation.GetStudyPath();
            FileProcessor.Process(folder, "*.dcm",
                                  delegate(string path, out bool cancel)
                                  {
                                      _filesCount++;
                                      Platform.Log(LogLevel.Debug, "Loading file: {0}", path);
                                      Context.CurrentFile = new DicomFile(path);
                                      Context.CurrentFile.Load();

                                      Platform.Log(LogLevel.Debug, "Updating file: {0}", path);
                                      TestResult result = _action.Execute(Context);

                                      if (result.Success)
                                      {
                                          UpdateXml(Context);

                                          // store the result in a temporary file
                                          string tempPath = GetTemporaryDicomFilePath(Context.CurrentFile);
                                          Platform.Log(LogLevel.Debug, "Writing temp dicom file: {0}", tempPath);
                                          Context.CurrentFile.Save(tempPath);

                                          cancel = false;
                                      }
                                      else
                                      {
                                          string failureReason = String.Format("StudyEditCommand failed on file {0}", path);
                                          foreach (TestResultReason reason in result.Reasons)
                                          {
                                              failureReason += String.Format("{0}\n", reason.Message);
                                          }

                                          cancel = true;
                                          throw new Exception(failureReason); // no changes have been made to the real filesystem, the database changes will be discarded by the caller
                                      }
                                  },

                                  true);
            if (_filesCount>0)
            {
                // Generate the new study xml
                string tempStudyFolder = Path.Combine(TempOutFolder, Context.Study.StudyInstanceUid);
                string newStudyXmlPath = Path.Combine(tempStudyFolder, Context.Study.StudyInstanceUid + ".xml");
                using (FileStream stream = new FileStream(newStudyXmlPath, FileMode.CreateNew))
                {
                    XmlDocument studyXml = Context.NewStudyXml.GetMemento(ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings);
                    StudyXmlIo.Write(studyXml, stream);

                    string newGzipStudyXmlPath = Path.Combine(tempStudyFolder, Context.Study.StudyInstanceUid + ".xml.gz");
                    using (FileStream gzStream = new FileStream(newGzipStudyXmlPath, FileMode.CreateNew))
                    {
                        StudyXmlIo.WriteGzip(studyXml, gzStream);
                    }
                }
            }

            
        }

        #endregion

        #region Overridden Protected Methods
        protected override void OnExecute()
        {
            CreateNewStudyFiles();

            UpdateOriginalFolder();
        }

        
        protected override void OnUndo()
        {
            if (_originalFilesModified)
            {
                string originalDir = Context.OriginalStorageLocation.GetStudyPath();
                string backupFolder = originalDir + ".tobedeleted";
                string newDir = Context.OriginalStorageLocation.GetStudyPath();
                try
                {

                    // restore original folder
                    if (Directory.Exists(backupFolder))
                        Directory.Move(backupFolder, originalDir);
                }
                catch (Exception e)
                {
                    // Write this to log so the files can restored manually if needed
                    string msg = String.Format("Unable to restore original study files from {0} : {1}", backupFolder, e);
                    Platform.Log(LogLevel.Error, e, msg);
                    throw;
                }

                // get rid of the new study folder if it is created in another location
                if (newDir != originalDir)
                {
                    DirectoryUtility.DeleteIfExists(newDir, true);
                }
            }

        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            string backupFolder = Context.OriginalStorageLocation.GetStudyPath() + ".tobedeleted";

            try
            {
                DirectoryUtility.DeleteIfExists(TempOutFolder);

                DirectoryInfo backupParentFolder =  Directory.GetParent(backupFolder);
                DirectoryUtility.DeleteIfExists(backupFolder);
                DirectoryUtility.DeleteIfEmpty(backupParentFolder.FullName); 
            }
            catch(Exception e)
            {
                // ignore
                Platform.Log(LogLevel.Warn, e, "Cleanup failed but ignored");
            }
            base.Dispose();
        }

        #endregion

    }

}
