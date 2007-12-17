#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

        [ConfigurationProperty("AETitle", DefaultValue = "CLEARCANVAS")]
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
