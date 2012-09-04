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
using ClearCanvas.Common.Configuration;

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
    internal sealed partial class ServerTreeSettings
    {
        internal void UpdateSharedServers(StoredServerGroup storedServerGroup)
        {
            this.SetSharedPropertyValue("SharedServers", storedServerGroup);
        }

        internal StoredServerGroup GetSharedServers()
        {
            if (SharedServers == null)
                return null;

            return SharedServers;
        }
    }
}
