using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ClearCanvas.ImageServer.Shreds
{
    [SettingsGroupDescription("Shred related settings for the ImageServer")]
	//[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ImageServerShredSettings
    {
        private ImageServerShredSettings()
		{
			//ApplicationSettingsRegister.Instance.RegisterInstance(this);
		}

        ~ImageServerShredSettings()
		{
			//ApplicationSettingsRegister.Instance.UnregisterInstance(this);
		}
    }
}
