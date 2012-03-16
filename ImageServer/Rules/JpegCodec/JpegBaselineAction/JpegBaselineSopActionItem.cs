#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Command;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegBaselineAction
{
	/// <summary>
	/// JPEG Baseline SOP Compress action for <see cref="ServerRulesEngine"/>.
	/// </summary>
	public class JpegBaselineSopActionItem : ServerActionItemBase
	{
		private readonly int _quality;
		private readonly bool _convertFromPalette;

		public JpegBaselineSopActionItem(int quality, bool convertFromPalette)
			: base("JPEG Baseline SOP compression action")
		{
			_quality = quality;
			_convertFromPalette = convertFromPalette;
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.JpegBaselineProcess1Uid;
			element.Attributes.Append(syntaxAttribute);

			syntaxAttribute = doc.CreateAttribute("quality");
			syntaxAttribute.Value = _quality.ToString();
			element.Attributes.Append(syntaxAttribute);

			syntaxAttribute = doc.CreateAttribute("convertFromPalette");
			syntaxAttribute.Value = _convertFromPalette.ToString();
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(new DicomCompressCommand(context.Message, doc));

			return true;
		}
	}
}
