#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Edit.Extensions.LogHistory
{
	/// <summary>
	/// Plugin for WebEditStudy processor to log the history record.
	/// </summary>
	[ExtensionOf(typeof(WebEditStudyProcessorExtensionPoint))]
	public class LogHistory:IWebEditStudyProcessorExtension
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

		public void Initialize()
		{
            
		}

		public void OnStudyEditing(WebEditStudyContext context)
		{
			_studyInfo = StudyInformation.CreateFrom(context.OriginalStudy);
			_changeDesc = new WebEditStudyHistoryChangeDescription
			              	{
			              		UpdateCommands = context.EditCommands,
			              		TimeStamp = Platform.Time,
			              		UserId = context.UserId,
			              		Reason = context.Reason,
                                EditType = context.EditType
			              	};
		}

        
		public void OnStudyEdited(WebEditStudyContext context)
		{
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				Platform.Log(LogLevel.Info, "Logging study history record...");
				IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
				StudyHistoryUpdateColumns recordColumns = CreateStudyHistoryRecord(context);
				StudyHistory entry = broker.Insert(recordColumns);
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

		    var columns = new StudyHistoryUpdateColumns
		                      {
		                          InsertTime = Platform.Time,
		                          StudyStorageKey = context.OriginalStudyStorageLocation.GetKey(),
		                          DestStudyStorageKey = context.NewStudystorageLocation.GetKey(),
		                          StudyData = XmlUtils.SerializeAsXmlDoc(_studyInfo),
		                          StudyHistoryTypeEnum =
		                              context.EditType == EditType.WebEdit
		                                  ? StudyHistoryTypeEnum.WebEdited
		                                  : StudyHistoryTypeEnum.ExternalEdit
		                      };


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