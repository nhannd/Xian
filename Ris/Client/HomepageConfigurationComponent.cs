#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (!Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View) ||
				!LoginSession.Current.IsStaff)
				return new IConfigurationPage[] {}; 
			
			return new IConfigurationPage[] { new HomepageConfigurationPage() };
		}

		#endregion
	}

	public class HomepageConfigurationPage : IConfigurationPage
	{
		private HomepageConfigurationComponent _component;

		#region IConfigurationPage Members

		public string GetPath()
		{
			return "Homepage";
		}

		public IApplicationComponent GetComponent()
		{
			if(_component == null)
			{
				_component = new HomepageConfigurationComponent();
			}
			return _component;
		}

		public void SaveConfiguration()
		{
			HomePageSettings.Default.Save();
		}

		#endregion
		
	}


	/// <summary>
	/// Extension point for views onto <see cref="HomepageConfigurationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class HomepageConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// HomepageConfigurationComponent class.
	/// </summary>
	[AssociateView(typeof(HomepageConfigurationComponentViewExtensionPoint))]
	public class HomepageConfigurationComponent : ApplicationComponent
	{


		/// <summary>
		/// Constructor.
		/// </summary>
		public HomepageConfigurationComponent()
		{
		}


		#region Presentation Model

		public bool ShowHomepageOnStartUp
		{
			get { return HomePageSettings.Default.ShowHomepageOnStartUp; }
			set
			{
				HomePageSettings.Default.ShowHomepageOnStartUp = value;
				if (!HomePageSettings.Default.ShowHomepageOnStartUp)
				{
					// it does not make sense to prevent the homepage from closing,
					// unless it is shown on startup
					HomePageSettings.Default.PreventHomepageFromClosing = false;

					NotifyPropertyChanged("PreventHomepageFromClosing");
				}
				NotifyPropertyChanged("PreventHomepageFromClosingEnabled");
				this.Modified = true;
			}
		}

		public bool PreventHomepageFromClosing
		{
			get { return HomePageSettings.Default.PreventHomepageFromClosing; }
			set
			{
				HomePageSettings.Default.PreventHomepageFromClosing = value;
				this.Modified = true;
			}
		}

		public bool PreventHomepageFromClosingEnabled
		{
			get
			{
				// it does not make sense to even have the option of preventing the homepage from closing,
				// unless it is shown on startup
				return HomePageSettings.Default.ShowHomepageOnStartUp;
			}	
		}

		#endregion

	}
}
