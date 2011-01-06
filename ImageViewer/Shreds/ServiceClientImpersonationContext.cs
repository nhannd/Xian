#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Principal;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds
{
	/// <summary>
	/// Wrapper for an impersonation context representing the Windows credentials of the WCF client during the lifetime of an instance of this class.
	/// </summary>
	/// <remarks>
	/// To impersonate the Windows credentials of the WCF client, construct a new instance of <see cref="ServiceClientImpersonationContext"/> before
	/// executing the statements that require the credentials. After the statements have executed, <see cref="Dispose"/> the instance to revert the context.
	/// </remarks>
	internal sealed class ServiceClientImpersonationContext : IDisposable
	{
		private WindowsImpersonationContext _windowsImpersonationContext;

		/// <summary>
		/// Initializes impersonation of the WCF client's Windows credentials, if possible.
		/// </summary>
		public ServiceClientImpersonationContext()
		{
			try
			{
				var securityContext = ServiceSecurityContext.Current;
				_windowsImpersonationContext = securityContext != null && securityContext.WindowsIdentity != null ? securityContext.WindowsIdentity.Impersonate() : null;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Exception thrown when accessing the security context of the service call for identity impersonation.");
			}
		}

		/// <summary>
		/// Ends impersonation of the WCF client's Windows credentials.
		/// </summary>
		public void Dispose()
		{
			if (_windowsImpersonationContext != null)
			{
				_windowsImpersonationContext.Dispose();
				_windowsImpersonationContext = null;
			}
		}
	}
}