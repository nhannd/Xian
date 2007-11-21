using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;
using ClearCanvas.ImageServer;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Performance.WorkQueue.StudyProcess
{
    /// <summary>
    /// Generate the performance statistics for a WorkQueue study processor.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="StudyProcessStatisticsRecorder"/> can be attached to a <see cref="StudyProcessItemProcessor"/> to generate
    /// performance statistics. The statistics is stored in an instance of <see cref="StudyProcessStatistics"/>.</para>
    /// 
    /// <para>
    /// The statistics can be turned off in the application settings entry
    ///     <Performance.WorkQueue.StudyProcess.StudyProcessStatisticsSettings>
    ///        <setting name="Enabled" serializeAs="String">
    ///            <value>False</value>
    ///        </setting>
    ///    </Performance.WorkQueue.StudyProcess.StudyProcessStatisticsSettings>
    /// </para>
    /// </remarks>
    [ExtensionOf(typeof(StudyProcessItemProcessorExtensionPoint))]
    class StudyProcessStatisticsRecorder:IStudyProcessItemProcessorListener
    {
        #region Private members

        private StudyProcessStatistics stats = new StudyProcessStatistics();
        private StudyProcessItemProcessor _processor;

        #endregion

        #region Private methods
        void DicomFileLoadBegin(string path)
        {
            stats.DicomFileLoadBegin(path);
        }
        void DicomFileLoadCompleted(DicomFile file)
        {
            stats.DicomFileLoadEnd(file);
        }


        void RuleEngineExecuteCompleted(ServerRulesEngine engine, ServerActionContext context)
        {
            stats.EngineExecutionEnd();
        }

        void RuleEngineExecuteBegin(ServerRulesEngine engine, ServerActionContext context)
        {
            stats.EngineExecutionBegin();
        }

        void RuleEngineLoadBegin(ServerRulesEngine engine)
        {
            stats.EngineLoadBegin();
        }

        void RuleEngineLoadCompleted(ServerRulesEngine engine)
        {
            stats.EngineLoadEnd();
        }

        void InsertStreamCommandRolledback(InsertStudyXmlCommand cmd)
        {
            stats.InsertStreamEnd();
        }

        void InsertInstanceCommandRolledback(InsertInstanceCommand cmd)
        {
            stats.InsertDBEnd();
        }


        void InsertStreamCommandCompleted(InsertStudyXmlCommand cmd)
        {
            stats.InsertStreamEnd();
        }

        void InsertStreamCommandBegin(InsertStudyXmlCommand cmd)
        {
            stats.InsertStreamBegin();
        }

        void InsertInstanceCommandCompleted(InsertInstanceCommand cmd)
        {
            stats.InsertDBEnd();
        }

        void InsertInstanceCommandBegin(InsertInstanceCommand cmd)
        {
            stats.InsertDBBegin();
        }

        void WorkQueueUIDProcessingCompleted(WorkQueueUid uid, bool status)
        {
            stats.NumInstances++;
        }

        void WorkQueueUIDProcessingBegin(WorkQueueUid uid)
        {
        }

        void ProcessingCompleted(ClearCanvas.ImageServer.Model.WorkQueue item, ProcessResultEnum status)
        {
            stats.End();
            
            if (stats.NumInstances != 0)
            {
                Platform.LogStatistics(LogLevel.Info, stats);
            }


        }

        void ProcessingBegin(ClearCanvas.ImageServer.Model.WorkQueue item)
        {
            stats.Begin();
        }
        
        #endregion

        #region Public properties
        #endregion Public properties

        #region IStudyProcessItemProcessorListener members
        public void Initialize(StudyProcessItemProcessor processor)
        {
            StudyProcessStatisticsSettings settings = StudyProcessStatisticsSettings.Default;

            if (settings.Enabled)
            {
                // register event listeners to all events notified by the processor.
                _processor = processor;
                processor.ProcessingBegin += new ProcessingBeginEventListener(ProcessingBegin);
                processor.ProcessingCompleted += new ProcessingCompletedEventListener(ProcessingCompleted);
                processor.WorkQueueUIDProcessingBegin += new StudyProcessItemProcessor.WorkQueueUIDProcessingBeginEventListener(WorkQueueUIDProcessingBegin);
                processor.WorkQueueUIDProcessingCompleted += new StudyProcessItemProcessor.WorkQueueUIDProcessingCompletedEventListener(WorkQueueUIDProcessingCompleted);
                processor.InsertInstanceCommandBegin += new StudyProcessItemProcessor.InsertInstanceCommandBeginEventListener(InsertInstanceCommandBegin);
                processor.InsertInstanceCommandCompleted += new StudyProcessItemProcessor.InsertInstanceCommandCompletedEventListener(InsertInstanceCommandCompleted);
                processor.InsertInstanceCommandRolledback+=new StudyProcessItemProcessor.InsertInstanceCommandRolledbackEventListener(InsertInstanceCommandRolledback);
                processor.InsertStudyXmlCommandBegin += new StudyProcessItemProcessor.InsertStreamCommandBeginEventListener(InsertStreamCommandBegin);
                processor.InsertStudyXmlCommandCompleted += new StudyProcessItemProcessor.InsertStreamCommandCompletedEventListener(InsertStreamCommandCompleted);
                processor.InsertStudyXmlCommandRolledback += new StudyProcessItemProcessor.InsertStreamCommandRolledbackEventListener(InsertStreamCommandRolledback);

                processor.RuleEngineLoadBegin += new StudyProcessItemProcessor.RuleEngineLoadBeginEventListener(RuleEngineLoadBegin);
                processor.RuleEngineLoadCompleted += new StudyProcessItemProcessor.RuleEngineLoadCompletedEventListener(RuleEngineLoadCompleted);
                processor.RuleEngineExecuteBegin += new StudyProcessItemProcessor.RuleEngineExecuteBeginEventListener(RuleEngineExecuteBegin);
                processor.RuleEngineExecuteCompleted += new StudyProcessItemProcessor.RuleEngineExecuteCompletedEventListener(RuleEngineExecuteCompleted);

                processor.DicomFileLoadBegin+=new StudyProcessItemProcessor.DicomFileLoadBeginEventListener(DicomFileLoadBegin);
                processor.DicomFileLoadCompleted += new StudyProcessItemProcessor.DicomFileLoadCompletedEventListener(DicomFileLoadCompleted);
            }

        }
        #endregion IStudyProcessItemProcessorListener members
        
        
    }
}
