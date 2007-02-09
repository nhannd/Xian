using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Configuration
{
	public abstract class ConfigurationApplicationComponent : ApplicationComponent
	{
		public ConfigurationApplicationComponent()
		{ 
		}

		public abstract void Save();
	}
}
