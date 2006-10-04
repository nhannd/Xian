using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Configuration
{
	public interface IConfigurationPageProvider
	{
		IConfigurationPage[] GetPages();
	}
}
