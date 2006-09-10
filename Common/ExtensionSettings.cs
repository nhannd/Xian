#if !MONO

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Currently empty, but in the future, methods may be added that, for example
	/// switches SettingsProviders.
	/// </summary>
	public class ExtensionSettings : ApplicationSettingsBase
	{
		public ExtensionSettings()
		{
		}
	}
}

#endif //MONO