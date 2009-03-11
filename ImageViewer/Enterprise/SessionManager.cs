#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.ImageViewer.Enterprise
{
	[ExtensionPoint]
	public class LoginDialogExtensionPoint : ExtensionPoint<ILoginDialog>
	{
	}

	[ExtensionPoint]
	public class ChangePasswordDialogExtensionPoint : ExtensionPoint<IChangePasswordDialog>
	{
	}


	[ExtensionOf(typeof(SessionManagerExtensionPoint))]
	public class SessionManager : ISessionManager
	{
		public SessionManager()
		{
		}
		
		#region ISessionManager Members

		bool ISessionManager.InitiateSession()
		{
			try
			{
				return Login();
			}
			catch (Exception e)
			{
				// can't use ExceptionHandler here because no desktopWindow exists yet
				Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
				return false;
			}
		}

		void ISessionManager.TerminateSession()
		{
			try
			{
				Logout();
			}
			catch (Exception e)
			{
				// since we're logging out, just log the exception and move on
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		#region Static Methods

		#region Internal methods

		internal static bool RenewLogin()
		{
			if (Session.Current == null)
				throw new InvalidOperationException("Unable to renew login; no one is currently logged in.");

			return Login(LoginDialogMode.RenewLogin, Session.Current.Principal.Identity.Name);
		}

		internal static bool ChangePassword()
		{
			if (Session.Current == null)
				throw new InvalidOperationException("Unable to change password; no one is currently logged in.");

			string newPassword;
			if (ChangePassword(Session.Current.Principal.Identity.Name, null, out newPassword))
			{
				try
				{
					Login(Session.Current.Principal.Identity.Name, newPassword);
				}
				catch(Exception e)
				{
					ReportException(e);
					return false;
				}

				return true;
			}

			return false;
		}

		#endregion

		#region Private Methods

		private static bool Login()
		{
			return Login(LoginDialogMode.InitialLogin, null);
		}

		private static bool Login(LoginDialogMode mode, string userName)
		{
			bool needLoginDialog = true;
			string password = null;

			while (true)
			{
				if (needLoginDialog && !ShowLoginDialog(mode, ref userName, out password))
				{
					// user cancelled
					return false;
				}

				bool changPassword = false;

				try
				{
					Login(userName, password);
					//no exception = success.
					return true;
				}
				catch(FaultException<PasswordExpiredException>)
				{
					changPassword = true;
				}
				catch (PasswordExpiredException)
				{
					changPassword = true;
				}
				catch (Exception e)
				{
					//TODO: resolve issues with Enterprise server faults - they don't provide enough info.
					ReportException(e);
				}

				needLoginDialog = true;

				if (changPassword)
				{
					string newPassword;
					if (!ChangePassword(userName, password, out newPassword))
					{
						// user cancelled password change, so just abort everything
						return false;
					}

					// loop again, but this time using the new password, and don't show the login dialog
					// since we already have the credentials
					password = newPassword;
					needLoginDialog = false;
				}
			}
		}

		private static bool ShowLoginDialog(LoginDialogMode mode, ref string userName, out string password)
		{
			using (ILoginDialog loginDialog = (ILoginDialog)(new LoginDialogExtensionPoint()).CreateExtension())
			{
				loginDialog.Mode = mode;
				loginDialog.UserName = userName;

				Platform.Log(LogLevel.Debug, "Showing login dialog.");
				if (loginDialog.Show())
				{
					userName = loginDialog.UserName;
					password = loginDialog.Password;

					return true;
				}
			}

			userName = null;
			password = null;

			return false;
		}

		private static void Login(string userName, string password)
		{
			Platform.GetService<IAuthenticationService>(
				delegate(IAuthenticationService service)
				{
					InitiateSessionRequest request = new InitiateSessionRequest(userName, password);
					InitiateSessionResponse response = service.InitiateSession(request);

					if (response.SessionToken == null)
						throw new Exception("Invalid session token returned from authentication service.");

					Session.Create(userName, response.DisplayName, response.AuthorityTokens, response.SessionToken);
					Thread.CurrentPrincipal = Session.Current.Principal;
				});
		}

		private static void Logout()
		{
			if (Session.Current == null)
				return;

			Platform.GetService<IAuthenticationService>(
				delegate(IAuthenticationService service)
				{
					IIdentity identity = Session.Current.Principal.Identity;
					TerminateSessionRequest request = new TerminateSessionRequest(identity.Name, Session.Current.Token);
					service.TerminateSession(request);
				});
		}

		private static bool ChangePassword(string userName, string oldPassword, out string newPassword)
		{
			using (IChangePasswordDialog changePasswordDialog = (IChangePasswordDialog)(new ChangePasswordDialogExtensionPoint()).CreateExtension())
			{
				changePasswordDialog.UserName = userName;
				changePasswordDialog.Password = oldPassword;

				while (true)
				{
					if (changePasswordDialog.Show())
					{
						try
						{
							string password = null;
							Platform.GetService<IAuthenticationService>(
								delegate(IAuthenticationService service)
									{
										ChangePasswordRequest request =
											new ChangePasswordRequest(userName, changePasswordDialog.Password, changePasswordDialog.NewPassword);
										ChangePasswordResponse response = service.ChangePassword(request);
										password = changePasswordDialog.NewPassword;
									});

							newPassword = password;
							return true;
						}
						catch(Exception e)
						{
							//TODO: resolve issues with Enterprise server faults - they don't provide enough info.
							ReportException(e);
						}
					}
					else
					{
						// user cancelled
						newPassword = null;
						return false;
					}
				}
			}
		}

		private static void ReportException(Exception e)
		{
			if (e is RequestValidationException || e is FaultException<RequestValidationException>)
			{
				Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
			else if (e is EndpointNotFoundException)
			{
				Platform.Log(LogLevel.Error, e);
				Application.ShowMessageBox(SR.MessageNoEndpointListening, MessageBoxActions.Ok);
			}
			else if (e is TimeoutException)
			{
				Platform.Log(LogLevel.Error, e);
				Application.ShowMessageBox(SR.MessageLoginTimeout, MessageBoxActions.Ok);
			}
			else if (e is CommunicationException)
			{
				//TODO: because we don't get an appropriate exception, back assume this is just 'invalid user name or password' for now.
				Platform.Log(LogLevel.Error, e);
				//Application.ShowMessageBox(SR.MessageCommunicationError, MessageBoxActions.Ok);
				Application.ShowMessageBox(SR.MessageInvalidCredentials, MessageBoxActions.Ok);
			}
			else
			{
				Platform.Log(LogLevel.Error, e);
				Application.ShowMessageBox(SR.MessageUnknownErrorCommunicatingWithServer, MessageBoxActions.Ok);
			}
		}

		#endregion
		#endregion
	}
}
