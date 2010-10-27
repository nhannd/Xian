#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Text;
using System.Xml;

namespace ClearCanvas.ImageServer.Model
{
	public partial class Alert
	{
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			StringWriter sw = new StringWriter();
			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.Encoding = Encoding.UTF8;
			xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlSettings.Indent = true;
			xmlSettings.NewLineOnAttributes = false;
			xmlSettings.CheckCharacters = true;
			xmlSettings.IndentChars = "  ";

			XmlWriter xw = XmlWriter.Create(sw, xmlSettings);

			Content.WriteTo(xw);

			xw.Close();

			sb.AppendFormat("{0} {1} {2} {3} [{4}] - {5}",
			                InsertTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
			                Source,
			                AlertLevelEnum.Description,
			                AlertCategoryEnum.Description,
			                Component, sw);

			return sb.ToString();
		}
	}
}
