using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Rules.AutoRoute
{
    [ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ViewerActionContext, RuleActionType>))]
    public class AutoRouteActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ViewerActionContext, RuleActionType>
    {
        public AutoRouteActionOperator()
            : base("auto-route")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ViewerActionContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["device"] == null)
                throw new XmlActionCompilerException("Unexpected missing device attribute for auto-route action");

            string device = xmlNode.Attributes["device"].Value;

            string transferSyntaxUid = null;
            if (xmlNode.Attributes["transferSyntaxUid"] != null)
                transferSyntaxUid = xmlNode.Attributes["transferSyntaxUid"].Value;

            if ((xmlNode.Attributes["startTime"] != null)
                && (xmlNode.Attributes["endTime"] != null))
            {
                DateTime startTime;
                if (!DateTime.TryParseExact(xmlNode.Attributes["startTime"].Value, "HH:mm:ss",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None,
                                            out startTime))
                {
                    throw new XmlActionCompilerException("Incorrect format of startTime: " + xmlNode.Attributes["startTime"].Value);
                }

                DateTime endTime;
                if (!DateTime.TryParseExact(xmlNode.Attributes["endTime"].Value, "HH:mm:ss",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None,
                                            out endTime))
                {
                    throw new XmlActionCompilerException("Incorrect format of endTime: " + xmlNode.Attributes["endTime"].Value);
                }

                return new AutoRouteActionItem(device, startTime, endTime, transferSyntaxUid);
            }
            if ((xmlNode.Attributes["startTime"] == null)
                && (xmlNode.Attributes["endTime"] != null))
                throw new XmlActionCompilerException("Unexpected missing startTime attribute for auto-route action");
            if ((xmlNode.Attributes["startTime"] != null)
                && (xmlNode.Attributes["endTime"] == null))
                throw new XmlActionCompilerException("Unexpected missing endTime attribute for auto-route action");

            return new AutoRouteActionItem(device, transferSyntaxUid);
        }

        public XmlSchemaElement GetSchema(RuleActionType ruleType)
        {
            if (!ruleType.Equals(RuleActionType.AutoRoute))
                return null;

            var type = new XmlSchemaComplexType();

            type.Attributes.Add(new XmlSchemaAttribute
                                            {
                                                Name = "device",
                                                Use = XmlSchemaUse.Required,
                                                SchemaTypeName =
                                                    new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                            });

            type.Attributes.Add(new XmlSchemaAttribute
                         {
                             Name = "startTime",
                             Use = XmlSchemaUse.Optional,
                             SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                         });

            type.Attributes.Add(new XmlSchemaAttribute
                         {
                             Name = "endTime",
                             Use = XmlSchemaUse.Optional,
                             SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                         });

            type.Attributes.Add(new XmlSchemaAttribute
            {
                Name = "transferSyntaxUid",
                Use = XmlSchemaUse.Optional,
                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
            });


            var element = new XmlSchemaElement
                              {
                                  Name = "auto-route", 
                                  SchemaType = type
                              };

            return element;
        }

        #endregion
    }
}
