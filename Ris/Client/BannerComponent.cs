#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public class BannerComponent : DHtmlComponent
	{
		private DataContractBase _healthcareContext;

		public BannerComponent()
		{
		}

		public BannerComponent(DataContractBase healthcareContext)
		{
			_healthcareContext = healthcareContext;
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.BannerPageUrl);
			base.Start();
		}

		public override bool ScrollBarsEnabled
		{
			get { return false; }
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _healthcareContext;
		}

		public DataContractBase HealthcareContext
		{
			get { return _healthcareContext; }
			set
			{
				_healthcareContext = value;

				if (this.IsStarted)
					Refresh();
			}
		}
	}
}
