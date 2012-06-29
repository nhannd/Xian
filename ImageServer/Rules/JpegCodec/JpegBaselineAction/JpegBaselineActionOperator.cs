#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegBaselineAction
{
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext,ServerRuleTypeEnum>))]
	public class JpegBaselineActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
	{
		public JpegBaselineActionOperator()
			: base("jpeg-baseline")
		{
		}

		public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
		{
			if (xmlNode.Attributes["time"] == null)
				throw new XmlActionCompilerException(
					"Unexpected missing time attribute for jpeg-baseline scheduling action");
			if (xmlNode.Attributes["unit"] == null)
				throw new XmlActionCompilerException(
					"Unexpected missing unit attribute for jpeg-baseline scheduling action");

			int quality;
			if (false == int.TryParse(xmlNode.Attributes["quality"].Value, out quality))
				throw new XmlActionCompilerException("Unable to parse quality value for jpeg-baseline scheduling rule");

			int time;
			if (false == int.TryParse(xmlNode.Attributes["time"].Value, out time))
				throw new XmlActionCompilerException("Unable to parse time value for jpeg-baseline scheduling rule");

			string xmlUnit = xmlNode.Attributes["unit"].Value;

			// this will throw exception if the unit is not defined
			var unit = (TimeUnit)Enum.Parse(typeof(TimeUnit), xmlUnit, true);

			bool convertFromPalette = false;
			if (xmlNode.Attributes["convertFromPalette"] != null)
			{
				if (false == bool.TryParse(xmlNode.Attributes["convertFromPalette"].Value, out convertFromPalette))
					throw new XmlActionCompilerException("Unable to parse convertFromPalette value for jpeg-baseline scheduling rule");
			}

			string refValue = xmlNode.Attributes["refValue"] != null ? xmlNode.Attributes["refValue"].Value : null;

			if (!String.IsNullOrEmpty(refValue))
			{
				if (xmlNode["expressionLanguage"] != null)
				{
					string language = xmlNode["expressionLanguage"].Value;
					Expression scheduledTime = CreateExpression(refValue, language);
					return new JpegBaselineActionItem(time, unit, scheduledTime, quality, convertFromPalette);
				}
				else
				{
					Expression scheduledTime = CreateExpression(refValue);
					return new JpegBaselineActionItem(time, unit, scheduledTime, quality, convertFromPalette);
				}
			}
			
			return new JpegBaselineActionItem(time, unit, quality, convertFromPalette);
		}

		public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
		{
			if (!ruleType.Equals(ServerRuleTypeEnum.StudyCompress))
				return null;

			XmlSchemaElement element = GetTimeSchema(OperatorTag);

			XmlSchemaAttribute attrib = new XmlSchemaAttribute
			                            	{
			                            		Name = "quality",
			                            		Use = XmlSchemaUse.Required,
			                            		SchemaTypeName =
			                            			new XmlQualifiedName("unsignedByte", "http://www.w3.org/2001/XMLSchema")
			                            	};
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