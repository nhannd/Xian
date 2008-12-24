using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy.Extensions.LogHistory
{
    /// <summary>
    /// Plugin for WebEditStudy processor to log the history record.
    /// </summary>
    [ExtensionOf(typeof(WebEditStudyProcessorExtensionPoint))]
    class LogHistory:IWebEditStudyProcessorExtension
    {
        #region Private Fields
        private StudyInformation _studyInfo;
        private WebEditStudyHistoryChangeDescription _changeDesc;
        #endregion

        #region IWebEditStudyProcessorExtension Members

        public bool Enabled
        {
            get { return true; } // TODO: Load from config 
        }

        public void Initialize(WebEditStudyItemProcessor workQueueProcessor)
        {
            
        }

        public void OnStudyEditing(WebEditStudyContext context)
        {
            _studyInfo = StudyInformation.CreateFrom(context.OriginalStudy);
            _changeDesc = new WebEditStudyHistoryChangeDescription();
            _changeDesc.UpdateCommands = context.EditCommands;

        }

        
        public void OnStudyEdited(WebEditStudyContext context)
        {
            StudyHistory entry = null;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                Platform.Log(LogLevel.Info, "Logging study history record...");
                IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistoryUpdateColumns recordColumns = CreateStudyHistoryRecord(context);
                entry = broker.Insert(recordColumns);
                if (entry != null)
                    ctx.Commit();
                else
                    throw new ApplicationException("Unable to log study history record");
            }
        }
        
        #endregion

        #region Private Methods
        private StudyHistoryUpdateColumns CreateStudyHistoryRecord(WebEditStudyContext context)
        {
            Platform.CheckForNullReference(context.OriginalStudyStorageLocation, "context.OriginalStudyStorageLocation");
            Platform.CheckForNullReference(context.NewStudystorageLocation, "context.NewStudystorageLocation");

            StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
            columns.InsertTime = Platform.Time;
            columns.StudyHistoryTypeEnum = StudyHistoryTypeEnum.WebEdited; // TODO: 
            columns.StudyStorageKey = context.OriginalStudyStorageLocation.GetKey();
            columns.DestStudyStorageKey = context.NewStudystorageLocation.GetKey();

            columns.StudyData = XmlUtils.SerializeAsXmlDoc(_studyInfo);
            XmlDocument doc = XmlUtils.SerializeAsXmlDoc(_changeDesc);
            columns.ChangeDescription = doc;
            return columns;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
        
    }
}