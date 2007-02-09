using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
	public sealed class ConfigurationPage<T> : IConfigurationPage
		 where T : ConfigurationApplicationComponent, new()
	{
		private T _component;
		private string _path;

		public ConfigurationPage(string path)
		{
			Platform.CheckForEmptyString(path, "path");
			_path = path;
			_component = new T();
		}

		private ConfigurationPage()
		{
		}

		#region IConfigurationPage Members

		public string GetPath()
		{
			return _path;
		}

		public IApplicationComponent GetComponent()
		{
			return _component;
		}

		public void SaveConfiguration()
		{
			_component.Save();
		}

		#endregion
	}
}
