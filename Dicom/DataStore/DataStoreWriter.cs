using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private class DataStoreWriter : SessionConsumer, IDataStoreWriter
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
					throw new DataStoreException("Failed to save Sop Instance(s) to the data store", e);
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
					throw new DataStoreException("Failed to save Series to the data store", e);
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
					throw new DataStoreException("Failed to save Study(s) to the data store.", e);
				}
			}

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
					throw new DataStoreException("Failed to clear all studies from the data store.", e);
				}
			}

			public void RemoveSopInstances(IEnumerable<ISopInstance> sops)
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (ISopInstance sop in sops)
							Session.Delete("from SopInstance where SopInstanceUid_ = ?", sop.GetSopInstanceUid().ToString(), NHibernateUtil.String);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException("Failed to clear the specified sop instance(s) from the data store.", e);
				}
			}

			public void RemoveSeries(IEnumerable<ISeries> series)
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (ISeries deleteSeries in series)
							Session.Delete("from Series where SeriesInstanceUid_ = ?", deleteSeries.GetSeriesInstanceUid().ToString(), NHibernateUtil.String);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException("Failed to clear the specified series from the data store.", e);
				}
			}

			public void RemoveStudies(IEnumerable<IStudy> studies)
			{
				try
				{
					using (IWriteTransaction transaction = SessionManager.GetWriteTransaction())
					{
						foreach (IStudy study in studies)
							Session.Delete("from Study where StudyInstanceUid_ = ?", study.GetStudyInstanceUid().ToString(), NHibernateUtil.String);

						transaction.Commit();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException("Failed to clear the specified study(s) from the data store.", e);
				}
			}

			#endregion
		}
	}
}