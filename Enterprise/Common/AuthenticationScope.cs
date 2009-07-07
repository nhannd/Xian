#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Establishes an authentication scope around a block of code.
	/// </summary>
	/// <remarks>
	/// The purpose of this class is to establish an authentication scope.  Within the scope,
	/// the <see cref="Thread.CurrentPrincipal"/> is set to a value representative of the user
	/// session associated with the scope.  When the scope is disposed, the thread principal
	/// is returned to it's previous value.
	/// </remarks>
	public class AuthenticationScope : IDisposable
	{
		[ThreadStatic]
		private static AuthenticationScope _current;

		private readonly AuthenticationScope _parent;
		private readonly string _userName;
		private readonly string _application;
		private readonly string _hostName;
        private readonly IPrincipal _principal;
		private readonly IPrincipal _previousPrincipal;
		private bool _disposed;


		#region Constructors

		/// <summary>
		/// Creates a new user session based on specified credentials, and constructs an instance of this
		/// class for that user session.
		/// </summary>
		/// <remarks>
		/// The user session is terminated when this instance is disposed.
		/// </remarks>
		/// <param name="userName"></param>
		/// <param name="application"></param>
		/// <param name="hostName"></param>
		/// <param name="password"></param>
		public AuthenticationScope(string userName, string application, string hostName, string password)
		{
			_userName = userName;
			_application = application;
			_hostName = hostName;

			_principal = InitiateSession(password);

			// if the session was successfully initiated (no exception thrown), then 
			// set the thread principal and establish this as the current scope
			_previousPrincipal = Thread.CurrentPrincipal;
			Thread.CurrentPrincipal = _principal;
			_parent = _current;
			_current = this;
		}

		#endregion

		#region Public API

		public static AuthenticationScope Current
		{
			get { return _current; }
		}

		public IPrincipal Principal
		{
			get { return _principal; }
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				if (this != _current)
					throw new InvalidOperationException("Disposed out of order.");

				try
				{
					// attempt to terminate the session
					TerminateSession();
				}
				finally
				{
					// even if it fails, we are still disposing of this scope, so we set the head
					// to point to the parent and revert the Thread.CurrentPrincipal
					_current = _parent;
					Thread.CurrentPrincipal = _previousPrincipal;
				}
			}
		}

		#endregion

		#region Helpers

		private IPrincipal InitiateSession(string password)
		{
            IPrincipal principal = null;
			Platform.GetService<IAuthenticationService>(
				delegate(IAuthenticationService service)
				{
					// obtain session
					InitiateSessionResponse response = service.InitiateSession(
						new InitiateSessionRequest(_userName, _application, _hostName, password, true));

					// create principal
                    principal = DefaultPrincipal.CreatePrincipal(
						new GenericIdentity(_userName),
						response.SessionToken,
						response.AuthorityTokens);

				});
			return principal;
		}

		private void TerminateSession()
		{
			Platform.GetService<IAuthenticationService>(
				delegate(IAuthenticationService service)
				{
					// terminate session
					service.TerminateSession(
						new TerminateSessionRequest(_userName, ((DefaultPrincipal)_principal).SessionToken));
				});
		}

		#endregion
	}
}
