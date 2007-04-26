using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    internal class DicomServerSettings : ShredConfigSection
    {
        private static DicomServerSettings _instance;

        private DicomServerSettings()
        {
        }

        public static string SettingName
        {
            get { return "DicomServerSettings"; }
        }

        public static DicomServerSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ShredConfigManager.GetConfigSection(DicomServerSettings.SettingName) as DicomServerSettings;
                    if (_instance == null)
                    {
                        _instance = new DicomServerSettings();
                        ShredConfigManager.UpdateConfigSection(DicomServerSettings.SettingName, _instance);
                    }
                }

                return _instance;
            }
        }

        public static void Save()
        {
            ShredConfigManager.UpdateConfigSection(DicomServerSettings.SettingName, _instance);
        }

        #region Public Properties

        [ConfigurationProperty("HostName", DefaultValue = "localhost")]
        public string HostName
        {
            get { return (string)this["HostName"]; }
            set { this["HostName"] = value; }
        }

        [ConfigurationProperty("AETitle", DefaultValue = "AETITLE")]
        public string AETitle
        {
            get { return (string)this["AETitle"]; }
            set { this["AETitle"] = value; }
        }

        [ConfigurationProperty("Port", DefaultValue = "104")]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("InterimStorageDirectory", DefaultValue = ".\\dicom_interim")]
        public string InterimStorageDirectory
        {
            get { return (string)this["InterimStorageDirectory"]; }
            set { this["InterimStorageDirectory"] = value; }
        }

        #endregion

        public override object Clone()
        {
            DicomServerSettings clone = new DicomServerSettings();

            clone.HostName = _instance.HostName;
            clone.AETitle = _instance.AETitle;
            clone.Port = _instance.Port;
            clone.InterimStorageDirectory = _instance.InterimStorageDirectory;

            return clone;
        }
    }
}
