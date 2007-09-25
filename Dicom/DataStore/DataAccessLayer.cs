using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using NHibernate.Collection;

namespace ClearCanvas.Dicom.DataStore
{
	internal interface IReadTransaction : IDisposable
	{
	}

	internal interface IWriteTransaction : IDisposable
	{
		void Commit();
	}

	internal interface ISessionManager : IDisposable
	{
		ISession Session { get; }
		IReadTransaction GetReadTransaction();
		IWriteTransaction GetWriteTransaction();
	}

	public sealed partial class DataAccessLayer
    {
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

		private class SessionManager : ISessionManager
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
						_session = Instance.SessionFactory.OpenSession();
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
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}

		private readonly static object _syncLock = new object();
		private static volatile DataAccessLayer _instance;

	    private readonly Configuration _hibernateConfiguration;
		private readonly ISessionFactory _sessionFactory;

		private readonly Dictionary<string, IDicomDictionary> _dicomDictionaries;

		private DataAccessLayer()
		{
			_hibernateConfiguration = new Configuration();
			string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
			HibernateConfiguration.Configure(assemblyName + ".cfg.xml");
			HibernateConfiguration.AddAssembly(assemblyName);
			_sessionFactory = HibernateConfiguration.BuildSessionFactory();

			_dicomDictionaries = new Dictionary<string, IDicomDictionary>();
		}

		private static DataAccessLayer Instance
		{
			get
			{
				if (_instance == null)
				{
					lock(_syncLock)
					{
						if (_instance == null)
							_instance = new DataAccessLayer();
					}
				}

				return _instance;
			}
		}

		private Dictionary<string, IDicomDictionary> DicomDictionaries
		{
			get { return _dicomDictionaries; }
		}

        private Configuration HibernateConfiguration
        {
            get { return _hibernateConfiguration; }
        }

		private ISessionFactory SessionFactory
		{
			get { return _sessionFactory; }
		}

		public static IDataStoreReader GetIDataStoreReader()
        {
			return new DataStoreReader(SessionManager.Get());
        }

		public static IDicomPersistentStore GetIDicomPersistentStore()
		{
			return new DicomPersistentStore();
		}

		public static IDataStoreStudyRemover GetIDataStoreStudyRemover()
		{
			return new DataStoreWriter(SessionManager.Get());
		}

		internal static IDataStoreWriter GetIDataStoreWriter()
        {
			return new DataStoreWriter(SessionManager.Get());
        }
		
		internal static IDicomDictionary GetIDicomDictionary()
		{
			return GetIDicomDictionary(DicomDictionary.DefaultDictionaryName);
		}

		internal static IDicomDictionary GetIDicomDictionary(string dictionaryName)
		{
			lock (_syncLock)
			{
				if (Instance.DicomDictionaries.ContainsKey(dictionaryName))
					return Instance.DicomDictionaries[dictionaryName];

				DicomDictionary newDictionary = new DicomDictionary(SessionManager.Get(), dictionaryName);
				Instance.DicomDictionaries[dictionaryName] = newDictionary;
				return newDictionary;
			}
		}

		internal static void InitializeAssociatedObject(object primaryObject, object associatedObject)
		{
			PersistentCollection associatedCollection = associatedObject as PersistentCollection;
			Platform.CheckForNullReference(primaryObject, "primaryObject");
			Platform.CheckForNullReference(associatedCollection, "associatedCollection");
			if (associatedCollection.WasInitialized)
				return;

			using (ISessionManager sessionManager = SessionManager.Get())
			{
				try
				{
					using (sessionManager.GetReadTransaction())
					{
						sessionManager.Session.Lock(primaryObject, LockMode.Read);
						NHibernateUtil.Initialize(associatedCollection);
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatFailedToInitializeAssociatedCollection, associatedObject.GetType(), primaryObject.GetType());
					throw new DataStoreException(message, e);
				}
			}
		}
    }
}
