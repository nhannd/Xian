#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegExtendedAction
{
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext, ServerRuleTypeEnum>))]
	public class JpegExtendedActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
	{
		public JpegExtendedActionOperator()
			: base("jpeg-extended")
		{
		}

		public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
		{
			if (xmlNode.Attributes["time"] == null)
				throw new XmlActionCompilerException(
					"Unexpected missing time attribute for jpeg-extended scheduling action");
			if (xmlNode.Attributes["unit"] == null)
				throw new XmlActionCompilerException(
					"Unexpected missing unit attribute for jpeg-extended scheduling action");

			int quality;
			if (false == int.TryParse(xmlNode.Attributes["quality"].Value, out quality))
				throw new XmlActionCompilerException("Unable to parse quality value for jpeg-extended scheduling rule");

			int time;
			if (false == int.TryParse(xmlNode.Attributes["time"].Value, out time))
				throw new XmlActionCompilerException("Unable to parse time value for jpeg-extended scheduling rule");

			string xmlUnit = xmlNode.Attributes["unit"].Value;

			// this will throw exception if the unit is not defined
			TimeUnit unit = (TimeUnit)Enum.Parse(typeof(TimeUnit), xmlUnit, true);

			string refValue = xmlNode.Attributes["refValue"] != null ? xmlNode.Attributes["refValue"].Value : null;

			bool convertFromPalette = false;
			if (xmlNode.Attributes["convertFromPalette"] != null)
			{
				if (false == bool.TryParse(xmlNode.Attributes["convertFromPalette"].Value, out convertFromPalette))
					throw new XmlActionCompilerException("Unable to parse convertFromPalette value for jpeg-extended scheduling rule");
			}

			if (!String.IsNullOrEmpty(refValue))
			{
				if (xmlNode["expressionLanguage"] != null)
				{
					string language = xmlNode["expressionLanguage"].Value;
					Expression scheduledTime = CreateExpression(refValue, language);
					return new JpegExtendedActionItem(time, unit, scheduledTime, quality, convertFromPalette);
				}
				else
				{
					Expression scheduledTime = CreateExpression(refValue);
					return new JpegExtendedActionItem(time, unit, scheduledTime, quality, convertFromPalette);
				}
			}

			return new JpegExtendedActionItem(time, unit, quality, convertFromPalette);
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