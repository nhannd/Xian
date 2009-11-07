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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class DeletedStudyDataSource 
	{
		private IList<DeletedStudyInfo> _studies;

		public string AccessionNumber { get; set; }

		public DeletedStudyInfo  Find(object key)
		{
			return CollectionUtils.SelectFirst(_studies,
			                                   info => info.RowKey.Equals(key));
		}

		public string PatientId { get; set; }

		public DateTime? StudyDate { get; set; }

		public string DeletedBy { get; set; }

		public string PatientsName { get; set; }

		public string StudyDescription { get; set; }

		private StudyDeleteRecordSelectCriteria GetSelectCriteria()
		{
			StudyDeleteRecordSelectCriteria criteria = new StudyDeleteRecordSelectCriteria();
			if (!String.IsNullOrEmpty(AccessionNumber))
			{
				string key = AccessionNumber.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.AccessionNumber.Like(key);
			}
			if (!String.IsNullOrEmpty(PatientId))
			{
				string key = PatientId.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.PatientId.Like(key);
			}
			if (!String.IsNullOrEmpty(PatientsName))
			{
				string key = PatientsName.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.PatientsName.Like(key);
			}
			if (!String.IsNullOrEmpty(StudyDescription))
			{
				string key = StudyDescription.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.StudyDescription.Like(key);
			}
			if (StudyDate != null)
				criteria.StudyDate.Like("%" + DateParser.ToDicomString(StudyDate.Value) + "%");

			return criteria;
		}

		public IEnumerable Select(int startRowIndex, int maxRows)
		{
			
			IStudyDeleteRecordEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IStudyDeleteRecordEntityBroker>();
			StudyDeleteRecordSelectCriteria criteria = GetSelectCriteria();
			criteria.Timestamp.SortDesc(0);
			IList<StudyDeleteRecord> list = broker.Find(criteria, startRowIndex, maxRows);

			_studies = CollectionUtils.Map(
				list, (StudyDeleteRecord record) => DeletedStudyInfoAssembler.CreateDeletedStudyInfo(record));

			// Additional filter: DeletedBy
            if (String.IsNullOrEmpty(DeletedBy)==false)
            {
                _studies = CollectionUtils.Select(_studies, delegate(DeletedStudyInfo record)
                                       {
                                           if (String.IsNullOrEmpty(record.UserId) || String.IsNullOrEmpty(record.UserName))
                                               return false;

                                           // either the id or user matches
                                           return record.UserId.ToUpper().IndexOf(DeletedBy.ToUpper()) >= 0 ||
                                                  record.UserName.ToUpper().IndexOf(DeletedBy.ToUpper()) >= 0;
                                       });
            }

			return _studies;
		
		}

		public int SelectCount()
		{
			StudyDeleteRecordSelectCriteria criteria = GetSelectCriteria();

            IStudyDeleteRecordEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IStudyDeleteRecordEntityBroker>();
		    return broker.Count(criteria);
		}
	}

	internal static class DeletedStudyInfoAssembler
	{
		public static DeletedStudyInfo CreateDeletedStudyInfo(StudyDeleteRecord record)
		{
			Filesystem fs = Filesystem.Load(record.FilesystemKey);

		    StudyDeleteExtendedInfo extendedInfo = XmlUtils.Deserialize<StudyDeleteExtendedInfo>(record.ExtendedInfo);
			DeletedStudyInfo info = new DeletedStudyInfo
			                        	{
			                        		DeleteStudyRecord = record.GetKey(),
			                        		RowKey = record.GetKey().Key,
			                        		StudyInstanceUid = record.StudyInstanceUid,
			                        		PatientsName = record.PatientsName,
			                        		AccessionNumber = record.AccessionNumber,
			                        		PatientId = record.PatientId,
			                        		StudyDate = record.StudyDate,
			                        		PartitionAE = record.ServerPartitionAE,
			                        		StudyDescription = record.StudyDescription,
			                        		BackupFolderPath = fs.GetAbsolutePath(record.BackupPath),
			                        		ReasonForDeletion = record.Reason,
			                        		DeleteTime = record.Timestamp,
			                        		UserName = extendedInfo.UserName,
			                        		UserId = extendedInfo.UserId
			                        	};
			if (record.ArchiveInfo!=null)
				info.Archives = XmlUtils.Deserialize<DeletedStudyArchiveInfoCollection>(record.ArchiveInfo);

            
			return info;
		}
	}
}