#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	public sealed class ApplicationTheme : IApplicationThemeResourceProvider
	{
		private readonly IList<IApplicationThemeResourceProvider> _providers;

		public ApplicationTheme(IApplicationThemeResourceProvider provider)
		{
			Platform.CheckForNullReference(provider, "provider");
			Platform.CheckForEmptyString(provider.Id, "provider.Id");
			_providers = new List<IApplicationThemeResourceProvider> {provider};
		}

		public ApplicationTheme(IEnumerable<IApplicationThemeResourceProvider> providers)
		{
			_providers = new List<IApplicationThemeResourceProvider>(providers);

			Platform.CheckTrue(_providers.Count > 0, @"At least one theme resource provider must be specified");
		}

		public string Id
		{
			get { return _providers[0].Id; }
		}

		public string Name
		{
			get { return _providers[0].Name; }
		}

		public string Description
		{
			get { return _providers[0].Description; }
		}

		public string Icon
		{
			get { return _providers[0].Icon; }
		}

		public IApplicationThemeColors Colors
		{
			get { return _providers[0].Colors; }
		}

		public Stream CreateIcon()
		{
			var resourceResolver = new ResourceResolver(_providers[0].GetType(), false);
			return resourceResolver.OpenResource(_providers[0].Icon);
		}

		public bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			foreach (var provider in _providers)
				if (provider.HasResource(resourceFullName, originalAssemblyHint)) return true;
			return false;
		}

		public Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			Stream stream;
			foreach (var provider in _providers)
				if ((stream = provider.OpenResource(resourceFullName, originalAssemblyHint)) != null) return stream;
			return null;
		}

		#region Static Helpers

		public static ApplicationTheme CurrentTheme
		{
			get { return Application.CurrentUITheme; }
			set { Application.CurrentUITheme = value; }
		}

		public static ICollection<ApplicationTheme> Themes
		{
			get { return ApplicationThemeManager.Themes; }
		}

		public static bool IsThemeDefined(string id)
		{
			return ApplicationThemeManager.IsThemeDefined(id);
		}

		public static ApplicationTheme GetTheme(string id)
		{
			return ApplicationThemeManager.GetTheme(id);
		}

		#endregion
	}
}