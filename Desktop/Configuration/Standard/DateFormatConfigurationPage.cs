using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	public sealed class DateFormatConfigurationPage : IConfigurationPage
	{
		private string _path; 
		private DateFormatApplicationComponent _component;

		public DateFormatConfigurationPage(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			_path = path;

			_component = new DateFormatApplicationComponent();
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
