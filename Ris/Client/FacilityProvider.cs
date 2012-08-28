#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof (LoginFacilityProviderExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	internal sealed class FacilityProvider : ILoginFacilityProvider
	{
		public IList<FacilityInfo> GetAvailableFacilities()
		{
			try
			{
				IList<FacilityInfo> choices = null;
				Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
				Platform.GetService<ILoginService>(service => { choices = RetrieveFacilityChoices(service); });
				Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
				return choices;
			}
			catch (Exception ex)
			{
				Desktop.Application.ShowMessageBox("Unable to connect to RIS server.  The workstation may be configured incorrectly, or the server may be unreachable.", MessageBoxActions.Ok);
				Platform.Log(LogLevel.Error, ex);
				return new FacilityInfo[0];
			}
		}

		private static IList<FacilityInfo> RetrieveFacilityChoices(ILoginService service)
		{
			var choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices;
			return choices != null ? choices.Select(fs => new FacilityInfo(fs.Code)).ToArray() : new FacilityInfo[0];
		}

		public FacilityInfo CurrentFacility
		{
			get { return LoginSession.Current != null ? new FacilityInfo(LoginSession.Current.WorkingFacility.Code) : null; }
			set
			{
				if (LoginSession.Current == null) throw new InvalidOperationException("Current working facility cannot be changed while a user is logged in.");
				if (value != null && !string.IsNullOrEmpty(value.Code))
				{
					LoginSession.Create(value.Code);
				}
			}
		}
	}
}