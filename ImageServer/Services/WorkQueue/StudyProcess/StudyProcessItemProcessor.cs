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
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

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
        private IStudyProcessItemProcessorListener[] _extensions;
      
        private event WorkQueueUIDListProcessingBeginEventListener _workQueueUIDListProcessingBegin;
        private event WorkQueueUIDListProcessingCompletedEventListener _workQueueUIDListProcessingCompleted;
        private event WorkQueueUIDProcessingBeginEventListener _workQueueUIDProcessingBegin;
        private event WorkQueueUIDProcessingCompletedEventListener _workQueueUIDProcessingCompleted;
        private event InsertStreamCommandBeginEventListener _insertStreamCommandBegin;
        private event InsertStreamCommandCompletedEventListener _insertStreamCommandCompleted;
        private event InsertStreamCommandRolledbackEventListener _insertStreamCommandRolledback;
        private event InsertInstanceCommandBeginEventListener _insertInstanceCommandBegin;
        private event InsertInstanceCommandCompletedEventListener _insertInstanceCommandCompleted;
        private event InsertInstanceCommandRolledbackEventListener _insertInstanceCommandRolledback;

        private event RuleEngineExecuteBeginEventListener _ruleEngineExecuteBegin;
        private event RuleEngineExecuteCompletedEventListener _ruleEngineExecuteCompleted;
        private event RuleEngineLoadBeginEventListener _ruleEngineLoadBegin;
        private event RuleEngineLoadCompletedEventListener _ruleEngineLoadCompleted;

        private event DicomFileLoadBeginEventListener _dicomFileLoadBegin;
        private event DicomFileLoadCompletedEventListener _dicomFileLoadCompleted;
        
        #endregion

        #region Public Properties
       
        #endregion

        #region Events
        /// <summary>
        /// Defines the interface for <see cref="WorkQueueUIDListProcessingBegin"/>.
        /// </summary>
        /// <param name="list"></param>
        public delegate void WorkQueueUIDListProcessingBeginEventListener(IList<WorkQueueUid> list);
        
        /// <summary>
        /// Defines the interface for <see cref="WorkQueueUIDListProcessingCompleted"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="status"></param>
        public delegate void WorkQueueUIDListProcessingCompletedEventListener(IList<WorkQueueUid> list, bool status);

        /// <summary>
        /// Defines the interface for <see cref="WorkQueueUIDProcessingBegin"/>.
        /// </summary>
        /// <param name="uid"></param>
        public delegate void WorkQueueUIDProcessingBeginEventListener(WorkQueueUid uid);

        /// <summary>
        /// Defines the interface for <see cref="WorkQueueUIDProcessingCompleted"/>.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        public delegate void WorkQueueUIDProcessingCompletedEventListener(WorkQueueUid uid, bool status);

        /// <summary>
        /// Defines the interface for <see cref="InsertStreamCommandBegin"/>.
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void InsertStreamCommandBeginEventListener(InsertStreamCommand cmd);

        /// <summary>
        /// Defines the interface for <see cref="InsertStreamCommandCompleted"/>.
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void InsertStreamCommandCompletedEventListener(InsertStreamCommand cmd);

        /// <summary>
        /// Defines the interface for <see cref="InsertStreamCommandRolledback"/>.
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void InsertStreamCommandRolledbackEventListener(InsertStreamCommand cmd);

        /// <summary>
        /// Defines the interface for <see cref="InsertInstanceCommandBegin"/>.
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void InsertInstanceCommandBeginEventListener(InsertInstanceCommand cmd);

        /// <summary>
        /// Defines the interface for <see cref="InsertInstanceCommandCompleted"/>.
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void InsertInstanceCommandCompletedEventListener(InsertInstanceCommand cmd);

        /// <summary>
        /// Defines the interface for <see cref="InsertInstanceCommandRolledback"/>.
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void InsertInstanceCommandRolledbackEventListener(InsertInstanceCommand cmd);

        /// <summary>
        /// Defines the interface for <see cref="RuleEngineLoadBegin"/>.
        /// </summary>
        /// <param name="engines"></param>
        public delegate void RuleEngineLoadBeginEventListener(ServerRulesEngine engines);

        /// <summary>
        /// Defines the interface for <see cref="RuleEngineLoadCompleted"/>.
        /// </summary>
        /// <param name="engines"></param>
        public delegate void RuleEngineLoadCompletedEventListener(ServerRulesEngine engines);

        /// <summary>
        /// Defines the interface for <see cref="RuleEngineExecuteBegin"/>.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="context"></param>
        public delegate void RuleEngineExecuteBeginEventListener(ServerRulesEngine engine, ServerActionContext context);

        /// <summary>
        /// Defines the interface for <see cref="RuleEngineExecuteCompleted"/>.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="context"></param>
        public delegate void RuleEngineExecuteCompletedEventListener(ServerRulesEngine engine, ServerActionContext context);

        /// <summary>
        /// Defines the interface for <see cref="DicomFileLoadBegin"/>.
        /// </summary>
        /// <param name="path"></param>
        public delegate void DicomFileLoadBeginEventListener(string path);

        /// <summary>
        /// Defines the interface for <see cref="DicomFileLoadCompleted"/>.
        /// </summary>
        /// <param name="file"></param>
        public delegate void DicomFileLoadCompletedEventListener(DicomFile file);
        
        /// <summary>
        /// Occurs when the processor begins processing the list of WorkQueueUIDs in the database.
        /// </summary>
        public event WorkQueueUIDListProcessingBeginEventListener WorkQueueUIDListProcessingBegin
        {
            add { _workQueueUIDListProcessingBegin += value; }
            remove { _workQueueUIDListProcessingBegin -= value; }
        }

        /// <summary>
        /// Occurs when the processor finishes processing the list of WorkQueueUIDs in the database.
        /// </summary>
        public event WorkQueueUIDListProcessingCompletedEventListener WorkQueueUIDListProcessingCompleted
        {
            add { _workQueueUIDListProcessingCompleted += value; }
            remove { _workQueueUIDListProcessingCompleted -= value; }
        }

        /// <summary>
        /// Occurs when the processor begins processing a <see cref="WorkQueueUid"/>.
        /// </summary>
        public event WorkQueueUIDProcessingBeginEventListener WorkQueueUIDProcessingBegin
        {
            add { _workQueueUIDProcessingBegin += value; }
            remove { _workQueueUIDProcessingBegin -= value; }
        }

        /// <summary>
        /// Occurs when the processor finishes processing a <see cref="WorkQueueUid"/>.
        /// </summary>
        public event WorkQueueUIDProcessingCompletedEventListener WorkQueueUIDProcessingCompleted
        {
            add { _workQueueUIDProcessingCompleted += value; }
            remove { _workQueueUIDProcessingCompleted -= value; }
        }

        /// <summary>
        /// Occurs when the processor begins executing a <see cref="InsertStreamCommand"/>.
        /// </summary>
        public event InsertStreamCommandBeginEventListener InsertStreamCommandBegin
        {
            add { _insertStreamCommandBegin += value; }
            remove { _insertStreamCommandBegin -= value; }
        }

        /// <summary>
        /// Occurs when the processor finishes executing a <see cref="InsertStreamCommand"/>.
        /// </summary>
        public event InsertStreamCommandCompletedEventListener InsertStreamCommandCompleted
        {
            add { _insertStreamCommandCompleted += value; }
            remove { _insertStreamCommandCompleted -= value; }
        }

        /// <summary>
        /// Occurs after the processor has rolled back a <see cref="InsertStreamCommand"/>.
        /// </summary>
        public event InsertStreamCommandRolledbackEventListener InsertStreamCommandRolledback
        {
            add { _insertStreamCommandRolledback += value; }
            remove { _insertStreamCommandRolledback -= value; }
        }

        /// <summary>
        /// Occurs when the processor begins executing a <see cref="InsertInstanceCommand"/>.
        /// </summary>
        public event InsertInstanceCommandBeginEventListener InsertInstanceCommandBegin
        {
            add { _insertInstanceCommandBegin += value; }
            remove { _insertInstanceCommandBegin -= value; }
        }

        /// <summary>
        /// Occurs when the processor finishes executing a <see cref="InsertInstanceCommand"/>.
        /// </summary>
        public event InsertInstanceCommandCompletedEventListener InsertInstanceCommandCompleted
        {
            add { _insertInstanceCommandCompleted += value; }
            remove { _insertInstanceCommandCompleted -= value; }
        }

        /// <summary>
        /// Occurs after the processor has rolled back a <see cref="InsertInstanceCommand"/>.
        /// </summary>
        public event InsertInstanceCommandRolledbackEventListener InsertInstanceCommandRolledback
        {
            add { _insertInstanceCommandRolledback += value; }
            remove { _insertInstanceCommandRolledback -= value; }
        }

        /// <summary>
        /// Occurs when the processor begins executing a <see cref="ServerRulesEngine"/>.
        /// </summary>
        public event RuleEngineExecuteBeginEventListener RuleEngineExecuteBegin
        {
            add { _ruleEngineExecuteBegin += value; }
            remove { _ruleEngineExecuteBegin -= value; }
        }

        /// <summary>
        /// Occurs when the processor finishes executing a <see cref="ServerRulesEngine"/>.
        /// </summary>
        public event RuleEngineExecuteCompletedEventListener RuleEngineExecuteCompleted
        {
            add { _ruleEngineExecuteCompleted += value; }
            remove { _ruleEngineExecuteCompleted -= value; }
        }

        /// <summary>
        /// Occurs when the processor begins loading a <see cref="ServerRulesEngine"/>.
        /// </summary>
        public event RuleEngineLoadBeginEventListener RuleEngineLoadBegin
        {
            add { _ruleEngineLoadBegin += value; }
            remove { _ruleEngineLoadBegin -= value; }
        }

        /// <summary>
        /// Occurs after the processor has finished loading a <see cref="ServerRulesEngine"/>.
        /// </summary>
        public event RuleEngineLoadCompletedEventListener RuleEngineLoadCompleted
        {
            add { _ruleEngineLoadCompleted += value; }
            remove { _ruleEngineLoadCompleted -= value; }
        }

        /// <summary>
        /// Occurs when the processor begins loading a <see cref="DicomFile"/>.
        /// </summary>
        public event DicomFileLoadBeginEventListener DicomFileLoadBegin
        {
            add { _dicomFileLoadBegin += value; }
            remove { _dicomFileLoadBegin -= value; }
        }

        /// <summary>
        /// Occurs after the processor has loaded a <see cref="DicomFile"/>.
        /// </summary>
        public event DicomFileLoadCompletedEventListener DicomFileLoadCompleted
        {
            add { _dicomFileLoadCompleted += value; }
            remove { _dicomFileLoadCompleted -= value; }
        }
        

        #endregion Events

        #region Constructors
        public StudyProcessItemProcessor():base()
        {
            LoadExtensions();
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
                _studyProcessedRulesEngine = CreateStudyProcessedRuleEngine();
                _studyProcessedRulesEngine.Load();
            }
            ServerActionContext context = new ServerActionContext(file,StorageLocation.FilesystemKey, item.ServerPartitionKey,item.StudyStorageKey);

            context.CommandProcessor = new ServerCommandProcessor("Study Rule Processor");

            _studyProcessedRulesEngine.Execute(context);

            if (false == context.CommandProcessor.Execute())
            {
                
            }
        }

        /// <summary>
        /// Method to create an instance of <see cref="ServerRulesEngine"/> to process study rules.
        /// </summary>
        private ServerRulesEngine CreateStudyProcessedRuleEngine()
        {
            ServerRulesEngine theEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("StudyProcessed"));

            theEngine.ExecuteBegin += delegate(ServerRulesEngine engine, ServerActionContext ctx)
                                                           {
                                                               EventsHelper.Fire(_ruleEngineExecuteBegin, engine, ctx);
                                                           };

            theEngine.ExecuteCompleted += delegate(ServerRulesEngine engine, ServerActionContext ctx)
                                                               {
                                                                   EventsHelper.Fire(_ruleEngineExecuteCompleted, engine, ctx);
                                                               };
            theEngine.LoadBegin += delegate(ServerRulesEngine engine)
                                                        {
                                                            EventsHelper.Fire(_ruleEngineLoadBegin, engine);
                                                        };
            theEngine.LoadCompleted += delegate(ServerRulesEngine engine)
                                                            {
                                                                EventsHelper.Fire(_ruleEngineLoadCompleted, engine);
                                                            };

            return theEngine;
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
                CreateSeriesProcessedRuleEngine();

                _seriesProcessedRulesEngine.Load();
            }
            ServerActionContext context = new ServerActionContext(file, StorageLocation.FilesystemKey, item.ServerPartitionKey, item.StudyStorageKey);

            context.CommandProcessor = new ServerCommandProcessor("Series Rule Processor");

            _seriesProcessedRulesEngine.Execute(context);

            if (false == context.CommandProcessor.Execute())
            {

            }
        }

        /// <summary>
        /// Method to create an instance of <see cref="ServerRulesEngine"/> to process series rules.
        /// </summary>
        /// 
        private ServerRulesEngine CreateSeriesProcessedRuleEngine()
        {
            ServerRulesEngine theEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SeriesProcessed"));

            theEngine.ExecuteBegin += delegate(ServerRulesEngine engine, ServerActionContext ctx)
                                            {
                                                EventsHelper.Fire(_ruleEngineExecuteBegin, engine, ctx);
                                            };

            theEngine.ExecuteCompleted += delegate(ServerRulesEngine engine, ServerActionContext ctx)
                                            {
                                                EventsHelper.Fire(_ruleEngineExecuteCompleted, engine, ctx);
                                            };
            theEngine.LoadBegin += delegate(ServerRulesEngine engine)
                                             {
                                                 EventsHelper.Fire(_ruleEngineLoadBegin, engine);
                                             };
            theEngine.LoadCompleted += delegate(ServerRulesEngine engine)
                                             {
                                                 EventsHelper.Fire(_ruleEngineLoadCompleted, engine);
                                             };

            return theEngine;
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

            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM File");
            try
            {
                EventsHelper.Fire(_dicomFileLoadBegin, path);

                file = new DicomFile(path);
                file.Load();

                EventsHelper.Fire(_dicomFileLoadCompleted, file);

                // Get the Patients Name for processing purposes.
                String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");
                String instanceuid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "");

                // Update the StudyStream object
                InsertStreamCommand insertCommand = new InsertStreamCommand(file, stream, StorageLocation);
                insertCommand.ExecuteBegin += delegate(ServerCommand cmd)
                                                  {
                                                      EventsHelper.Fire(_insertStreamCommandBegin, cmd);
                                                  };
                insertCommand.ExecuteCompleted += delegate(ServerCommand cmd)
                                                  {
                                                      EventsHelper.Fire(_insertStreamCommandCompleted, cmd);
                                                  };
                insertCommand.UndoCompleted += delegate(ServerCommand cmd)
                                                  {
                                                      EventsHelper.Fire(_insertStreamCommandRolledback, cmd);
                                                  };

                processor.AddCommand(insertCommand);

                // Insert into the database
                InsertInstanceCommand insertInstanceCommand = new InsertInstanceCommand(file, StorageLocation);
                insertInstanceCommand.ExecuteBegin += delegate(ServerCommand cmd)
                                                  {
                                                      EventsHelper.Fire(_insertInstanceCommandBegin, cmd);
                                                  };
                insertInstanceCommand.ExecuteCompleted += delegate(ServerCommand cmd)
                                                  {
                                                      EventsHelper.Fire(_insertInstanceCommandCompleted, cmd);
                                                  };
                insertInstanceCommand.UndoCompleted += delegate(ServerCommand cmd)
                                                  {
                                                      EventsHelper.Fire(_insertInstanceCommandRolledback, cmd);
                                                  };
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
                    Platform.Log(LogLevel.Error, "Failure processing command: {0}", processor.Description);
                    keys = null;
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
                throw new ApplicationException("Unexpected exception when processing file.", e);
            }

            if (keys != null)
            {
                // We've inserted a new Study, process Study Rules
                if (keys.InsertStudy)
                {
                    ProcessStudyRules(item, file);
                }

                // We've inserted a new Series, process Series Rules
                if (keys.InsertSeries)
                {
                    ProcessSeriesRules(item, file);
                }
            }
        }


        /// <summary>
        /// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item.</param>
        private void ProcessUidList(Model.WorkQueue item)
        {
            StudyXml studyXml;
            
            EventsHelper.Fire(_workQueueUIDListProcessingBegin, WorkQueueUidList);
            
            studyXml = LoadStudyXml(StorageLocation);
            
            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in WorkQueueUidList)
            {
                EventsHelper.Fire(_workQueueUIDProcessingBegin, sop);

                string path = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ".dcm");

                try
                {
                    ProcessFile(item, path,studyXml);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                    EventsHelper.Fire(_workQueueUIDProcessingCompleted, sop, false);
                    continue;
                }

                successfulProcessCount++;

                // Delete it out of the queue
                DeleteWorkQueueUid(sop);

                EventsHelper.Fire(_workQueueUIDProcessingCompleted, sop, true);
            }

            if (successfulProcessCount == 0)
            {
                SetWorkQueueItemPending(item, true); // set failure status, retries will be attempted if necessary
                EventsHelper.Fire(_workQueueUIDListProcessingCompleted, WorkQueueUidList, true);
            }
            else
            {
                SetWorkQueueItemPending(item, false); // set success status}
                EventsHelper.Fire(_workQueueUIDListProcessingCompleted, WorkQueueUidList, false);
            }
            
        }


        private void LoadExtensions()
        {
            StudyProcessItemProcessorExtensionPoint ep = new StudyProcessItemProcessorExtensionPoint();
            try
            {
                object[] ex = ep.CreateExtensions();
                _extensions = new IStudyProcessItemProcessorListener[ex.Length];
                ex.CopyTo(_extensions, 0);

                foreach(IStudyProcessItemProcessorListener listener in _extensions)
                {
                    listener.Initialize(this);
                }
            }
            catch(PluginException e)
            {
                Console.WriteLine(e);
            }
            
        }
        #endregion

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item.
        /// </summary>
        protected override void OnProcess()
        {

            Model.WorkQueue item = WorkQueueItem; // avoid using property everytime for performance purpose

#if DEBUG_TEST
        // Simulate slow processing so that we can stop the service
        // and test that it reset workqueue item when restarted
            Console.WriteLine("WorkQueue Item has been locked for processing...");
            Console.WriteLine("Press <Ctrl-C> to stop the service now\n");
            Thread.Sleep(10000);
            Console.WriteLine("WorkQueue Item is being processed...");
#endif

            //Load the specific UIDs that need to be processed.
            LoadUids(item);

            if (WorkQueueUidList.Count == 0)
            {
                // No UIDs associated with the WorkQueue item.  Set the status back to pending.
                SetWorkQueueItemCompleteIfExpired(item);
            }
            else
            {
                // Load the rules engine
                _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SopProcessed"));
                _sopProcessedRulesEngine.ExecuteBegin += delegate(ServerRulesEngine engine, ServerActionContext ctx)
                                                             {
                                                                 EventsHelper.Fire(_ruleEngineExecuteBegin, engine, ctx);
                                                             };

                _sopProcessedRulesEngine.ExecuteCompleted += delegate(ServerRulesEngine engine, ServerActionContext ctx)
                                                             {
                                                                 EventsHelper.Fire(_ruleEngineExecuteCompleted, engine, ctx);
                                                             };
                _sopProcessedRulesEngine.LoadBegin += delegate(ServerRulesEngine engine)
                                                       {
                                                           EventsHelper.Fire(_ruleEngineLoadBegin, engine);
                                                       };
                _sopProcessedRulesEngine.LoadCompleted += delegate(ServerRulesEngine engine)
                                                       {
                                                           EventsHelper.Fire(_ruleEngineLoadCompleted, engine);
                                                       };
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