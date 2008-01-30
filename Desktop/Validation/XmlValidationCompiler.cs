using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    public class XmlValidationCompiler
    {
        private readonly XmlSpecificationCompiler _specCompiler;

        public XmlValidationCompiler()
            :this("jscript")
        {
        }

        public XmlValidationCompiler(string defaultExpressionLanguage)
        {
            _specCompiler = new XmlSpecificationCompiler(defaultExpressionLanguage);
        }

        public List<IValidationRule> CompileRules(TextReader rulesDocument)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(rulesDocument);

            return CompileRules(xmlDocument);
        }


        public List<IValidationRule> CompileRules(XmlDocument rulesDocument)
        {
            XmlNodeList ruleNodes = rulesDocument.SelectNodes("validation-rules/validation-rule");
            return CollectionUtils.Map<XmlNode, IValidationRule>(ruleNodes,
                delegate(XmlNode ruleNode) { return CompileRule((XmlElement)ruleNode); });
        }

        public IValidationRule CompileRule(XmlElement ruleNode)
        {
            string property = ruleNode.GetAttribute("boundProperty");

            ISpecification spec = _specCompiler.Compile(ruleNode);
            return new ValidationRule(property, spec);
        }
    }
}
