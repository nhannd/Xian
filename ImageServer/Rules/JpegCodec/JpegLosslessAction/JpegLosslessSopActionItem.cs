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
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Rules;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegLosslessAction
{
	/// <summary>
	/// JPEG Lossless SOP Compress action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class JpegLosslessSopActionItem : ActionItemBase<ServerActionContext>
	{
		private readonly bool _convertFromPalette;

		public JpegLosslessSopActionItem(bool convertFromPalette)
			: base("JPEG Lossless SOP compression action")
		{
			_convertFromPalette = convertFromPalette;
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1Uid;
			element.Attributes.Append(syntaxAttribute);

			syntaxAttribute = doc.CreateAttribute("convertFromPalette");
			syntaxAttribute.Value = _convertFromPalette.ToString();
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(new DicomCompressCommand(context.Message, doc));

			return true;
		}

	}
}
