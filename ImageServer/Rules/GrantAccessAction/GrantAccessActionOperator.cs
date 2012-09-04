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

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext, ServerRuleTypeEnum>))]
	public class GrantAccessActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
    {
        public GrantAccessActionOperator()
            : base("grant-access")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["authorityGroupOid"] == null)
                throw new XmlActionCompilerException("Unexpected missing authorityGroupOid attribute for grant-access action");

            string authorityGroup = xmlNode.Attributes["authorityGroupOid"].Value;

            return new GrantAccessActionItem(authorityGroup);
        }

		public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
		{
			if (!ruleType.Equals(ServerRuleTypeEnum.DataAccess))
				return null;

            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute
                                            {
                                                Name = "authorityGroupOid",
                                                Use = XmlSchemaUse.Required,
                                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                            };
		    type.Attributes.Add(attrib);

            XmlSchemaElement element = new XmlSchemaElement
                                           {
                                               Name = "grant-access", 
                                               SchemaType = type
                                           };
		    return element;
        }

        #endregion
    }
}