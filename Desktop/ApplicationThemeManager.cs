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
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	public static class ApplicationThemeManager
	{
		private static readonly Dictionary<string, ApplicationTheme> _themes;

		static ApplicationThemeManager()
		{
			var themeResourceProviders = new Dictionary<string, List<IApplicationThemeResourceProvider>>();
			foreach (IApplicationThemeResourceProvider theme in new ApplicationThemeResourceProviderExtensionPoint().CreateExtensions())
			{
				if (!string.IsNullOrEmpty(theme.Id))
				{
					if (!themeResourceProviders.ContainsKey(theme.Id))
						themeResourceProviders.Add(theme.Id, new List<IApplicationThemeResourceProvider>());
					themeResourceProviders[theme.Id].Add(theme);
				}
			}
			_themes = CollectionUtils.Map(themeResourceProviders, kvp => new KeyValuePair<string, ApplicationTheme>(kvp.Key, new ApplicationTheme(kvp.Value)));
		}

		public static ApplicationTheme CurrentTheme
		{
			get { return Application.CurrentUITheme; }
			set { Application.CurrentUITheme = value; }
		}

		public static ICollection<ApplicationTheme> Themes
		{
			get { return _themes.Values; }
		}

		public static bool IsThemeDefined(string id)
		{
			return !string.IsNullOrEmpty(id) && _themes.ContainsKey(id);
		}

		public static ApplicationTheme GetTheme(string id)
		{
			ApplicationTheme theme;
			return !string.IsNullOrEmpty(id) && _themes.TryGetValue(id, out theme) ? theme : null;
		}

		public static bool HasResource(string resourceFullName)
		{
			return HasResource(resourceFullName, null);
		}

		public static bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var currentTheme = CurrentTheme;
				return currentTheme != null && currentTheme.HasResource(resourceFullName, originalAssemblyHint);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "An exception was thrown while processing an application theme.");
				return false;
			}
		}

		public static Stream OpenResource(string resourceFullName)
		{
			return OpenResource(resourceFullName, null);
		}

		public static Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var currentTheme = CurrentTheme;
				return currentTheme != null ? currentTheme.OpenResource(resourceFullName, originalAssemblyHint) : null;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "An exception was thrown while processing an application theme.");
				return null;
			}
		}
	}
}