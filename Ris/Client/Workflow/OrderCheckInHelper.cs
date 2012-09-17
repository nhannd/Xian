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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public static class OrderCheckInHelper
	{
		public static bool CheckIn(EntityRef orderRef, string description, IDesktopWindow desktopWindow)
		{
			List<ProcedureSummary> procedures = null;
			Platform.GetService((IRegistrationWorkflowService service) =>
				procedures = service.ListProceduresForCheckIn(new ListProceduresForCheckInRequest(orderRef)).Procedures);

			if(procedures.Count == 0)
			{
				desktopWindow.ShowMessageBox(SR.MessageNoProceduresCanBeCheckedIn, MessageBoxActions.Ok);
				return false;
			}

			var checkInComponent = new CheckInOrderComponent(procedures);
			var exitCode = ApplicationComponent.LaunchAsDialog(
				desktopWindow,
				checkInComponent,
				String.Format("Checking in {0}", description));

			return (exitCode == ApplicationComponentExitCode.Accepted);
		}
	}
}
