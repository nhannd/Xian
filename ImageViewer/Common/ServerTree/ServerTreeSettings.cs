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
using System.Text;
using System.Xml.Serialization;
using ClearCanvas.Common.Configuration;
using System.Xml;
using System.IO;

namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    //[Serializable]
    //public class StoredServerGroupRoot
    //{
    //    public StoredServerGroup Root { get; set; }
    //}

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
    
    internal static class StoredServerGroupSerializationHelper
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(StoredServerGroup), new[] { typeof(DirectoryServerReference), typeof(StoredServerGroup) });

        //internal static XmlDocument Serialize(StoredServerGroup storedServerGroup)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        _serializer.Serialize(stream, new StoredServerGroupRoot { Root = storedServerGroup });
        //        stream.Flush();
        //        stream.Position = 0;
        //        var doc = new XmlDocument();
        //        doc.Load(stream);
        //        return doc;
        //    }
        //}

        //internal static StoredServerGroup Deserialize(XmlDocument storedServerGroup)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        storedServerGroup.Save(stream);
        //        stream.Flush();
        //        stream.Position = 0;
        //        var root = _serializer.Deserialize(stream) as StoredServerGroupRoot;
        //        return root == null ? null : root.Root;
        //    }
        //}
    }

    //For now, this is a local shared setting, just like it used to be in a shared xml file.
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class ServerTreeSettings
    {
        internal void UpdateSharedServers(StoredServerGroup storedServerGroup)
        {
            //var serialized = StoredServerGroupSerializationHelper.Serialize(storedServerGroup);
            //ApplicationSettingsExtensions.SetSharedPropertyValue(this, "SharedServersXml", serialized);

            //var serialized = StoredServerGroupSerializationHelper.Serialize(storedServerGroup);
            //ApplicationSettingsExtensions.SetSharedPropertyValue(this, "SharedServers", storedServerGroup);

            ApplicationSettingsExtensions.SetSharedPropertyValue(this, "SharedServers", storedServerGroup);
        }

        internal StoredServerGroup GetSharedServers()
        {
            if (SharedServers == null)
                return null;

            //return StoredServerGroupSerializationHelper.Deserialize(SharedServers);
            return SharedServers;
        }
    }
}
