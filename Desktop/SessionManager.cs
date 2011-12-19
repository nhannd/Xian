#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public enum SessionStatus
	{
		/// <summary>
		/// Operating as a standalone installation, without an enterprise server.
		/// </summary>
		LocalOnly = 0,

		/// <summary>
		/// Not yet determined.
		/// </summary>
		Unknown,

		/// <summary>
		/// Online
		/// </summary>
		Online,

		/// <summary>
		/// Online, but session has expired, and user must re-authenticate.
		/// </summary>
		Expired,

		/// <summary>
		/// Offline
		/// </summary>
		Offline
	}

	public class SessionStatusChangedEventArgs : EventArgs
	{
		internal SessionStatusChangedEventArgs(SessionStatus oldStatus, SessionStatus newStatus)
		{
			OldStatus = oldStatus;
			NewStatus = newStatus;
		}

		public SessionStatus OldStatus { get; private set; }
		public SessionStatus NewStatus { get; private set; }
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="SessionManagerExtensionPoint"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A session manager extension is optional.  If present, the application will load the session manager and
	/// call its <see cref="InitiateSession"/> and <see cref="TerminateSession"/> at the beginning and end
	/// of the applications execution, respectively.
	/// </para>
	/// <para>
	/// The purpose of the session manager is to provide a hook through which custom session management can occur.
	/// A typical session manager implemenation may show a login dialog at start-up in order to gather user credentials,
	/// and may perform other custom initialization.
	/// </para>
	/// </remarks>
	public interface ISessionManager
	{
		/// <summary>
		/// Called by the framework at start-up to initiate a session.
		/// </summary>
		/// <remarks>
		/// This method is called after the GUI system and application view have been initialized,
		/// so the implementation may interact with the user if necessary, and may
		/// make use of the <see cref="Application"/> object.  However, no desktop windows exist yet.
		/// Any exception thrown from this method will effectively prevent the establishment of a session, causing
		/// execution to terminate with an error.  A return value of false may be used
		/// to silently refuse initiation of a session.  In this case, no error is reported, but the application
		/// terminates immediately.
		/// </remarks>
		bool InitiateSession();

		/// <summary>
		/// Called by the framework at shutdown to terminate an existing session.
		/// </summary>
		/// <remarks>
		/// This method is called prior to terminating the GUI subsytem and application view, so the
		/// implementation may interact with the user if necessary.
		/// </remarks>
		void TerminateSession();

		SessionStatus SessionStatus { get; }
		event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;
	}

	#region Static Part

	public partial class SessionManager
	{
		#region Default Session Manager Implementation

		private class Default : SessionManager
		{
			public Default()
				: base(SessionStatus.LocalOnly)
			{
			}

			protected override bool InitiateSession()
			{
				return true;
			}

			protected override void TerminateSession()
			{
			}
		}

		#endregion

		static SessionManager()
		{
			Current = Create();
		}

		internal static readonly ISessionManager Current;

		internal static ISessionManager Create()
		{
			try
			{
				var sessionManager = (ISessionManager)(new SessionManagerExtensionPoint()).CreateExtension();
				Platform.Log(LogLevel.Debug, string.Format("Using session manager extension: {0}", sessionManager.GetType().FullName));
				return sessionManager;
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No session manager extension found");
				return new Default();
			}
		}
	}

	#endregion

	#region Instance Part

	public abstract partial class SessionManager : ISessionManager
	{
		private readonly object _syncLock = new object();
		private SessionStatus _sessionStatus;
		private event EventHandler<SessionStatusChangedEventArgs> _sessionStatusChanged;

		protected SessionManager()
		{
		}

		protected SessionManager(SessionStatus initialStatus)
		{
			_sessionStatus = initialStatus;
		}

		#region ISessionManager Members

		bool ISessionManager.InitiateSession()
		{
			return InitiateSession();
		}

		void ISessionManager.TerminateSession()
		{
			TerminateSession();
		}

		public SessionStatus SessionStatus
		{
			get
			{
				lock (_syncLock)
				{
					return _sessionStatus;
				}
			}
			protected set
			{
				lock (_syncLock)
				{
					if (_sessionStatus == value)
						return;

					var old = _sessionStatus;
					_sessionStatus = value;
					NotifyStatusChanged(old, value);
				}
			}
		}

		public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged
		{
			add
			{
				lock (_syncLock)
				{
					_sessionStatusChanged += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_sessionStatusChanged -= value;
				}
			}
		}

		#endregion

		protected abstract bool InitiateSession();
		protected abstract void TerminateSession();

		protected virtual void OnStatusChanged(SessionStatus oldStatus, SessionStatus newStatus)
		{
		}

		private void NotifyStatusChanged(SessionStatus oldStatus, SessionStatus newStatus)
		{
			Action<object> statusChanged = ignored => OnStatusChanged(oldStatus, newStatus);
			Application.MarshalDelegate(statusChanged, this);

			Delegate[] delegates;
			lock (_syncLock)
			{
				if (_sessionStatusChanged == null)
					return;

				delegates = _sessionStatusChanged.GetInvocationList();
			}

			var args = new SessionStatusChangedEventArgs(oldStatus, newStatus);
			foreach (var @delegate in delegates)
			{
				if (!Application.MarshalDelegate(@delegate, this, args))
					@delegate.DynamicInvoke(this, args);
			}
		}
	}

	#endregion
}