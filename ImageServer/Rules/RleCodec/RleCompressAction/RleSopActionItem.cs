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

namespace ClearCanvas.ImageServer.Rules.RleCodec.RleCompressAction
{
	/// <summary>
	/// RLE SOP compression action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class RleSopActionItem : ServerActionItemBase
	{
		private readonly bool _convertFromPalette;

		public RleSopActionItem(bool convertFromPalette)
			: base("RLE SOP compression action")
		{
			_convertFromPalette = convertFromPalette;
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.RleLosslessUid;
			element.Attributes.Append(syntaxAttribute);

			syntaxAttribute = doc.CreateAttribute("convertFromPalette");
			syntaxAttribute.Value = _convertFromPalette.ToString();
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(new DicomCompressCommand(context.Message, doc));

			return true;
		}
	}
}
