using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class XmlSpecificationCompiler
    {
        delegate Specification CreationDelegate(XmlElement xmlNode);

        private Dictionary<string, CreationDelegate> _factoryMethodMap = new Dictionary<string, CreationDelegate>();
        private ISpecificationProvider _resolver;

        public XmlSpecificationCompiler(ISpecificationProvider resolver)
        {
            _resolver = resolver;

            _factoryMethodMap.Add("true", CreateTrue);
            _factoryMethodMap.Add("false", CreateFalse);
            _factoryMethodMap.Add("and", CreateAnd);
            _factoryMethodMap.Add("or", CreateOr);
            _factoryMethodMap.Add("regex", CreateRegex);
            _factoryMethodMap.Add("null", CreateIsNull);
            _factoryMethodMap.Add("not-null", CreateNotNull);
            _factoryMethodMap.Add("count", CreateCount);
            _factoryMethodMap.Add("each", CreateEach);
            _factoryMethodMap.Add("any", CreateAny);
            _factoryMethodMap.Add("defined", CreateDefined);
        }

        public XmlSpecificationCompiler()
            :this(null)
        {
        }

        public Specification BuildSpecification(XmlElement specificationNode)
        {
            return CreateImplicitAnd(GetChildElements(specificationNode));
        }

        private Specification BuildNode(XmlElement node)
        {
            Specification spec = _factoryMethodMap[node.LocalName](node);
            spec.TestExpression = GetAttributeOrNull(node, "test");
            spec.IfExpression = GetAttributeOrNull(node, "if");
            spec.FailureMessage = GetAttributeOrNull(node, "failMessage");
            return spec;
        }

        private Specification CreateAnd(XmlElement node)
        {
            AndSpecification spec = new AndSpecification();
            foreach (XmlElement child in GetChildElements(node))
            {
                spec.Add(BuildNode(child));
            }
            return spec;
        }

        private Specification CreateOr(XmlElement node)
        {
            OrSpecification spec = new OrSpecification();
            foreach (XmlElement child in GetChildElements(node))
            {
                spec.Add(BuildNode(child));
            }
            return spec;
        }

        private Specification CreateRegex(XmlElement node)
        {
            return new RegexSpecification(node.GetAttribute("pattern"));
        }

        private Specification CreateNotNull(XmlElement node)
        {
            return new NotNullSpecification();
        }

        private Specification CreateIsNull(XmlElement node)
        {
            return new IsNullSpecification();
        }

        private Specification CreateCount(XmlElement node)
        {
            string minString = node.GetAttribute("min");
            string maxString = node.GetAttribute("max");

            int min = (minString == "") ? 0 : Int32.Parse(minString);
            int max = (maxString == "") ? Int32.MaxValue : Int32.Parse(maxString);

            return new CountSpecification(min, max);
        }

        private Specification CreateEach(XmlElement node)
        {
            Specification elementSpec = CreateImplicitAnd(GetChildElements(node));
            return new EachSpecification(elementSpec);
        }

        private Specification CreateAny(XmlElement node)
        {
            Specification elementSpec = CreateImplicitAnd(GetChildElements(node));
            return new AnySpecification(elementSpec);
        }

        private Specification CreateTrue(XmlElement node)
        {
            return new TrueSpecification();
        }

        private Specification CreateFalse(XmlElement node)
        {
            return new FalseSpecification();
        }

        private Specification CreateDefined(XmlElement node)
        {
            string id = node.GetAttribute("spec");
            if (_resolver == null)
                throw new XmlSpecificationCompilerException(string.Format("Cannot resolve reference {0} because no resolver was provided.", id));

            return new DefinedSpecification(_resolver.GetSpecification(id));
        }

        private Specification CreateImplicitAnd(ICollection<XmlNode> nodes)
        {
            if (nodes.Count == 1)
            {
                // only 1 node, so we don't need to "and"
                return BuildNode((XmlElement)CollectionUtils.FirstElement(nodes));
            }
            else
            {
                // create an "and" for the child nodes
                AndSpecification spec = new AndSpecification(null, null);
                foreach (XmlElement node in nodes)
                {
                    spec.Add(BuildNode(node));
                }
                return spec;
            }
        }

        private ICollection<XmlNode> GetChildElements(XmlElement node)
        {
            return CollectionUtils.Select<XmlNode>(node.ChildNodes, delegate(XmlNode child) { return child is XmlElement; });
        }

        private string GetAttributeOrNull(XmlElement node, string attr)
        {
            string val = node.GetAttribute(attr);
            return val == "" ? null : val;
        }
    }
}
