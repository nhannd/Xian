#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Edit.Extensions.LogHistory
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

		public void Initialize()
		{
            
		}

		public void OnStudyEditing(WebEditStudyContext context)
		{
			_studyInfo = StudyInformation.CreateFrom(context.OriginalStudy);
			_changeDesc = new WebEditStudyHistoryChangeDescription();
			_changeDesc.UpdateCommands = context.EditCommands;
		    _changeDesc.TimeStamp = Platform.Time;
		    _changeDesc.UserId = context.UserId;
		    _changeDesc.Reason = context.Reason;
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