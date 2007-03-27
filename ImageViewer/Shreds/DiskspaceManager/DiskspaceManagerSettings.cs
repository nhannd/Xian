using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    public class DiskspaceManagerSettings : ShredConfigSection
    {
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

        [ConfigurationProperty("DriveName", DefaultValue = "C")]
        public string DriveName
        {
            get { return (string)this["DriveName"]; }
            set { this["DriveName"] = value; }
        }

        [ConfigurationProperty("Status", DefaultValue = "")]
        public string Status
        {
            get { return (string)this["Status"]; }
            set { this["Status"] = value; }
        }

        [ConfigurationProperty("LowWatermark", DefaultValue = "60.0")]
        public float LowWatermark
        {
            get { return (float)this["LowWatermark"]; }
            set { this["LowWatermark"] = value; }
        }

        [ConfigurationProperty("HighWatermark", DefaultValue = "80.0")]
        public float HighWatermark
        {
            get { return (float)this["HighWatermark"]; }
            set { this["HighWatermark"] = value; }
        }

        [ConfigurationProperty("UsedSpace", DefaultValue = "70.0")]
        public float UsedSpace
        {
            get { return (float)this["UsedSpace"]; }
            set { this["UsedSpace"] = value; }
        }

        [ConfigurationProperty("CheckFrequency", DefaultValue = "10")]
        public int CheckFrequency
        {
            get { return (int)this["CheckFrequency"]; }
            set { this["CheckFrequency"] = value; }
        }

        #endregion

        public override object Clone()
        {
            DiskspaceManagerSettings clone = new DiskspaceManagerSettings();

            clone.DriveName = _instance.DriveName;
            clone.Status = _instance.Status;
            clone.LowWatermark = _instance.LowWatermark;
            clone.HighWatermark = _instance.HighWatermark;
            clone.UsedSpace = _instance.UsedSpace;
            clone.CheckFrequency = _instance.CheckFrequency;

            return clone;
        }
    }
}
