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
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Dicom.Utilities.Rules.Specifications
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
            var type = new XmlSchemaComplexType();

            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "refValue",
                                        Use = XmlSchemaUse.Required,
                                        SchemaTypeName = new XmlQualifiedName("positiveInteger", "http://www.w3.org/2001/XMLSchema")
                                    });

            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "test",
                                        Use = XmlSchemaUse.Optional,
                                        SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                    });

            var restriction = new XmlSchemaSimpleTypeRestriction
                                  {
                                      BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                  };

            restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = "years"});
            restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = "weeks"});
            restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = "days"});

            var simpleType = new XmlSchemaSimpleType
                                 {
                                     Content = restriction
                                 };


            type.Attributes.Add(new XmlSchemaAttribute
                                    {
                                        Name = "units", 
                                        Use = XmlSchemaUse.Required, 
                                        SchemaType = simpleType
                                    });


            type.Attributes.Add(new XmlSchemaAttribute
                         {
                             Name = "expressionLanguage",
                             Use = XmlSchemaUse.Optional,
                             SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                         });

            return new XmlSchemaElement
                       {
                           Name = "dicom-age-greater-than", 
                           SchemaType = type
                       };
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
                    throw new SpecificationException(Common.SR.ExceptionCastExpressionString);

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
            throw new SpecificationException(Common.SR.ExceptionCastExpressionString);
        }
    }
}