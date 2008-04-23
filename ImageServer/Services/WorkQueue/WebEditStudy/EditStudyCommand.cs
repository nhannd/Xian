using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    public class EditStudyCommand : ServerDatabaseCommand
    {
        private EditStudyContext _context = null;
        private IActionSet<EditStudyContext> _action = null;
        
        public EditStudyCommand(string description)
            : base("Edit Study " + description, true)
        {
            
        }

        public EditStudyContext Context
        {
            get { return _context; }
            set { _context = value; }
        }


        public void Compile(XmlElement containingNode)
        {
            Platform.CheckForNullReference(containingNode, "containingNode");

            XmlActionCompiler<EditStudyContext> actionCompiler = new XmlActionCompiler<EditStudyContext>();
            _action = actionCompiler.Compile(containingNode, false);
        }


        public string GetTemporaryStudyFolderPath(EditStudyContext context)
        {
            Platform.CheckForEmptyString(context.DestinationFolder, "Context.DestinationFolder");
            Platform.CheckForEmptyString(context.NewStudyDate, "Context.NewStudyDate");
            Platform.CheckForEmptyString(context.NewStudyInstanceUid, "Context.NewStudyInstanceUid");

            string path = context.DestinationFolder;
            path = Path.Combine(path, context.NewStudyDate);
            path = Path.Combine(path, context.NewStudyInstanceUid);

            return path;
        }
        public string GetTemporaryFilePath(DicomFile file, EditStudyContext context)
        {
            
            string seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            string sopInstanceUid = file.MediaStorageSopInstanceUid;
            if (sopInstanceUid == String.Empty)
                sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

            Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");
            Platform.CheckForEmptyString(sopInstanceUid, "sopInstanceUid");
            

            string path = GetTemporaryStudyFolderPath(context);
            path = Path.Combine(path, seriesInstanceUid);
            path = Path.Combine(path, sopInstanceUid);

            path = path + ".dcm";

            return path;

        }


        protected void CommitChanges(EditStudyContext context)
        {

            string originalStudyFolder = context.StorageLocation.GetStudyPath();

            try
            {
                Platform.Log(LogLevel.Info, "Committing changes to filesystem folder..");

                string srcStudyFolder = context.DestinationFolder;
                srcStudyFolder = Path.Combine(srcStudyFolder, context.NewStudyDate);
                srcStudyFolder = Path.Combine(srcStudyFolder, context.NewStudyInstanceUid);


                string partitionFolder =
                    Path.Combine(context.StorageLocation.FilesystemPath, context.StorageLocation.PartitionFolder);


                string newStudyXmlPath = Path.Combine(srcStudyFolder, context.NewStudyInstanceUid);
                newStudyXmlPath += ".xml";

                using (FileStream stream = new FileStream(newStudyXmlPath, FileMode.CreateNew))
                {
                    StudyXmlIo.Write(context.NewStudyXml.GetMemento(), stream);
                }



                Directory.Move(originalStudyFolder, originalStudyFolder + ".ToBeDeleted");

                string newStudyRootFolder = Path.Combine(partitionFolder, context.NewStudyDate);
                if (!Directory.Exists(newStudyRootFolder))
                {
                    Directory.CreateDirectory(newStudyRootFolder);
                }

                if (Directory.Exists(newStudyRootFolder))
                {
                    // move the study folder to the new folder

                    string destFolder = Path.Combine(partitionFolder, context.NewStudyDate);
                    destFolder = Path.Combine(destFolder, context.NewStudyInstanceUid);
                    Platform.Log(LogLevel.Info, "Moving {0} to {1}", srcStudyFolder, destFolder);
                    Directory.Move(srcStudyFolder, destFolder);

                    Directory.Delete(originalStudyFolder + ".ToBeDeleted", true);
                }
                else
                    throw new Exception(String.Format("Destination directory {0} not found", newStudyRootFolder));
            }
            catch (Exception e)
            {
                if (Directory.Exists(originalStudyFolder + ".ToBeDeleted"))
                {
                    Platform.Log(LogLevel.Info, "Reverting changes to folder");
                    Directory.Move(originalStudyFolder + ".ToBeDeleted", originalStudyFolder);
                }

                throw;

            }
            finally
            {
                Directory.Delete(context.DestinationFolder, true);
            }

        }


        protected override void OnExecute(IUpdateContext updateContext)
        {
            if (Context!=null)
            {

                if (_action != null)
                {
                    Context.UpdateContext = updateContext;
                    
                    FileProcessor.Process(Context.StorageLocation.GetStudyPath(), "*.dcm", 
                        delegate(string path, out bool cancel)
                        {
                            Platform.Log(LogLevel.Info, "Updating file: {0}", path);
                                
                            Context.CurrentFile = new DicomFile(path);
                            Context.CurrentFile.Load();

                            TestResult result = _action.Execute(Context);

                            if (result.Success)
                            {
                                
                                Context.NewStudyDate =
                                    Context.CurrentFile.DataSet[DicomTags.StudyDate].GetString(0, Platform.Time.ToString("yyyyMMdd"));
                                Context.NewStudyInstanceUid =
                                    Context.CurrentFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, "Unknown");

                                SaveFile(Context);

                                cancel = false;
                            }
                            else
                            {
                                string failureReason=String.Format("StudyEdit failed on file {0}", path);

                                foreach (TestResultReason reason in result.Reasons)
                                {
                                    failureReason += String.Format("{0}\n", reason.Message);
                                }
                                
                                cancel = true;
                                throw new Exception(failureReason);
                            }
                        }, 

                        true);

                    CommitChanges(Context);
                }
            }
            
        }

        private void SaveFile(EditStudyContext context)
        {
            // store the result in a temporary file
            string tempPath = GetTemporaryFilePath(context.CurrentFile, Context);
            Platform.Log(LogLevel.Info, "Writing to temp file: {0}", tempPath);
            context.CurrentFile.Save(tempPath);
        }
    }
}
