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
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Command for inserting an entry for a dicom file in the Study Integrity Queue
	/// </summary>
	class InsertSIQReconcileStudyCommand : ServerDatabaseCommand
	{
		private readonly ReconcileImageContext _context;

		public InsertSIQReconcileStudyCommand(ReconcileImageContext context)
			: base("InsertSIQReconcileStudyCommand", true)
		{
			Platform.CheckForNullReference(context, "context");
			_context = context;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor, ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
		{
			Platform.CheckForNullReference(_context, "_context");
			Platform.CheckForNullReference(_context.Partition, "_context.Partition");
			Platform.CheckForNullReference(_context.CurrentStudyLocation, "_context.CurrentStudyLocation");
			Platform.CheckForNullReference(_context.File, "_context.File");

			XmlDocument descXml = XmlUtils.SerializeAsXmlDoc(_context.ImageSet);

			ReconcileStudyQueueDescription queueDesc = GetQueueEntryDescription();

			IInsertStudyIntegrityQueue broker = updateContext.GetBroker<IInsertStudyIntegrityQueue>();
			InsertStudyIntegrityQueueParameters parameters = new InsertStudyIntegrityQueueParameters();
			parameters.Description = queueDesc.ToString();
			parameters.StudyInstanceUid = _context.CurrentStudyLocation.StudyInstanceUid;
			parameters.ServerPartitionKey = _context.Partition.GetKey();
			parameters.StudyStorageKey = _context.CurrentStudyLocation.GetKey();
			parameters.StudyIntegrityReasonEnum = StudyIntegrityReasonEnum.InconsistentData;
			parameters.SeriesInstanceUid = _context.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
			parameters.SeriesDescription = _context.File.DataSet[DicomTags.SeriesDescription].GetString(0, String.Empty);
			parameters.SopInstanceUid = _context.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
			parameters.StudyData = descXml;

			StudyIntegrityQueue item = broker.FindOne(parameters);
			if (item==null)
			{
				throw new ApplicationException("Unable to update reconcile queue");
			}

			_context.ReconcileQueue = item;
			if (item.QueueData == null)
			{
				// this is new entry, need to assign the path and update the entry
				_context.StoragePath = GetInitialReconcileStoragePath();
                
				ReconcileStudyWorkQueueData data = new ReconcileStudyWorkQueueData();
				data.StoragePath = _context.StoragePath;
				data.Details = new ImageSetDetails(_context.File.DataSet);
			    data.Details.InsertFile(_context.File);
				XmlDocument xmlQueueData = XmlUtils.SerializeAsXmlDoc(data);

				item.QueueData = xmlQueueData;

				IStudyIntegrityQueueEntityBroker updateBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
				updateBroker.Update(item);
			}
			else
			{
				// Need to re-use the path that's already assigned for this entry
				ReconcileStudyWorkQueueData data = XmlUtils.Deserialize < ReconcileStudyWorkQueueData>(item.QueueData);
				_context.StoragePath = data.StoragePath;

				data.Details.InsertFile(_context.File);
                
				XmlDocument updatedQueueDataXml = XmlUtils.SerializeAsXmlDoc(data);
				IStudyIntegrityQueueEntityBroker updateBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
				StudyIntegrityQueueUpdateColumns columns = new StudyIntegrityQueueUpdateColumns();
				columns.QueueData = updatedQueueDataXml;
				updateBroker.Update(item.GetKey(), columns);

			}
		}

		private string GetInitialReconcileStoragePath()
		{
			String path =
				Path.Combine(_context.CurrentStudyLocation.FilesystemPath, _context.CurrentStudyLocation.PartitionFolder);
			path = Path.Combine(path, "Reconcile");
			path = Path.Combine(path, Guid.NewGuid().ToString());
			return path;
		}

		private ReconcileStudyQueueDescription GetQueueEntryDescription()
		{
			ReconcileStudyQueueDescription desc = new ReconcileStudyQueueDescription();
			desc.ExistingPatientId = _context.CurrentStudy.PatientId;
			desc.ExistingPatientName = _context.CurrentStudy.PatientsName;
			desc.ExistingAccessionNumber = _context.CurrentStudy.AccessionNumber;
            
			desc.ConflictingPatientName = _context.File.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
			desc.ConflictingPatientId = _context.File.DataSet[DicomTags.PatientId].GetString(0, String.Empty);
			desc.ConflictingAccessionNumber = _context.File.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty);
            
			return desc;
		}
	}
}