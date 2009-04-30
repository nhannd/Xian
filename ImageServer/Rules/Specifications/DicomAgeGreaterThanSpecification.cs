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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities;
using CommonSR=ClearCanvas.Common.SR;

namespace ClearCanvas.ImageServer.Rules.Specifications
{
    [ExtensionOf(typeof (XmlSpecificationCompilerOperatorExtensionPoint))]
    public class DicomAgeGreaterThanSpecificationOperator : IXmlSpecificationCompilerOperator
    {
        #region IXmlSpecificationCompilerOperator Members

        public string OperatorTag
        {
            get { return "dicom-age-greater-than"; }
        }

        public Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context)
        {
            string units = xmlNode.GetAttribute("units").ToLower();
            if (units == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'units' is required.");

            if (!units.Equals("years")
                && !units.Equals("weeks")
                && !units.Equals("days"))
                throw new XmlSpecificationCompilerException(
                    "Incorrect value for 'units' Xml attribute.  Should be 'years', 'weeks', or 'days'");

            string refValue = GetAttributeOrNull(xmlNode, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            return new DicomAgeGreaterThanSpecification(units, refValue);
        }

        public XmlSchemaElement GetSchema()
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "refValue";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaTypeName = new XmlQualifiedName("positiveInteger", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "test";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();

            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "years";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "weeks";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "days";
            restriction.Facets.Add(enumeration);

            simpleType.Content = restriction;

            attrib = new XmlSchemaAttribute();
            attrib.Name = "units";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaType = simpleType;
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "dicom-age-greater-than";
            element.SchemaType = type;

            return element;
        }

        #endregion

        private string GetAttributeOrNull(XmlElement node, string attr)
        {
            string val = node.GetAttribute(attr);
            return string.IsNullOrEmpty(val) ? null : val;
        }
    }

    public class DicomAgeGreaterThanSpecification : PrimitiveSpecification
    {
        private readonly string _refValue;
        private readonly string _units;

        public DicomAgeGreaterThanSpecification(string units, string refValue)
        {
            _units = units.ToLower();
            _refValue = refValue;
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            // assume that null matches anything
            if (exp == null || root == null)
                return DefaultTestResult(true);

            if (exp is string)
            {
                DateTime comparisonTime = Platform.Time;
                double time;
                if (false == double.TryParse(_refValue, out time))
                    throw new SpecificationException(CommonSR.ExceptionCastExpressionString);

                time = time*-1;

                if (_units.Equals("weeks"))
                    comparisonTime = comparisonTime.AddDays(time*7f);
                else if (_units.Equals("days"))
                    comparisonTime = comparisonTime.AddDays(time);
                else
                    comparisonTime = comparisonTime.AddYears((int) time);

                DateTime? testTime = DateTimeParser.Parse(exp as string);

                return DefaultTestResult(comparisonTime > testTime);
            }
            else
            {
                throw new SpecificationException(CommonSR.ExceptionCastExpressionString);
            }
        }
    }
}