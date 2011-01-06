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
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Rules.Jpeg2000Codec.Jpeg2000LossyAction
{
	/// <summary>
	/// JPEG 2000 Lossy action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class Jpeg2000LossySopActionItem : ServerActionItemBase
	{
		private readonly float _ratio;


		public Jpeg2000LossySopActionItem(float ratio)
			: base("JPEG 2000 Lossy SOP compression action")
		{
			_ratio = ratio;
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.Jpeg2000ImageCompressionUid;
			element.Attributes.Append(syntaxAttribute);

			syntaxAttribute = doc.CreateAttribute("ratio");
			syntaxAttribute.Value = _ratio.ToString();
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(new DicomCompressCommand(context.Message, doc));

			return true;
		}
	}
}
