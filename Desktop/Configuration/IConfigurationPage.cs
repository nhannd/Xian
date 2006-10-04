using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Configuration
{
	public interface IConfigurationPage
	{
		string GetPath();
		IApplicationComponent GetComponent();
	}
}
