#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion
using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Server Command for editting a study
    /// </summary>
    public class EditStudyCommand : ServerDatabaseCommand
    {
        #region Private Members
        private EditStudyContext _context = null;
        private readonly IActionSet<EditStudyContext> _action = null;
        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Create an instance of <see cref="EditStudyCommand"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="actionNode">An <see cref="XmlElement"/> specifying the actions to be performed</param>
        public EditStudyCommand(string description, XmlElement actionNode)
            : base(description, true)
        {
            Platform.CheckForNullReference(actionNode, "actionNode");
            
            XmlActionCompiler<EditStudyContext> actionCompiler = new XmlActionCompiler<EditStudyContext>();
            _action = actionCompiler.Compile(actionNode, false);

        }
        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Sets/Gets the context associated with the operation
        /// </summary>
        public EditStudyContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        #endregion Public Properties


        #region Private Methods

        /// <summary>
        /// Returns the temporary DICOM file path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetTemporaryDicomFilePath(DicomFile file, EditStudyContext context)
        {
            
            string seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            string sopInstanceUid = file.MediaStorageSopInstanceUid;
            if (sopInstanceUid == String.Empty)
                sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

            Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");
            Platform.CheckForEmptyString(sopInstanceUid, "sopInstanceUid");

            string path = context.DestinationFolder;
            path = Path.Combine(path, context.NewStudyDate);
            path = Path.Combine(path, context.NewStudyInstanceUid);
            path = Path.Combine(path, seriesInstanceUid);
            path = Path.Combine(path, sopInstanceUid);

            path = path + ".dcm";

            return path;

        }

        /// <summary>
        /// Commit all changes to the filesystem.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="Exception">Thrown if an error occurs that prevents the new study files to be copied to the filesystem</exception>
        private static void CommitChanges(EditStudyContext context)
        {
            Platform.Log(LogLevel.Debug, "Committing changes to filesystem folder..");

            string originalStudyFolder = context.StorageLocation.GetStudyPath();
            string partitionFolder = Path.Combine(context.StorageLocation.FilesystemPath, context.StorageLocation.PartitionFolder);
            string newStudyFolder = Path.Combine(partitionFolder, context.NewStudyDate);

            // get the path to the output folder where the temp files are stored
            string srcStudyFolder = context.DestinationFolder;
            srcStudyFolder = Path.Combine(srcStudyFolder, context.NewStudyDate);
            srcStudyFolder = Path.Combine(srcStudyFolder, context.NewStudyInstanceUid);

            // this is where the new study files should go
            string destFolder = Path.Combine(partitionFolder, context.NewStudyDate);
            destFolder = Path.Combine(destFolder, context.NewStudyInstanceUid);

            try
            {
                // Save the new study xml
                string newStudyXmlPath = Path.Combine(srcStudyFolder, context.NewStudyInstanceUid + ".xml");
                using (FileStream stream = new FileStream(newStudyXmlPath, FileMode.CreateNew))
                {
                    StudyXmlIo.Write(context.NewStudyXml.GetMemento(), stream);
                }

                // Before moving the temp study folder to the real filesystem, 
                // we must change the named of the original folder. This will allow us to undo if we encounter problems
                Directory.Move(originalStudyFolder, originalStudyFolder + ".ToBeDeleted");

                if (!Directory.Exists(newStudyFolder))
                {
                    Directory.CreateDirectory(newStudyFolder);
                }
                
                // move the temp study folder to the new folder
                Platform.Log(LogLevel.Debug, "Moving {0} to {1}", srcStudyFolder, destFolder);
                Directory.Move(srcStudyFolder, destFolder);

                // everything's good. Now we can delete the original folder
                Directory.Delete(originalStudyFolder + ".ToBeDeleted", true);
            }
            catch (Exception)
            {
                if (Directory.Exists(originalStudyFolder + ".ToBeDeleted"))
                {
                    Platform.Log(LogLevel.Debug, "Error has occured. Restoring original study folder..");
                    Directory.Move(originalStudyFolder + ".ToBeDeleted", originalStudyFolder);
                }

                throw;

            }
            finally
            {
                // Clean up the temp folder
                try
                {
                    Directory.Delete(context.DestinationFolder, true);
                }
                catch(Exception e)
                {
                    // can't delete the temp files? So be it. We don't care.
                    Platform.Log(LogLevel.Debug, e, String.Format("Unable to clean up temporary folder {0}", context.DestinationFolder));
                }
            }

        }

        private void UpdateXml(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");

            if (!context.NewStudyXml.AddFile(context.CurrentFile))
            {
                throw new Exception("Unable to update study header xml");
            }
        }

        #endregion Private Methods

        /// <summary>
        /// Updates the study information 
        /// </summary>
        /// <param name="updateContext"></param>
        /// 
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
                            Platform.Log(LogLevel.Debug, "Loading file: {0}", path);
                            Context.CurrentFile = new DicomFile(path);
                            Context.CurrentFile.Load();

                            Platform.Log(LogLevel.Debug, "Updating file: {0}", path);
                            TestResult result = _action.Execute(Context);

                            if (result.Success)
                            {
                                UpdateXml(Context);

                                Context.NewStudyDate = Context.CurrentFile.DataSet[DicomTags.StudyDate].GetString(0, Platform.Time.ToString("yyyyMMdd"));
                                Context.NewStudyInstanceUid = Context.CurrentFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, "Unknown");

                                // store the result in a temporary file
                                string tempPath = GetTemporaryDicomFilePath(Context.CurrentFile, Context);
                                Platform.Log(LogLevel.Debug, "Writing temp dicom file: {0}", tempPath);
                                Context.CurrentFile.Save(tempPath);

                                cancel = false;
                            }
                            else
                            {
                                string failureReason=String.Format("StudyEditCommand failed on file {0}", path);
                                foreach (TestResultReason reason in result.Reasons)
                                {
                                    failureReason += String.Format("{0}\n", reason.Message);
                                }
                                
                                cancel = true;
                                throw new Exception(failureReason); // no changes have been made to the real filesystem, the database changes will be discarded by the caller
                            }
                        }, 

                        true);

                    CommitChanges(Context);
                }
            }
            
        }
        
    }
}
