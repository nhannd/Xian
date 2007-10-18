using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		internal interface ISessionManager : IDisposable
		{
			ISession Session { get; }
			IReadTransaction GetReadTransaction();
			IWriteTransaction GetWriteTransaction();
		}

		private sealed class SessionManager : ISessionManager
		{
			private static readonly object _syncLock = new object();
			private static readonly List<SessionManager> _sessionManagers = new List<SessionManager>();

			private readonly Thread _thread;
			private ISession _session;
			private int _referenceCount;
			private volatile bool _resetOnNextRead;

			private SessionManager()
			{
				_thread = Thread.CurrentThread;
				_referenceCount = 0;
				_resetOnNextRead = false;
			}

			private Thread Thread
			{
				get { return _thread; }
			}

			private void IncrementReferenceCount()
			{
				Interlocked.Increment(ref _referenceCount);
			}

			private void DecrementReferenceCount()
			{
				Interlocked.Decrement(ref _referenceCount);
			}

			private int ReferenceCount
			{
				get { return Thread.VolatileRead(ref _referenceCount); }
			}

			private void DisconnectSession()
			{
				_resetOnNextRead = false;

				if (_session != null)
				{
					_session.Disconnect();
					_session.Clear();
					_session.Close();
					_session.Dispose();
					_session = null;
				}
			}

			public static ISessionManager Get()
			{
				SessionManager manager = null;
				lock (_syncLock)
				{
					manager = _sessionManagers.Find(
					   delegate(SessionManager test) { return test.Thread.Equals(Thread.CurrentThread); });

					if (manager == null)
					{
						manager = new SessionManager();
						_sessionManagers.Add(manager);
					}
				}

				manager.IncrementReferenceCount();
				return manager;
			}

			#region ISessionManager Members

			public ISession Session
			{
				get
				{
					if (!Thread.Equals(Thread.CurrentThread))
						throw new DataStoreException(SR.ExceptionSessionsCanOnlyBeUsedOnOneThread);

					if (_session == null)
					{
						_session = SessionFactory.OpenSession();
						_session.FlushMode = FlushMode.Commit;
					}

					return _session;
				}
			}

			public IReadTransaction GetReadTransaction()
			{
				if (_resetOnNextRead)
					DisconnectSession();

				return new ReadTransaction(Session);
			}

			public IWriteTransaction GetWriteTransaction()
			{
				_resetOnNextRead = true;
				return new WriteTransaction(Session);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					DecrementReferenceCount();
					if (this.ReferenceCount <= 0)
					{
						lock (_syncLock)
						{
							_sessionManagers.Remove(this);
						}

						DisconnectSession();
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}
	}
}