#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Processor for 'StudyProcess' WorkQueue entries.
    /// </summary>
    public class StudyProcessItemProcessor : BaseItemProcessor, IWorkQueueItemProcessor
    {
        #region Private Members
        private ServerRulesEngine _sopProcessedRulesEngine;
        private string _processorID;
        #endregion

        #region Public Properties
        public string ProcessorID
        {
            set { _processorID = value; }
            get { return _processorID; }
        }
        #endregion

        #region Private Methods


        private StudyXml LoadStudyStream(string streamFile)
        {
            StudyXml theStream = new StudyXml();

            if (File.Exists(streamFile))
            {
                Stream fileStream = new FileStream(streamFile, FileMode.Open);

                XmlDocument theDoc = new XmlDocument();

                StudyXmlIo.Read(theDoc, fileStream);

                fileStream.Close();

                theStream.SetMemento(theDoc);
            }

            return theStream;
        }

        private void ProcessFile(Model.WorkQueue item, string path, StudyXml stream, string studyStreamFile)
        {
            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM File");
            try
            {
                DicomFile file = new DicomFile(path);

                file.Load();

                // Get the Patients Name for processing purposes.
                String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

                // Update the StudyStream object
                processor.AddCommand(new InsertStreamCommand(file, stream, studyStreamFile));

                // Insert into the database
                processor.AddCommand(new InsertInstanceCommand(file,_storageLocation));

                ServerActionContext context = new ServerActionContext(file, item.ServerPartitionKey, item.StudyStorageKey);
                context.CommandProcessor = processor;

                _sopProcessedRulesEngine.Execute(context);

                if (!processor.Execute())
                {
                    Platform.Log(LogLevel.Error,"Failure processing command: {0}", processor.Description);
                }
                else
                    Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", processor.Description);
                processor.Rollback();
                throw new ApplicationException("Unexpected exception when processing file.",e);
            }            
        }

        private void ProcessUidList(Model.WorkQueue item)
        {
            string studyStreamPath =
                Path.Combine(_storageLocation.GetStudyPath(), _storageLocation.StudyInstanceUid + ".xml");
            
            StudyXml stream;

            try
            {
                stream = LoadStudyStream(studyStreamPath);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e,
                             "Unexpected exception when loading study stream to process StudyProcess WorkQueue item for Study Instance UID: {0} File: {1}", _storageLocation.StudyInstanceUid, studyStreamPath);
                throw new ApplicationException("Unexpected exception when loading study stream", e);
            }

            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in _uidList)
            {
                string path = Path.Combine(_storageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ".dcm");
                try
                {
                    ProcessFile(item, path,stream, studyStreamPath);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                    continue;
                }

                successfulProcessCount++;

                // Delete it out of the queue
                DeleteWorkQueueUid(sop);                
            }

            if (successfulProcessCount == 0)
                SetWorkQueueItemPending(item, true); // set failure status if retries expired
            else
                SetWorkQueueItemPending(item, false); // set success status

        }
        #endregion

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        public void Process(Model.WorkQueue item)
        {

#if DEBUG
        // Simulate slow processing so that we can stop the service
        // and test that it reset workqueue item when restarted
            Console.WriteLine("WorkQueue Item has been locked for processing...");
            Console.WriteLine("Press <Ctrl-C> to stop the service now\n");
            Thread.Sleep(10000);
            Console.WriteLine("WorkQueue Item is being processed...");
#endif

            //Load the specific UIDs that need to be processed.
            LoadUids(item);

            if (_uidList.Count == 0)
            {
                SetWorkQueueItemComplete(item);
            }
            else
            {
                // Load the rules engine
                _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SopProcessed"));
                _sopProcessedRulesEngine.Load();

                //Load the storage location.
                LoadStorageLocation(item);

                ProcessUidList(item);
            }
        }
        #endregion

    }
}