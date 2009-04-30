#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    internal sealed class DiskspaceManagerSettings : ShredConfigSection
    {
		public const float LowWaterMarkDefault = 60F;
		public const float HighWaterMarkDefault = 80F;
		public const int CheckFrequencyDefault = 10;
    	public const int StudyLimitDefault = 500;
    	public const int MinStudyLimitDefault = 30;
		public const int MaxStudyLimitDefault = 10000;

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

        [ConfigurationProperty("EnforceStudyLimit", DefaultValue = false)]
		public bool EnforceStudyLimit
		{
            get { return (bool)this["EnforceStudyLimit"]; }
            set { this["EnforceStudyLimit"] = value; }
		}

		[ConfigurationProperty("MinStudyLimit", DefaultValue = MinStudyLimitDefault)]
		public int MinStudyLimit
		{
			get { return (int)this["MinStudyLimit"]; }
			set { this["MinStudyLimit"] = value; }
		}

		[ConfigurationProperty("MaxStudyLimit", DefaultValue = MaxStudyLimitDefault)]
		public int MaxStudyLimit
		{
			get { return (int)this["MaxStudyLimit"]; }
			set { this["MaxStudyLimit"] = value; }
		}

		[ConfigurationProperty("StudyLimit", DefaultValue = StudyLimitDefault)]
		public int StudyLimit
		{
			get { return (int)this["StudyLimit"]; }
			set { this["StudyLimit"] = Math.Min(Math.Max(value, MinStudyLimit), MaxStudyLimit); }
		}
		
		#endregion

        public override object Clone()
        {
            DiskspaceManagerSettings clone = new DiskspaceManagerSettings();

            clone.LowWatermark = _instance.LowWatermark;
            clone.HighWatermark = _instance.HighWatermark;
            clone.CheckFrequency = _instance.CheckFrequency;
        	clone.EnforceStudyLimit = _instance.EnforceStudyLimit;
			clone.MaxStudyLimit = _instance.MaxStudyLimit;
			clone.MinStudyLimit = _instance.MinStudyLimit;
			clone.StudyLimit = _instance.StudyLimit;

            return clone;
        }
    }
}
