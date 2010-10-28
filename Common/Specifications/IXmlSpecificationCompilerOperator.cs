#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Xml;
using System.Xml.Schema;

namespace ClearCanvas.Common.Specifications
{
    /// <summary>
    /// Interface for Specification Operators.
    /// </summary>
    public interface IXmlSpecificationCompilerOperator
    {
        /// <summary>
        /// The XML Tag for the operator.
        /// </summary>
        string OperatorTag { get; }
        /// <summary>
        /// Compile the operator.
        /// </summary>
        /// <param name="xmlNode">The XML Node associated with the operator.</param>
        /// <param name="context">A context for the compiler.</param>
        /// <returns>A compiled <see cref="Specification"/>.</returns>
        Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context);
        /// <summary>
        /// Get an XmlSchema element that describes the schema for the operator element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is assumed that a simple <see cref="XmlSchemaElement"/> is returned for the 
        /// operator.  The compiler combine the elements for each operator together into an
        /// <see cref="XmlSchema"/>.  If the specific element allows subelements, it should 
        /// be declared to allow any elements from the local namespace/Schema.
        /// </para>
        /// </remarks>
        /// <returns>The Schema element.</returns>
        XmlSchemaElement GetSchema();
    }
}
