using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Configuration
{
	public interface IConfigurationPageProvider
	{
		IEnumerable<IConfigurationPage> GetPages();
	}
}
