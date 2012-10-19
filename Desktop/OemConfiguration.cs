#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Globalization;
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

		public Color GlyphColor { get; private set; }

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
					else if (reader.Name == @"GlyphColor" && !reader.IsEmptyElement)
					{
						GlyphColor = ParseColor(reader.ReadElementString());
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
			writer.WriteElementString(@"GlyphColor", FormatColor(GlyphColor));
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		private static string FormatColor(Color color)
		{
			if (color.IsNamedColor) return color.Name;

			var colorValue = (uint) color.ToArgb();
			return color.A == 255 ? string.Format("#{0:X6}", colorValue & 0x00FFFFFF) : string.Format("#{0:X8}", colorValue);
		}

		private static Color ParseColor(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value[0] == '#') value = value.Substring(1); // drop the # prefix used in HTML/CSS

				int argb;
				if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out argb))
				{
					var color = Color.FromArgb(argb);
					if (value.Length < 7) color = Color.FromArgb(255, color); // if less than 7 characters, assume alpha of 255 (full opacity)
					return color;
				}

				// try parsing as a known color - if not known, the ARGB value will be 0
				var knownColor = Color.FromName(value);
				if (knownColor.ToArgb() != 0) return knownColor;
			}
			return Color.Empty;
		}
	}
}