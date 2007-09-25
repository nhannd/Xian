using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using ClearCanvas.Common;

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
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (SopInstance sop in sops)
							Session.SaveOrUpdate(sop);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToStoreSopInstances, e);
				}
			}

			public void StoreSeries(IEnumerable<Series> series)
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (Series saveSeries in series)
							Session.SaveOrUpdate(saveSeries);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToStoreSeries, e);
				}
			}

			public void StoreStudies(IEnumerable<Study> studies)
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (Study study in studies)
							Session.SaveOrUpdate(study);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToStoreStudies, e);
				}
			}

			#endregion

			#region IDataStoreStudyRemover Members

			public void ClearAllStudies()
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						Session.Delete("from Study");
						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToClearAllStudies, e);
				}
			}

			public void RemoveStudy(Uid studyUid)
			{
				RemoveStudies(new Uid[] { studyUid });
			}

			public void RemoveStudies(IEnumerable<Uid> studyUids)
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (Uid uid in studyUids)
							Session.Delete("from Study where StudyInstanceUid_ = ?", uid.ToString(), NHibernateUtil.String);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToClearStudies, e);
				}
			}

			#endregion
		}
	}
}