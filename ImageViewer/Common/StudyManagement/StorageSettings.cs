#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.ImageViewer.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    [SettingsGroupDescription("Configuration for local study storage.")]
    [SettingsProvider(typeof (SystemConfigurationSettingsProvider))]
    internal sealed partial class StorageSettings
    {
        public sealed class Proxy
        {
            private readonly StorageSettings _settings;

            public Proxy(StorageSettings settings)
            {
                _settings = settings;
            }

            private object this[string propertyName]
            {
                get { return _settings[propertyName]; }
                set { _settings.SetSharedPropertyValue(propertyName, value); }
            }

            public double MinimumFreeSpacePercent
            {
                get { return _settings.MinimumFreeSpacePercent; }
                set { this["MinimumFreeSpacePercent"] = value; }
            }

            public string FileStoreDirectory
            {
                get { return _settings.FileStoreDirectory; }
                set { this["FileStoreDirectory"] = value; }
            }

            public void Save()
            {
                _settings.Save();
            }
        }

        public Proxy GetProxy()
        {
            return new Proxy(this);
        }
    }
}
