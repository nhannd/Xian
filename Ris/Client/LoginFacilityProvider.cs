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
	internal sealed class LoginFacilityProvider : ILoginFacilityProvider
	{
		private readonly object _syncRoot = new object();
		private IList<FacilityInfo> _listFacilities;

		public IList<FacilityInfo> GetAvailableFacilities()
		{
			lock (_syncRoot)
			{
				try
				{
					Platform.Log(LogLevel.Debug, "Contacting server to obtain facility choices for login dialog...");
					Platform.GetService<ILoginService>(service => { _listFacilities = RetrieveFacilityChoices(service); });
					Platform.Log(LogLevel.Debug, "Got facility choices for login dialog.");
					return _listFacilities;
				}
				catch (Exception ex)
				{
					Desktop.Application.ShowMessageBox("Unable to connect to RIS server.  The workstation may be configured incorrectly, or the server may be unreachable.", MessageBoxActions.Ok);
					Platform.Log(LogLevel.Error, ex);
					return new FacilityInfo[0];
				}
			}
		}

		private FacilityInfo GetFacility(string code)
		{
			lock (_syncRoot)
			{
				return _listFacilities != null ? _listFacilities.FirstOrDefault(fs => fs.Code == code) : null;
			}
		}

		private static IList<FacilityInfo> RetrieveFacilityChoices(ILoginService service)
		{
			var choices = service.GetWorkingFacilityChoices(new GetWorkingFacilityChoicesRequest()).FacilityChoices;
			return choices != null ? choices.Select(fs => new FacilityInfo(fs.Code, fs.Name)).ToArray() : new FacilityInfo[0];
		}

		public FacilityInfo CurrentFacility
		{
			get
			{
				return LoginSession.Current == null  || LoginSession.Current.WorkingFacility == null ? null
					: GetFacility(LoginSession.Current.WorkingFacility.Code);
			}
			set
			{
				// once the session has been established, it can't be changed, but we
				// can just fail silently in order to support the case where the user
				// reconnects after a session expires
				if (LoginSession.Current != null)
					return;

				if (value != null && !string.IsNullOrEmpty(value.Code))
				{
					LoginSession.Create(value.Code);
				}
			}
		}
	}
}