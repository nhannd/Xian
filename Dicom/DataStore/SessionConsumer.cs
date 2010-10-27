#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private abstract class SessionConsumer : IDisposable
		{
			private ISessionManager _sessionManager;

			protected SessionConsumer(ISessionManager sessionManager)
			{
				_sessionManager = sessionManager;
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
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e);
				}
			}

			#endregion
		}
	}
}