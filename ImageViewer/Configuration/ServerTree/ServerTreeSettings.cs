#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    [Serializable]
    public class StoredServerGroup
    {
        public StoredServerGroup()
        {
            ChildGroups = new List<StoredServerGroup>();
            DirectoryServerReferences = new List<DirectoryServerReference>();
        }

        public StoredServerGroup(string name)
        {
            Name = name;
            ChildGroups = new List<StoredServerGroup>();
            DirectoryServerReferences = new List<DirectoryServerReference>();
        }

        public string Name { get; set; }

        public List<StoredServerGroup> ChildGroups { get; set; }
        public List<DirectoryServerReference> DirectoryServerReferences { get; set; }
    }

    [Serializable]
    public class DirectoryServerReference
    {
        public DirectoryServerReference()
        {
        }

        public DirectoryServerReference(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
    
    //For now, this is a local shared setting, just like it used to be in a shared xml file.
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class ServerTreeSettings : IMigrateSettings
    {
        internal void UpdateSharedServers(StoredServerGroup storedServerGroup)
        {
            ApplicationSettingsExtensions.SetSharedPropertyValue(this, "SharedServers", storedServerGroup);
        }

        internal StoredServerGroup GetSharedServers()
        {
            if (SharedServers == null)
                return null;

            return SharedServers;
        }

        #region IMigrateSettings Members

        public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
        {
            if (migrationValues.PropertyName != "SharedServers")
                return;

            if (migrationValues.PreviousValue != null)
            {
                migrationValues.CurrentValue = migrationValues.PreviousValue;
                return;
            }
            
            try
            {
                //TODO (Marmot): Need to test this!
                var old = SerializationHelper.LoadFromXml();
                var serverTree = new ServerTree(old);
                migrationValues.CurrentValue = serverTree.RootServerGroup.ToStoredServerGroup();
                serverTree.SaveServersToDirectory();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to migrate server tree from legacy xml file.");
            }
        }

        #endregion
    }
}
