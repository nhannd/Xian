using System;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		internal interface IReadTransaction : IDisposable
		{
		}

		internal interface IWriteTransaction : IDisposable
		{
			void Commit();
		}

		private sealed class ReadTransaction : IReadTransaction
		{
			private ITransaction _transaction;

			public ReadTransaction(ISession session)
			{
				_transaction = session.BeginTransaction();
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (_transaction != null)
				{
					_transaction.Dispose();
					_transaction = null;
				}
			}

			#endregion
		}

		private sealed class WriteTransaction : IWriteTransaction
		{
			private ITransaction _transaction;

			public WriteTransaction(ISession session)
			{
				_transaction = session.BeginTransaction();
			}

			#region IWriteTransaction Members

			public void Commit()
			{
				if (_transaction != null)
				{
					_transaction.Commit();
					_transaction.Dispose();
					_transaction = null;
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_transaction != null)
				{
					_transaction.Rollback();
					_transaction.Dispose();
					_transaction = null;
				}
			}

			#endregion
		}
	}
}