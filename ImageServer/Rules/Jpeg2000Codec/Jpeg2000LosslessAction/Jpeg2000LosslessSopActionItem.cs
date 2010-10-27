#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Rules.Jpeg2000Codec.Jpeg2000LosslessAction
{
	/// <summary>
	/// JPEG 2000 Lossless SOP Compress action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class Jpeg2000LosslessSopActionItem : ServerActionItemBase
	{
		public Jpeg2000LosslessSopActionItem()
			: base("JPEG 2000 Lossless SOP compression action")
		{
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.Jpeg2000ImageCompressionLosslessOnlyUid;
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(
				new DicomCompressCommand(context.Message, doc));

			return true;
		}
	}
}
