using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.Desktop.Configuration.Application
{
	class DateFormatApplicationConfigurationPage : IConfigurationPage
	{
		private DateFormatApplicationComponent _dateFormatApplicationComponent;
		private string _path;

		public DateFormatApplicationConfigurationPage(string path)
		{ 
			_path = path;
			_dateFormatApplicationComponent = new DateFormatApplicationComponent();
		}

		#region IConfigurationPage Members

		public string GetPath()
		{
			return _path;
		}

		public IApplicationComponent GetComponent()
		{
			return _dateFormatApplicationComponent;
		}

		#endregion
	}
}
