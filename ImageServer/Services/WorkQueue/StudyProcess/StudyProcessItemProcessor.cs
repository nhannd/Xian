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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Processor for 'StudyProcess' <see cref="WorkQueue"/> entries.
    /// </summary>
    public class StudyProcessItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ServerRulesEngine _sopProcessedRulesEngine;
        private ServerRulesEngine _studyProcessedRulesEngine;
        private ServerRulesEngine _seriesProcessedRulesEngine;

        private StudyProcessStatistics _stats;
        private InstanceStatistics _instanceStats;
        
        #endregion

        #region Public Properties
       
        #endregion

        #region Constructors
        public StudyProcessItemProcessor()
        {
            _stats = new StudyProcessStatistics();
        }
        #endregion Constructors

        #region Private Methods
        /// <summary>
        /// Method for applying rules when a new study has been inserted.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed.</param>
        /// <param name="file">The DICOM file being processed.</param>
        private void ProcessStudyRules(Model.WorkQueue item, DicomFile file)
        {
            if (_studyProcessedRulesEngine == null)
            {
                _studyProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("StudyProcessed"), item.ServerPartitionKey);
                _studyProcessedRulesEngine.Load();
                _instanceStats.EngineLoadTime.Add(_studyProcessedRulesEngine.Statistics.LoadTime);
            }
            ServerActionContext context = new ServerActionContext(file,StorageLocation.FilesystemKey, item.ServerPartitionKey,item.StudyStorageKey);

            context.CommandProcessor = new ServerCommandProcessor("Study Rule Processor");

            _studyProcessedRulesEngine.Execute(context);

            if (false == context.CommandProcessor.Execute())
            {
                Platform.Log(LogLevel.Error, "Unexpeected failure processing Study level rules");   
            }

            _instanceStats.EngineExecutionTime.Add(_studyProcessedRulesEngine.Statistics.ExecutionTime);
        }


        /// <summary>
        /// Method for applying rules when a new series has been inserted.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed.</param>
        /// <param name="file">The DICOM file being processed.</param>
        private void ProcessSeriesRules(Model.WorkQueue item, DicomFile file)
        {
            if (_seriesProcessedRulesEngine == null)
            {
                _seriesProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SeriesProcessed"), item.ServerPartitionKey);
                _seriesProcessedRulesEngine.Load();
                _instanceStats.EngineLoadTime.Add(_seriesProcessedRulesEngine.Statistics.LoadTime);
            }
            ServerActionContext context = new ServerActionContext(file, StorageLocation.FilesystemKey, item.ServerPartitionKey, item.StudyStorageKey);

            context.CommandProcessor = new ServerCommandProcessor("Series Rule Processor");

            _seriesProcessedRulesEngine.Execute(context);

            if (false == context.CommandProcessor.Execute())
            {
                Platform.Log(LogLevel.Error,"Error processing Series level rules StudyStorage {0}",item.StudyStorageKey);
            }

            _instanceStats.EngineExecutionTime.Add(_seriesProcessedRulesEngine.Statistics.ExecutionTime);
        }

        /// <summary>
        /// Process a specific DICOM file related to a <see cref="WorkQueue"/> request.
        /// </summary>
        /// <param name="item">The WorkQueue item to process</param>
        /// <param name="path">The path of the file to process.</param>
        /// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
        private void ProcessFile(Model.WorkQueue item, string path, StudyXml stream)
        {
            InstanceKeys keys;
            DicomFile file;
            long fileSize;
            String modality = null;

            FileInfo fileInfo = new FileInfo(path);
            fileSize = fileInfo.Length;

            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM File");
            InsertStudyXmlCommand insertStudyXmlCommand = null;
            InsertInstanceCommand insertInstanceCommand = null;

            try
            {
                OnProcessFileBegin();

                _instanceStats.FileLoadTime.Start();
                file = new DicomFile(path);
                file.Load();
                _instanceStats.FileLoadTime.End();
                _instanceStats.FileSize = (ulong) fileSize;

                _instanceStats.Name = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:"+fileInfo.Name);

                // Get the Patients Name for processing purposes.
                String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");
                modality = file.DataSet[DicomTags.Modality].GetString(0, "");
                
                // Update the StudyStream object
                insertStudyXmlCommand = new InsertStudyXmlCommand(file, stream, StorageLocation);
                processor.AddCommand(insertStudyXmlCommand);

                // Insert into the database
                insertInstanceCommand = new InsertInstanceCommand(file, StorageLocation);
                processor.AddCommand(insertInstanceCommand);

                // Create a context for applying actions from the rules engine
                ServerActionContext context =
                    new ServerActionContext(file, StorageLocation.FilesystemKey, item.ServerPartitionKey, item.StudyStorageKey);
                context.CommandProcessor = processor;

                // Run the rules engine against the object.
                _sopProcessedRulesEngine.Execute(context);
                
                // Do the actual processing
                if (!processor.Execute())
                {
                    Platform.Log(LogLevel.Error, "Failure processing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
                    throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid,
                                 patientsName);
                    keys = insertInstanceCommand.InsertKeys;
                }

                
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
                             processor.Description);
                processor.Rollback();
                _instanceStats.ProcessTime.End();  
                throw new ApplicationException("Unexpected exception when processing file.", e);
            }
            finally
            {
                _instanceStats.EngineLoadTime.Add(_sopProcessedRulesEngine.Statistics.LoadTime);
                _instanceStats.EngineExecutionTime.Add(_sopProcessedRulesEngine.Statistics.ExecutionTime);
                if (insertInstanceCommand != null)
                    _instanceStats.InsertDBTime.Add(insertInstanceCommand.Statistics);
                if (insertStudyXmlCommand!=null)
                _instanceStats.InsertStreamTime.Add(insertStudyXmlCommand.Statistics);

                _stats.StudyInstanceUid = StorageLocation.StudyInstanceUid;
                if (String.IsNullOrEmpty(modality) == false)
                    _stats.Modality = modality;

                // Update the statistics
                _stats.NumInstances++;

            }
            

            if (keys != null)
            {
                // We've inserted a new Study, process Study Rules
                if (keys.InsertStudy)
                {
                    ProcessStudyRules(item, file);
                    _instanceStats.EngineLoadTime.Add(_studyProcessedRulesEngine.Statistics.LoadTime);
                    _instanceStats.EngineExecutionTime.Add(_studyProcessedRulesEngine.Statistics.ExecutionTime);
                }

                // We've inserted a new Series, process Series Rules
                if (keys.InsertSeries)
                {
                    ProcessSeriesRules(item, file);
                    _instanceStats.EngineLoadTime.Add(_seriesProcessedRulesEngine.Statistics.LoadTime);
                    _instanceStats.EngineExecutionTime.Add(_seriesProcessedRulesEngine.Statistics.ExecutionTime);
                }
            }

            _instanceStats.ProcessTime.End();            
        }

        private void OnProcessFileBegin()
        {
            _instanceStats = _stats.NewStatistics("Instance Processing");
            _instanceStats.ProcessTime.Start();
            _sopProcessedRulesEngine.Statistics.LoadTime.Reset();
            _sopProcessedRulesEngine.Statistics.ExecutionTime.Reset();
        }


        /// <summary>
        /// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item.</param>
        private void ProcessUidList(Model.WorkQueue item)
        {
            StudyXml studyXml;
            
            studyXml = LoadStudyXml(StorageLocation);
            
            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in WorkQueueUidList)
            {
                string path = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ".dcm");

                try
                {
                    ProcessFile(item, path,studyXml);
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
            {
                PostProcessing(item, WorkQueueUidList.Count, true); // set the status to Pending or Failed depending on whether or not the max retry has been reached
            }
            else
            {
                PostProcessing(item, WorkQueueUidList.Count, false); // set the status to Pending

				if (_stats.NumInstances > 0)
                {
                    StatisticsLogger.Log(LogLevel.Info, _stats);
                }
            }
            
        }


       
        #endregion

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item.
        /// </summary>
        public override void Process(Model.WorkQueue item)
        {

#if DEBUG_TEST
        // Simulate slow processing so that we can stop the service
        // and test that it reset workqueue item when restarted
            Console.WriteLine("WorkQueue Item has been locked for processing...");
            Console.WriteLine("Press <Ctrl-C> to stop the service now\n");
            Thread.Sleep(10000);
            Console.WriteLine("WorkQueue Item is being processed...");
#endif
            _stats = new StudyProcessStatistics();

            //Load the specific UIDs that need to be processed.
            LoadUids(item);

            if (WorkQueueUidList.Count == 0)
            {
                // No UIDs associated with the WorkQueue item.  Set the status back to idle
                PostProcessing(item, 0, false);
            }
            else
            {
                // Load the rules engine
                _sopProcessedRulesEngine =
                    new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SopProcessed"), item.ServerPartitionKey);
                _sopProcessedRulesEngine.Load();
                
                //Load the storage location.
                LoadStorageLocation(item);

                // Process the images in the list
                ProcessUidList(item);
            }



        }

        #endregion
    }
}
