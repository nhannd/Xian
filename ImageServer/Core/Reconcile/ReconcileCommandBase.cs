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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	internal abstract class ReconcileCommandBase : ServerCommand<ReconcileStudyProcessorContext>, IReconcileServerCommand, IDisposable
	{
	    private UidMapper _uidMapper;

	    /// <summary>
		/// Creates an instance of <see cref="ServerCommand"/>
		/// </summary>
		/// <param name="description"></param>
		/// <param name="requiresRollback"></param>
		/// <param name="context"></param>
		public ReconcileCommandBase(string description, bool requiresRollback, ReconcileStudyProcessorContext context)
			: base(description, requiresRollback, context)
		{
		}

	    protected bool SeriesMappingUpdated
	    {
	        get;
	        set;
	    }

	    public UidMapper UidMapper
	    {
	        get { return _uidMapper; }
	        set { _uidMapper = value; }
	    }

	    protected string GetReconcileUidPath(WorkQueueUid sop)
		{
			if (String.IsNullOrEmpty(sop.RelativePath))
			{
				return Path.Combine(Context.ReconcileWorkQueueData.StoragePath, sop.SopInstanceUid + ".dcm");
			}
			else
				return Path.Combine(Context.ReconcileWorkQueueData.StoragePath, sop.RelativePath);
		}

        
		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				DirectoryUtility.DeleteIfEmpty(Context.ReconcileWorkQueueData.StoragePath);
			}
			catch(IOException ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Unable to cleanup {0}", Context.ReconcileWorkQueueData.StoragePath);
			}
		}

		#endregion

		protected static void FailUid(WorkQueueUid sop, bool retry)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
				if (!retry)
					columns.Failed = true;
				else
				{
					if (sop.FailureCount >= ImageServerCommonConfiguration.WorkQueueMaxFailureCount)
					{
						columns.Failed = true;
					}
					else
					{
						columns.FailureCount = sop.FailureCount++;
					}
				}

				uidUpdateBroker.Update(sop.GetKey(), columns);
				updateContext.Commit();
			}
		}

		protected static StudyXml LoadStudyXml(StudyStorageLocation location)
		{
			StudyXml theXml = new StudyXml(location.StudyInstanceUid);

			String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
			if (File.Exists(streamFile))
			{
				using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
				{
					XmlDocument theDoc = new XmlDocument();

					StudyXmlIo.Read(theDoc, fileStream);

					theXml.SetMemento(theDoc);

					fileStream.Close();
				}
			}

			return theXml;
		}

	    protected void LoadUidMappings()
	    {
	        // Load from history
	        StudyReconcileDescriptor changeDesc = XmlUtils.Deserialize<StudyReconcileDescriptor>(Context.History.ChangeDescription);
	        if (changeDesc.SeriesMappings != null)
	        {
	            _uidMapper = new UidMapper(changeDesc.SeriesMappings);
	        }
	        else
	            _uidMapper = new UidMapper();

	        _uidMapper.SeriesMapUpdated += UidMapper_SeriesMapUpdated;
	    }


        void UidMapper_SeriesMapUpdated(object sender, SeriesMapUpdatedEventArgs e)
        {
            SeriesMappingUpdated = true;
        }

	    protected void UpdateHistory()
	    {
	        using(ServerCommandProcessor processor = new ServerCommandProcessor("Reconcile-CreateStudy-Update History"))
	        {
	            processor.AddCommand(new UpdateHistoryCommand(Context, UidMapper));
	            if (!processor.Execute())
	            {
	                throw new ApplicationException("Unable to update the history", processor.FailureException);
	            }
	        }
	        
	    }
	}
}