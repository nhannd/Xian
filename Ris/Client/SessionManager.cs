#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Client
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
	class SessionManager : ISessionManager
	{
		#region ISessionManager Members

		bool ISessionManager.InitiateSession()
		{
			try
			{
				return Login();
			}
			catch(Exception e)
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
				var currentSession = LoginSession.Current;
				if (currentSession != null)
				{
					currentSession.Terminate();
				}
			}
			catch (Exception e)
			{
				// since we're logging out, just log the exception and move on
				Platform.Log(LogLevel.Error, e);
			}
		}

		public SessionStatus SessionStatus
		{
			get
			{
				// the RIS can only be used online, so the session status will always be online
				return SessionStatus.Online;
			}
		}

		public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

		#endregion

		#region Internal methods

		internal static bool RenewLogin()
		{
			var current = LoginSession.Current;
			if (current == null)
				return false;

			return Login(LoginDialogMode.RenewLogin, current.UserName, current.WorkingFacility.Code);
		}

		internal static bool ChangePassword()
		{
			var current = LoginSession.Current;
			if(current == null)
				return false;

			string newPassword;
			return ChangePassword(current.UserName, null, out newPassword);
		}

		#endregion

		private static bool Login()
		{
			return Login(LoginDialogMode.InitialLogin, null, null);
		}

		private static bool Login(LoginDialogMode mode, string userName, string facility)
		{
			var needLoginDialog = true;
			string password = null;

			var risServerDown = false;
			var facilities = new List<FacilitySummary>();
			try
			{
				Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
				facilities = GetFacilityChoices();
				Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
			}
			catch (Exception e)
			{
				risServerDown = true;
				Platform.Log(LogLevel.Debug, "Ris is down.  Failed to get facility choices for login dialog.");
				Platform.Log(LogLevel.Error, e);

				if (DialogBoxAction.No == Desktop.Application.ShowMessageBox(SR.MessageRisServerDowntime, MessageBoxActions.YesNo))
					return false;
			}

			while (true)
			{
				if (needLoginDialog)
				{
					if (!ShowLoginDialog(mode, facilities, ref userName, ref facility, out password))
					{
						// user cancelled
						return false;
					}
				}

				try
				{
					var selectedFacility = CollectionUtils.SelectFirst(facilities, fs => fs.Code == facility);

					// try to create the session
					LoginSession.Create(userName, password, selectedFacility, risServerDown);

					// successfully logged in
					return true;
				}
				catch (PasswordExpiredException)
				{
					string newPassword;
					if(!ChangePassword(userName, password, out newPassword))
					{
						// user cancelled password change, so just abort everything
						return false;
					}

					// loop again, but this time using the new password, and don't show the login dialog
					// since we already have the credentials
					password = newPassword;
					needLoginDialog = false;
				}
				catch (Exception e)
				{
					ReportException(e);
				}
			}
		}

		private static bool ShowLoginDialog(LoginDialogMode mode, List<FacilitySummary> facilities, ref string userName, ref string facility, out string password)
		{
			using (var loginDialog = (ILoginDialog)(new LoginDialogExtensionPoint()).CreateExtension())
			{
				var facilityCodes = CollectionUtils.Map(facilities, (FacilitySummary fs) => fs.Code).ToArray();

				var initialFacilityCode = LoginDialogSettings.Default.SelectedFacility;
				var location = LoginDialogSettings.Default.DialogScreenLocation;
				

				// if no saved facility, just choose the first one
				if (string.IsNullOrEmpty(initialFacilityCode) && facilityCodes.Length > 0)
					initialFacilityCode = facilityCodes[0];

				loginDialog.Mode = mode;
				loginDialog.FacilityChoices = facilityCodes;
				loginDialog.Facility = initialFacilityCode;
				loginDialog.UserName = userName;
				if (location != Point.Empty)
					loginDialog.Location = location;

				Platform.Log(LogLevel.Debug, "Showing login dialog.");
				if (loginDialog.Show())
				{
					// save selected facility
					LoginDialogSettings.Default.SelectedFacility = loginDialog.Facility;
					LoginDialogSettings.Default.DialogScreenLocation = loginDialog.Location;
					LoginDialogSettings.Default.Save();

					userName = loginDialog.UserName;
					password = loginDialog.Password;
					facility = loginDialog.Facility;

					return true;
				}
			}
			userName = null;
			password = null;
			facility = null;

			return false;
		}

		private static bool ChangePassword(string userName, string oldPassword, out string newPassword)
		{
			using (var changePasswordDialog = (IChangePasswordDialog)(new ChangePasswordDialogExtensionPoint()).CreateExtension())
			{
				changePasswordDialog.UserName = userName;
				changePasswordDialog.Password = oldPassword;
				while (true)
				{
					if (changePasswordDialog.Show())
					{
						try
						{
							LoginSession.ChangePassword(userName, changePasswordDialog.Password,
												changePasswordDialog.NewPassword);

							newPassword = changePasswordDialog.NewPassword;
							return true;
						}
						catch (Exception e)
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
			if(e is RequestValidationException)
			{
				Desktop.Application.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
			else if (e is UserAccessDeniedException)
			{
				Desktop.Application.ShowMessageBox(SR.MessageLoginAccessDenied, MessageBoxActions.Ok);
			}
			else if (e is CommunicationException)
			{
				Platform.Log(LogLevel.Error, e);
				Desktop.Application.ShowMessageBox(SR.MessageCommunicationError, MessageBoxActions.Ok);
			}
			else if (e is TimeoutException)
			{
				Platform.Log(LogLevel.Error, e);
				Desktop.Application.ShowMessageBox(SR.MessageLoginTimeout, MessageBoxActions.Ok);
			}
			else
			{
				Platform.Log(LogLevel.Error, e);
				Desktop.Application.ShowMessageBox(SR.MessageUnknownErrorCommunicatingWithServer, MessageBoxActions.Ok);
			}
		}

		private static List<FacilitySummary> GetFacilityChoices()
		{
			List<FacilitySummary> choices = null;
			Platform.GetService<ILoginService>(
				service => choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices);
			return choices;
		}
	}
}
