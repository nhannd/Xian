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
    // TODO (CR Jun 2012): Leaving internal, at least for now, since the StorageConfiguration data contract
    // actually provides a useful abstraction and some important logic, and there's really no immediate need to change it.

    [SettingsGroupDescription("Configuration for local study deletion.")]
    [SettingsProvider(typeof (SystemConfigurationSettingsProvider))]
    internal sealed partial class StudyDeletionSettings
    {
        public sealed class Proxy
        {
            private readonly StudyDeletionSettings _settings;

            public Proxy(StudyDeletionSettings settings)
            {
                _settings = settings;
            }

            public bool Enabled
            {
                get { return _settings.Enabled; }
                set { this["Enabled"] = value; }
            }

            public TimeUnit TimeUnit
            {
                get { return _settings.TimeUnit; }
                set { this["TimeUnit"] = value; }
            }

            public int TimeValue
            {
                get { return _settings.TimeValue; }
                set { this["TimeValue"] = value; }
            }

            private object this[string propertyName]
            {
                get { return _settings[propertyName]; }
                set { _settings.SetSharedPropertyValue(propertyName, value); }
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
