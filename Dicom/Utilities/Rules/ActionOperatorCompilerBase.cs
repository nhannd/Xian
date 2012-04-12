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
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Base class for server rule action operator compiler.
    /// </summary>
    public abstract class ActionOperatorCompilerBase
    {
        
        #region Private Memebers
        private string _operatorTag;
        private static readonly IExpressionFactory _defaultExpressionFactory = new ExpressionFactoryExtensionPoint().CreateExtension("dicom");
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ActionOperatorCompilerBase"/>
        /// </summary>
        /// <param name="operatorTag">The operator tag for the operator</param>
        public ActionOperatorCompilerBase(string operatorTag)
        {
            _operatorTag = operatorTag;
        }

        #endregion

        #region Public Properties

        public string OperatorTag
        {
            get { return _operatorTag; }
            set { _operatorTag = value; }
        }

        #endregion

        #region Protected Static Methods

        protected static IExpressionFactory CreateExpressionFactory(string language)
        {
            IExpressionFactory factory = new ExpressionFactoryExtensionPoint().CreateExtension(language);
            if (factory == null)
            {
                return _defaultExpressionFactory;
            }
            else
                return factory;
        }

        protected static Expression CreateExpression(string text, string language)
        {
            IExpressionFactory exprFactory = CreateExpressionFactory(language);
            if (language != null)
                exprFactory = _defaultExpressionFactory;

            return exprFactory.CreateExpression(text);
        }

        protected static Expression CreateExpression(string text)
        {
            return _defaultExpressionFactory.CreateExpression(text);
        }

		protected static XmlSchemaElement GetTimeSchema(string elementName)
		{
			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();

			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "minutes";
			restriction.Facets.Add(enumeration);

			enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "hours";
			restriction.Facets.Add(enumeration);

			enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "weeks";
			restriction.Facets.Add(enumeration);

			enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "days";
			restriction.Facets.Add(enumeration);

			enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "months";
			restriction.Facets.Add(enumeration);

			enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "years";
			restriction.Facets.Add(enumeration);

			simpleType.Content = restriction;


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
			attrib.SchemaTypeName = new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema");
			type.Attributes.Add(attrib);

			attrib = new XmlSchemaAttribute();
			attrib.Name = "unit";
			attrib.Use = XmlSchemaUse.Required;
			attrib.SchemaType = simpleType;
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
			element.Name = elementName;
			element.SchemaType = type;

			return element;
		}

		protected static XmlSchemaElement GetBaseSchema(string elementName)
		{
			XmlSchemaSimpleType languageType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeRestriction languageEnum = new XmlSchemaSimpleTypeRestriction();
			languageEnum.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
			enumeration.Value = "dicom";
			languageEnum.Facets.Add(enumeration);
			languageType.Content = languageEnum;

			XmlSchemaComplexType type = new XmlSchemaComplexType();

			XmlSchemaAttribute attrib = new XmlSchemaAttribute();
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
			element.Name = elementName;
			element.SchemaType = type;

			return element;
		}
        #endregion
    }
}