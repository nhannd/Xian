using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    internal class Builder
    {
        delegate Specification CreationDelegate(XmlElement xmlNode, string testExp, string failureMessage);

        private Dictionary<string, CreationDelegate> _factoryMethodMap = new Dictionary<string, CreationDelegate>();
        private SpecificationFactory _factory;

        internal Builder(SpecificationFactory factory)
        {
            _factory = factory;

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

        public Specification BuildSpecification(XmlElement specificationNode)
        {
            return CreateImplicitAnd(GetChildElements(specificationNode));
        }

        private Specification BuildNode(XmlElement node)
        {
            string test = node.GetAttribute("test");
            string failMessage = node.GetAttribute("failMessage");
            return _factoryMethodMap[node.LocalName](node, test == "" ? null : test, failMessage == "" ? null : failMessage);
        }

        private Specification CreateAnd(XmlElement node, string testExpr, string failureMessage)
        {
            AndSpecification spec = new AndSpecification(testExpr, failureMessage);
            foreach (XmlElement child in GetChildElements(node))
            {
                spec.Add(BuildNode(child));
            }
            return spec;
        }

        private Specification CreateOr(XmlElement node, string testExpr, string failureMessage)
        {
            OrSpecification spec = new OrSpecification(testExpr, failureMessage);
            foreach (XmlElement child in GetChildElements(node))
            {
                spec.Add(BuildNode(child));
            }
            return spec;
        }

        private Specification CreateRegex(XmlElement node, string testExpr, string failureMessage)
        {
            return new RegexSpecification(testExpr, node.GetAttribute("pattern"), failureMessage);
        }

        private Specification CreateNotNull(XmlElement node, string testExpr, string failureMessage)
        {
            return new NotNullSpecification(testExpr, failureMessage);
        }

        private Specification CreateIsNull(XmlElement node, string testExpr, string failureMessage)
        {
            return new IsNullSpecification(testExpr, failureMessage);
        }

        private Specification CreateCount(XmlElement node, string testExpr, string failureMessage)
        {
            string minString = node.GetAttribute("min");
            string maxString = node.GetAttribute("max");

            int min = (minString == "") ? 0 : Int32.Parse(minString);
            int max = (maxString == "") ? Int32.MaxValue : Int32.Parse(maxString);

            return new CountSpecification(testExpr, min, max, failureMessage);
        }

        private Specification CreateEach(XmlElement node, string testExpr, string failureMessage)
        {
            Specification elementSpec = CreateImplicitAnd(GetChildElements(node));
            return new EachSpecification(testExpr, elementSpec, failureMessage);
        }

        private Specification CreateAny(XmlElement node, string testExpr, string failureMessage)
        {
            Specification elementSpec = CreateImplicitAnd(GetChildElements(node));
            return new AnySpecification(testExpr, elementSpec, failureMessage);
        }

        private Specification CreateTrue(XmlElement node, string testExpr, string failureMessage)
        {
            return new TrueSpecification(testExpr, failureMessage);
        }

        private Specification CreateFalse(XmlElement node, string testExpr, string failureMessage)
        {
            return new FalseSpecification(testExpr, failureMessage);
        }

        private Specification CreateDefined(XmlElement node, string testExpr, string failureMessage)
        {
            return new DefinedSpecification(testExpr, _factory.GetSpecification(node.GetAttribute("spec")), failureMessage);
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
    }
}
