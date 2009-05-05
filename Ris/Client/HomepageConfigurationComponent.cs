#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (!Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Homepage))
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
