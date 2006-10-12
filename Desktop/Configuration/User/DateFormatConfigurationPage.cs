using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.Desktop.Configuration.User
{
	internal class DateFormatConfigurationPage : IConfigurationPage
	{
		private string _path; 
		private DateFormatApplicationComponent _component;

		public DateFormatConfigurationPage(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			_path = path;

			//initialize the component with the stored settings.
			_component = new DateFormatApplicationComponent(DateFormatSettings.Configuration);
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
			//Save the settings to the persistent store.
			DateFormatSettings.Configuration = _component.Configuration;
		}

		#endregion
	}
}
