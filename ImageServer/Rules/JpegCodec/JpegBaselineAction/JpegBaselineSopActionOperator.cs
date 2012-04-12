#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegBaselineAction
{
	/// <summary>
	/// Jpeg Baseline SOP Compress Action Operator, for use with <see cref="IXmlActionCompilerOperator{ServerActionContext,ServerRuleTypeEnum}"/>
	/// </summary>
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext, ServerRuleTypeEnum>))]
	public class JpegBaselineSopActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
	{
		public JpegBaselineSopActionOperator()
			: base("jpeg-baseline-sop")
		{
		}

		public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
		{
			int quality;
			if (false == int.TryParse(xmlNode.Attributes["quality"].Value, out quality))
				throw new XmlActionCompilerException("Unable to parse quality value for jpeg-baseline-sop quality factor");

			bool convertFromPalette = false;
			if (xmlNode.Attributes["convertFromPalette"] != null)
			{
				if (false == bool.TryParse(xmlNode.Attributes["convertFromPalette"].Value, out convertFromPalette))
					throw new XmlActionCompilerException("Unable to parse convertFromPalette value for jpeg-baseline-sop scheduling rule");
			}

			return new JpegBaselineSopActionItem(quality, convertFromPalette);
		}

		public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
		{
			if (!ruleType.Equals(ServerRuleTypeEnum.SopCompress))
				return null;

			XmlSchemaElement element = GetBaseSchema(OperatorTag);

			XmlSchemaAttribute attrib = new XmlSchemaAttribute();
			attrib.Name = "quality";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaTypeName = new XmlQualifiedName("unsignedByte", "http://www.w3.org/2001/XMLSchema");
			(element.SchemaType as XmlSchemaComplexType).Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute
			{
				Name = "convertFromPalette",
				Use = XmlSchemaUse.Optional,
				SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema")
			};
			(element.SchemaType as XmlSchemaComplexType).Attributes.Add(attrib);

			return element;
		}
	}
}
