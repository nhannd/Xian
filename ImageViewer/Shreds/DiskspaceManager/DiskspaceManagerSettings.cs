using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    internal class DiskspaceManagerSettings : ShredConfigSection
    {
		public const float LowWaterMarkDefault = 60F;
		public const float HighWaterMarkDefault = 80F;
		public const int CheckFrequencyDefault = 10;

        private static DiskspaceManagerSettings _instance;

        private DiskspaceManagerSettings()
        {
        }

        public static string SettingName
        {
            get { return "DiskspaceManagerSettings"; }
        }

        public static DiskspaceManagerSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ShredConfigManager.GetConfigSection(DiskspaceManagerSettings.SettingName) as DiskspaceManagerSettings;
                    if (_instance == null)
                    {
                        _instance = new DiskspaceManagerSettings();
                        ShredConfigManager.UpdateConfigSection(DiskspaceManagerSettings.SettingName, _instance);
                    }
                }

                return _instance;
            }
        }

        public static void Save()
        {
            ShredConfigManager.UpdateConfigSection(DiskspaceManagerSettings.SettingName, _instance);
        }

        #region Public Properties

		[ConfigurationProperty("LowWatermark", DefaultValue = LowWaterMarkDefault)]
        public float LowWatermark
        {
            get { return (float)this["LowWatermark"]; }
            set { this["LowWatermark"] = value; }
        }

        [ConfigurationProperty("HighWatermark", DefaultValue = HighWaterMarkDefault)]
        public float HighWatermark
        {
            get { return (float)this["HighWatermark"]; }
            set { this["HighWatermark"] = value; }
        }

        [ConfigurationProperty("CheckFrequency", DefaultValue = CheckFrequencyDefault)]
        public int CheckFrequency
        {
            get { return (int)this["CheckFrequency"]; }
            set { this["CheckFrequency"] = value; }
        }

        #endregion

        public override object Clone()
        {
            DiskspaceManagerSettings clone = new DiskspaceManagerSettings();

            clone.LowWatermark = _instance.LowWatermark;
            clone.HighWatermark = _instance.HighWatermark;
            clone.CheckFrequency = _instance.CheckFrequency;

            return clone;
        }
    }
}
