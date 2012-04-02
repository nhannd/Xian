#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	internal sealed class OemConfiguration : IXmlSerializable
	{
		public static OemConfiguration Load()
		{
			var oemPath = System.IO.Path.Combine(Platform.InstallDirectory, @"oem\config.xml");
			return new OemConfiguration(File.Exists(oemPath) ? oemPath : null);
		}

		private OemConfiguration(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				try
				{
					using (var stream = File.OpenRead(file))
					using (var reader = XmlReader.Create(stream))
					{
						ReadXml(reader);
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Failed to load OEM product configuration.");
				}
			}
		}

		public string ProductName { get; private set; }

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var empty = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!empty)
			{
				while (reader.MoveToContent() == XmlNodeType.Element)
				{
					if (reader.Name == @"ProductName" && !reader.IsEmptyElement)
					{
						ProductName = reader.ReadElementString();
					}
					else
					{
						// consume the bad element and skip to next sibling or the parent end element tag
						reader.ReadOuterXml();
					}
				}
				reader.MoveToContent();
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString(@"ProductName", ProductName);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}
	}
}