using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ClearCanvas.ImageServer.Queue.Work
{
    [SettingsGroupDescription("Work Queue related settings for the ImageServer")]
    //[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ImageServerQueueWorkSettings
    {
        private ImageServerQueueWorkSettings()
        {
            //ApplicationSettingsRegistry.Instance.RegisterInstance(this);
        }
    }
}
