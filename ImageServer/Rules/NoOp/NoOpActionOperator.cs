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

namespace ClearCanvas.ImageServer.Rules.NoOp
{
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext, ServerRuleTypeEnum>))]
	public class NoOpActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
    {
        public NoOpActionOperator()
            : base("no-op")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            return new NoOpActionItem();
        }

        public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "no-op";
            element.SchemaType = type;

            return element;
        }

        #endregion
    }
}