#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Rules.Tier1RetentionAction
{
    [ExtensionOf(typeof (XmlActionCompilerOperatorExtensionPoint<ServerActionContext,ServerRuleTypeEnum>))]
    public class Tier1RetentionActionOperator : ActionOperatorCompilerBase,
												IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
    {
        public Tier1RetentionActionOperator()
            : base("tier1-retention")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["time"] == null)
                throw new XmlActionCompilerException("Unexpected missing time attribute for tier1-retention action");
            if (xmlNode.Attributes["unit"] == null)
                throw new XmlActionCompilerException("Unexpected missing unit attribute for tier1-retention action");

            int time;
            if (false == int.TryParse(xmlNode.Attributes["time"].Value, out time))
                throw new XmlActionCompilerException("Unable to parse time value for tier1-retention rule");

            string xmlUnit = xmlNode.Attributes["unit"].Value;
            TimeUnit unit = (TimeUnit) Enum.Parse(typeof (TimeUnit), xmlUnit, true);
            // this will throw exception if the unit is not defined

            string refValue = xmlNode.Attributes["refValue"] != null ? xmlNode.Attributes["refValue"].Value : null;
            if (!String.IsNullOrEmpty(refValue))
            {
                if (xmlNode["expressionLanguage"] != null)
                {
                    string language = xmlNode["expressionLanguage"].Value;
                    Expression scheduledTime = CreateExpression(refValue, language);
                    return new Tier1RetentionActionItem(time, unit, scheduledTime);
                }
                else
                {
                    Expression scheduledTime = CreateExpression(refValue);
                    return new Tier1RetentionActionItem(time, unit, scheduledTime);
                }
            }
            else
            {
                return new Tier1RetentionActionItem(time, unit);
            }
        }

        public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
        {
			if (!ruleType.Equals(ServerRuleTypeEnum.Tier1Retention))
				return null;

            XmlSchemaSimpleType timeUnitType = new XmlSchemaSimpleType();

            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "minutes";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "hours";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "days";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "weeks";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "months";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "years";
            restriction.Facets.Add(enumeration);

            timeUnitType.Content = restriction;


            XmlSchemaSimpleType languageType = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeRestriction languageEnum = new XmlSchemaSimpleTypeRestriction();
            languageEnum.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "dicom";
            languageEnum.Facets.Add(enumeration);
            languageType.Content = languageEnum;

            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "time";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "unit";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaType = timeUnitType;
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "refValue";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaType = languageType;
            type.Attributes.Add(attrib);


            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "tier1-retention";
            element.SchemaType = type;

            return element;
        }

        #endregion
    }
}