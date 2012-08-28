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
	[ExtensionOf(typeof (FacilityProviderExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	internal sealed class FacilityProvider : IFacilityProvider
	{
		public IList<Facility> GetAvailableFacilities()
		{
			try
			{
				IList<Facility> choices = null;
				Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
				Platform.GetService<ILoginService>(service => { choices = RetrieveFacilityChoices(service); });
				Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
				return choices;
			}
			catch (Exception ex)
			{
				Desktop.Application.ShowMessageBox("Unable to connect to RIS server.  The workstation may be configured incorrectly, or the server may be unreachable.", MessageBoxActions.Ok);
				Platform.Log(LogLevel.Error, ex);
				return new Facility[0];
			}
		}

		private static IList<Facility> RetrieveFacilityChoices(ILoginService service)
		{
			var choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices;
			return choices != null ? choices.Select(fs => new Facility(fs.Code)).ToArray() : new Facility[0];
		}

		public Facility CurrentFacility
		{
			get { return LoginSession.Current != null ? new Facility(LoginSession.Current.WorkingFacility.Code) : null; }
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