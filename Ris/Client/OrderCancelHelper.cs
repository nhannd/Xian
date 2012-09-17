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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public static class OrderCancelHelper
	{
		public static bool CancelOrder(EntityRef orderRef, string description, IDesktopWindow desktopWindow)
		{
			// first check for warnings
			var warnings = new List<string>();
			Platform.GetService<IOrderEntryService>(
				service => warnings = service.QueryCancelOrderWarnings(new QueryCancelOrderWarningsRequest(orderRef)).Warnings);

			if (warnings.Count > 0)
			{
				var warn = CollectionUtils.FirstElement(warnings);
				var action = desktopWindow.ShowMessageBox(
					warn + "\n\nAre you sure you want to cancel this order?",
					MessageBoxActions.YesNo);
				if (action == DialogBoxAction.No)
					return false;
			}

			var cancelOrderComponent = new CancelOrderComponent();
			var exitCode = ApplicationComponent.LaunchAsDialog(
				desktopWindow,
				cancelOrderComponent,
				String.Format(SR.TitleCancelOrder, description));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				Platform.GetService<IOrderEntryService>(
					service => service.CancelOrder(new CancelOrderRequest(orderRef, cancelOrderComponent.SelectedCancelReason)));

				return true;
			}
			return false;
		}
	}
}
