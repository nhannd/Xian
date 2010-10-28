#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// A default implementation of <see cref="IConfigurationPage"/>.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="IConfigurationApplicationComponent"/>-derived 
	/// class that will be hosted in this page.  The class must have a parameterless default constructor.</typeparam>
	public class ConfigurationPage<T> : ConfigurationPage
		 where T : IConfigurationApplicationComponent, new()
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to the <see cref="ConfigurationPage{T}"/>.</param>
		public ConfigurationPage(string path)
			: base(path, new T())
		{
		}
	}
	
	public class ConfigurationPage : IConfigurationPage
	{
		private readonly IConfigurationApplicationComponent _component;
		private readonly string _path;

		public ConfigurationPage(string path, IConfigurationApplicationComponent component)
		{
			_path = path;
			_component = component;
		}

		#region IConfigurationPage Members

		/// <summary>
		/// Gets the path to this page.
		/// </summary>
		public string GetPath()
		{
			return _path;
		}

		/// <summary>
		/// Gets the <see cref="IApplicationComponent"/> that is hosted in this page.
		/// </summary>
		public IApplicationComponent GetComponent()
		{
			return _component;
		}

		/// <summary>
		/// Saves any configuration changes that were made.
		/// </summary>
		public void SaveConfiguration()
		{
			_component.Save();
		}

		#endregion
	}
}
