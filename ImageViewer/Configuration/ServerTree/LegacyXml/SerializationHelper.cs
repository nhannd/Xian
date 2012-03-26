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
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml
{
    internal static class SerializationHelper
    {
        private const string _myServersXmlFile = "DicomAEServers.xml";

        private static readonly System.Xml.Serialization.XmlSerializer _serializer
            = new System.Xml.Serialization.XmlSerializer(typeof(ServerTreeRoot),
                                new Type[]
                                    {
                                        typeof (ServerTreeGroup),
                                        typeof (Server),
                                        typeof (List<ServerTreeGroup>),
                                        typeof (List<Server>)
                                    });

        internal static ServerTreeRoot LoadFromXml()
        {
            string file = GetServersXmlFileName();
            if (File.Exists(file))
            {
                using (Stream fStream = File.OpenRead(file))
                    return (ServerTreeRoot)_serializer.Deserialize(fStream);
            }

            return null;
        }

        internal static void SaveToXml(ServerTreeRoot root)
        {
			using (Stream fStream = new FileStream(GetServersXmlFileName(), FileMode.Create, FileAccess.Write, FileShare.Read))
			{
			    _serializer.Serialize(fStream, root);
                fStream.Close();
            }
        }

        private static string GetServersXmlFileName()
		{
			return Path.Combine(Platform.InstallDirectory, _myServersXmlFile);
		}
    }
}
