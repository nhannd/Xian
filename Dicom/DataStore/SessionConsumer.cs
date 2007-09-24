using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
	internal abstract class SessionConsumer : IDisposable
	{
		private ISessionManager _sessionManager;

		protected SessionConsumer(ISessionManager sessionManager)
		{
			_sessionManager = sessionManager;	
		}

		~SessionConsumer()
		{
			try
			{
				Dispose(true);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		protected ISessionManager SessionManager
		{
			get { return _sessionManager; }
		}

		protected ISession Session
		{
			get { return SessionManager.Session; }
		}


		protected virtual void Dispose(bool disposing)
		{
			if (_sessionManager != null)
			{
				_sessionManager.Dispose();
				_sessionManager = null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		#endregion
	}
}
