#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	/// <summary>
	/// Provides common <see cref="IConfigurationPage"/>s for settings defined in the framework.
	/// </summary>
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	internal class StandardOptionsConfigurationPageProvider : IConfigurationPageProvider
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public StandardOptionsConfigurationPageProvider()
		{ 
		}

		#region IConfigurationPageProvider Members

		/// <summary>
		/// Gets all the <see cref="IConfigurationPage"/>s for this provider.
		/// </summary>
		public IEnumerable<IConfigurationPage> GetPages()
		{
			var listPages = new List<IConfigurationPage>();

			if(CheckPermission(AuthorityTokens.Desktop.CustomizeDateTimeFormat))
			{
				listPages.Add(new ConfigurationPage<DateFormatApplicationComponent>("TitleDateFormat"));
			}

			listPages.Add(new ConfigurationPage<ToolbarConfigurationComponent>("TitleToolbar"));
		
			return listPages.AsReadOnly();
		}

		#endregion

		private static bool CheckPermission(string authorityToken)
		{
			// if the thread is running in a non-authenticated mode, then we have no choice but to allow.
			// this seems a little counter-intuitive, but basically we're counting on the fact that if
			// the desktop is running in an enterprise environment, then the thread *will* be authenticated,
			// and that this is enforced by some mechanism outside the scope of this class.  The only
			// scenario in which the thread would ever be unauthenticated is the stand-alone scenario.
			return Thread.CurrentPrincipal == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false
			       || Thread.CurrentPrincipal.IsInRole(authorityToken);
		}
	}
}
