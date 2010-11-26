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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.ImageViewer.Services.Auditing;
using System.Net;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop
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

				bool changePassword = false;

				// attempt to re-enable auditing, in case it was turned off in an earlier session
				AuditHelper.Enabled = true;

				try
				{
					Login(userName, password);
					return true;
				}
				catch (FaultException<PasswordExpiredException>)
				{
					changePassword = true;
				}
				catch (PasswordExpiredException)
				{
					changePassword = true;
				}
				catch (Exception e)
				{
					ReportException(e);
				}

				needLoginDialog = true;

				if (changePassword)
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
					try
					{
						InitiateSessionRequest request = new InitiateSessionRequest(userName, Application.Name, Dns.GetHostName(), password);
						request.GetAuthorizations = true;
						InitiateSessionResponse response = service.InitiateSession(request);

						if (response.SessionToken == null)
							throw new Exception("Invalid session token returned from authentication service.");

						Session.Create(userName, response.DisplayName, response.AuthorityTokens, response.SessionToken);
						Thread.CurrentPrincipal = Session.Current.Principal;

						AuditHelper.LogLogin(userName, EventResult.Success);
					}
					catch(FaultException<PasswordExpiredException>)
					{
						//log it after the user has changed their password.
						throw;
					}
					catch(PasswordExpiredException)
					{
						//log it after the user has changed their password.
						throw;
					}
					catch
					{
						AuditHelper.LogLogin(userName, EventResult.SeriousFailure);
						throw;
					}
				});
		}

		private static void Logout()
		{
			if (Session.Current == null)
				return;

			string userName = Session.Current.Principal.Identity.Name;

			try
			{
				Platform.GetService<IAuthenticationService>(
					delegate(IAuthenticationService service)
						{
							IIdentity identity = Session.Current.Principal.Identity;
							TerminateSessionRequest request = new TerminateSessionRequest(identity.Name, Session.Current.Token);
							service.TerminateSession(request);
						});

				AuditHelper.LogLogout(userName, EventResult.Success);
			}
			catch (Exception)
			{
				AuditHelper.LogLogout(userName, EventResult.SeriousFailure);
				throw;
			}
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
			if (e is FaultException<RequestValidationException> || e is RequestValidationException)
			{
				Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
			else if (e is FaultException<UserAccessDeniedException> || e is UserAccessDeniedException)
			{
				Platform.Log(LogLevel.Error, e);
				Application.ShowMessageBox(SR.MessageAccessDenied, MessageBoxActions.Ok);
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
			else if (e is UnknownServiceException)
			{
				Application.ShowMessageBox(SR.MessageNoServiceProvider, MessageBoxActions.Ok);
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
