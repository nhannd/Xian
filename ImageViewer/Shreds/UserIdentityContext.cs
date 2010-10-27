#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds
{
	/// <summary>
	/// Represents a stored set of user credentials.
	/// </summary>
	internal class UserIdentityContext : IDisposable
	{
		/// <summary>
		/// Initializes a default instance of <see cref="UserIdentityContext"/> without any credentials.
		/// </summary>
		public UserIdentityContext()
		{
			Disposed = false;
		}

		~UserIdentityContext()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases the resources associated with this <see cref="UserIdentityContext"/>.
		/// </summary>
		public void Dispose()
		{
			Disposed = true;
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex);
			}
		}

		/// <summary>
		/// Impersonates the set of user credentials. The returned instance should be disposed of as soon as impersonation is no longer necessary.
		/// </summary>
		/// <returns>An impersonation context object that should be disposed when impersonation is no longer necessary. May be null if impersonation is not possible.</returns>
		public IDisposable Impersonate()
		{
			if (Disposed)
				Platform.Log(LogLevel.Debug, new ObjectDisposedException(GetType().FullName), "An attempt was made to impersonate a user context that has already been disposed.");

			return CreateImpersonationContext();
		}

		/// <summary>
		/// Gets a value indicating whether or not this <see cref="UserIdentityContext"/> has previously been disposed.
		/// </summary>
		protected bool Disposed { get; private set; }

		/// <summary>
		/// Called to dispose any resources held by this <see cref="UserIdentityContext"/>.
		/// </summary>
		/// <param name="disposing">True if the object is being disposed; False if the object is being finalized.</param>
		protected virtual void Dispose(bool disposing) {}

		/// <summary>
		/// Called to create an impersonation context object.
		/// </summary>
		/// <returns>An impersonation context object that can be disposed when impersonation is no longer necessary. May be null if impersonation is not possible.</returns>
		protected virtual IDisposable CreateImpersonationContext()
		{
			return null;
		}

		//TODO (CR Sept 2010): long name; just call it "Default"?
		/// <summary>
		/// Gets a static instance of an empty <see cref="UserIdentityContext"/>.
		/// </summary>
		public static readonly UserIdentityContext DefaultUserIdentityContext = new UserIdentityContext();

		/// <summary>
		/// Creates a new instance of a <see cref="UserIdentityContext"/> representing the <see cref="Thread.CurrentPrincipal">current thread's principal</see>.
		/// </summary>
		/// <returns>A new instance of a <see cref="UserIdentityContext"/> representing the <see cref="Thread.CurrentPrincipal">current thread's principal</see>.</returns>
		public static UserIdentityContext CreateFromCurrentThreadPrincipal()
		{
			try
			{
				return new WindowsClientUserContext(Thread.CurrentPrincipal.Identity as WindowsIdentity);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Exception thrown when accessing the identity of the current thread principal.");
			}
			return new UserIdentityContext();
		}

		#region WindowsClientUserContext Class

		private class WindowsClientUserContext : UserIdentityContext
		{
			private WindowsIdentity _windowsIdentity;

			/// <summary>
			/// Initializes an instance of <see cref="UserIdentityContext"/> from the specified <see cref="WindowsIdentity"/> without taking ownership of the provided identity object.
			/// </summary>
			/// <param name="sourceWindowsIdentity">The <see cref="WindowsIdentity"/> from which the identity information is copied. May be null.</param>
			public WindowsClientUserContext(WindowsIdentity sourceWindowsIdentity)
			{
				try
				{
					_windowsIdentity = sourceWindowsIdentity != null ? new WindowsIdentity(sourceWindowsIdentity.Token) : null;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "Exception thrown when creating identity object.");
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (_windowsIdentity != null)
					{
						_windowsIdentity.Dispose();
						_windowsIdentity = null;
					}
				}
				base.Dispose(disposing);
			}

			protected override IDisposable CreateImpersonationContext()
			{
				try
				{
					return _windowsIdentity != null ? _windowsIdentity.Impersonate() : null;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "Exception thrown when impersonating identity.");
				}
				return null;
			}
		}

		#endregion
	}
}