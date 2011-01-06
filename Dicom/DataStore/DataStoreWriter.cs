#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		internal interface IDataStoreWriter : IDisposable
		{
			void StoreStudies(IEnumerable<Study> studies);
		}

		private class DataStoreWriter : SessionConsumer, IDataStoreWriter, IDataStoreStudyRemover
		{
			public DataStoreWriter(ISessionManager sessionManager)
				: base(sessionManager)
			{
			}

			#region IDataStoreWriter Members

			public void StoreStudies(IEnumerable<Study> studies)
			{
				try
				{
					SessionManager.BeginWriteTransaction();
					foreach (Study study in studies)
						Session.SaveOrUpdate(study);
				}
				catch (Exception e)
				{
					SessionManager.Rollback();
					throw new DataStoreException("Failed to commit studies to the data store.", e);
				}
			}

			#endregion

			#region IDataStoreStudyRemover Members

			public void ClearAllStudies()
			{
				try
				{
					using (IDataStoreReader reader = GetIDataStoreReader())
					{
						foreach (Study study in reader.GetStudies())
							File.Delete(study.StudyXmlUri.LocalDiskPath);
					}

					SessionManager.BeginWriteTransaction();
					Session.Delete("from Study");
					SessionManager.Commit();
				}
				catch (Exception e)
				{
					SessionManager.Rollback();
					throw new DataStoreException("Failed to clear all studies from the data store.", e);
				}
			}

			public void RemoveStudy(string studyInstanceUid)
			{
				RemoveStudies(new string[] { studyInstanceUid });
			}

			public void RemoveStudies(IEnumerable<string> studyInstanceUids)
			{
				try
				{
					using (IDataStoreReader reader = GetIDataStoreReader())
					{
						foreach (string studyUid in studyInstanceUids)
						{
							Study study = (Study)reader.GetStudy(studyUid);
							File.Delete(study.StudyXmlUri.LocalDiskPath);
						}
					}

					SessionManager.BeginWriteTransaction();
					foreach (string uid in studyInstanceUids)
					{
						Session.Delete("from Study where StudyInstanceUid_ = ?", uid, NHibernateUtil.String);
					}

					SessionManager.Commit();
				}
				catch (Exception e)
				{
					SessionManager.Rollback();
					throw new DataStoreException("Failed to clear specified studies from the data store.", e);
				}
			}

			#endregion

			protected override void Dispose(bool disposing)
			{
				SessionManager.Commit();
				base.Dispose(disposing);
			}
		}
	}
}