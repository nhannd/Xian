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
using System.Collections.Generic;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private class DataStoreWriter : SessionConsumer, IDataStoreWriter, IDataStoreStudyRemover
		{
			public DataStoreWriter(ISessionManager sessionManager)
				: base(sessionManager)
			{
			}

			#region IDataStoreWriter Members

			public void StoreSopInstances(IEnumerable<SopInstance> sops)
			{
				try
				{
					base.SessionManager.BeginWriteTransaction();
					foreach (SopInstance sop in sops)
						Session.SaveOrUpdate(sop);
				}
				catch (Exception e)
				{
					base.SessionManager.Rollback();
					throw new DataStoreException(SR.ExceptionFailedToStoreSopInstances, e);
				}
			}

			public void StoreSeries(IEnumerable<Series> series)
			{
				try
				{
					base.SessionManager.BeginWriteTransaction();
					foreach (Series saveSeries in series)
						Session.SaveOrUpdate(saveSeries);
				}
				catch (Exception e)
				{
					base.SessionManager.Rollback();
					throw new DataStoreException(SR.ExceptionFailedToStoreSeries, e);
				}
			}

			public void StoreStudies(IEnumerable<Study> studies)
			{
				try
				{
					base.SessionManager.BeginWriteTransaction();
					foreach (Study study in studies)
						Session.SaveOrUpdate(study);
				}
				catch (Exception e)
				{
					base.SessionManager.Rollback();
					throw new DataStoreException(SR.ExceptionFailedToStoreStudies, e);
				}
			}

			#endregion

			#region IDataStoreStudyRemover Members

			public void ClearAllStudies()
			{
				try
				{
					base.SessionManager.BeginWriteTransaction();
					Session.Delete("from Study");
					base.SessionManager.Commit();
				}
				catch (Exception e)
				{
					base.SessionManager.Rollback();
					throw new DataStoreException(SR.ExceptionFailedToClearAllStudies, e);
				}
			}

			public void RemoveStudy(string studyUid)
			{
				RemoveStudies(new string[] { studyUid });
			}

			public void RemoveStudies(IEnumerable<string> studyUids)
			{
				try
				{
					base.SessionManager.BeginWriteTransaction();
					foreach (string uid in studyUids)
						Session.Delete("from Study where StudyInstanceUid_ = ?", uid.ToString(), NHibernateUtil.String);

					base.SessionManager.Commit();
				}
				catch (Exception e)
				{
					base.SessionManager.Rollback();
					throw new DataStoreException(SR.ExceptionFailedToClearStudies, e);
				}
			}

			#endregion

			protected override void Dispose(bool disposing)
			{
				base.SessionManager.Commit();
				base.Dispose(disposing);
			}
		}
	}
}