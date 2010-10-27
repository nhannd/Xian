#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml;
using System.Xml.Schema;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
    /// Interface for extensions implementing <see cref="XmlActionCompilerOperatorExtensionPoint{TActionContext,TSchemaContext}"/>.
    /// </summary>
    public interface IXmlActionCompilerOperator<TActionContext, TSchemaContext>
    {
        /// <summary>
        /// The name of the action implemented.  This is typically the name of the <see cref="XmlElement"/> describing the action.
        /// </summary>
        string OperatorTag { get; }

        /// <summary>
        /// Method used to compile the action.  
        /// </summary>
        /// <param name="xmlNode">Input <see cref="XmlElement"/> describing the action to perform.</param>
        /// <returns>A class implementing the <see cref="IActionItem{T}"/> interface which can perform the action.</returns>
        IActionItem<TActionContext> Compile(XmlElement xmlNode);

        /// <summary>
        /// Get an <see cref="XmlSchemaElement"/> describing the ActionItem for validation purposes.
        /// </summary>
        /// <param name="context">A context in which the schema is being generated.</param>
        /// <returns></returns>
        XmlSchemaElement GetSchema(TSchemaContext context);
    }
}