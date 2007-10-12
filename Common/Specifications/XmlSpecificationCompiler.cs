#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
    [ExtensionPoint]
    public class ExpressionFactoryExtensionPoint : ExtensionPoint<IExpressionFactory>
    {
        public IExpressionFactory CreateExtension(string language)
        {
            return (IExpressionFactory) CreateExtension(new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
        }
    }

    public interface IXmlSpecificationCompilerContext
    {
        IExpressionFactory DefaultExpressionFactory { get; }
        IExpressionFactory GetExpressionFactory(string language);
        ISpecification Compile(XmlElement containingNode);
        ISpecification GetSpecification(string id);
    }

    [ExtensionPoint]
    public class XmlSpecificationCompilerOperatorExtensionPoint : ExtensionPoint<IXmlSpecificationCompilerOperator>
    {
    }


    public class XmlSpecificationCompiler
    {
        #region IXmlSpecificationCompilerContext implementation class

        class Context : IXmlSpecificationCompilerContext
        {
            private XmlSpecificationCompiler _compiler;

            public Context(XmlSpecificationCompiler compiler)
            {
                _compiler = compiler;
            }

            #region IXmlSpecificationCompilerContext Members

            public IExpressionFactory DefaultExpressionFactory
            {
                get { return _compiler._defaultExpressionFactory; }
            }

            public IExpressionFactory GetExpressionFactory(string language)
            {
                return XmlSpecificationCompiler.CreateExpressionFactory(language);
            }

            public ISpecification Compile(XmlElement containingNode)
            {
                return _compiler.Compile(containingNode);
            }

            public ISpecification GetSpecification(string id)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        delegate Specification CreationDelegate(XmlElement xmlNode);

        #region BuiltInOperator class

        class BuiltInOperator : IXmlSpecificationCompilerOperator
        {
            private string _operator;
            private CreationDelegate _factoryMethod;

            public BuiltInOperator(string op, CreationDelegate factoryMethod)
            {
                _operator = op;
                _factoryMethod = factoryMethod;
            }

            #region IXmlSpecificationCompilerOperator Members

            public string OperatorTag
            {
                get { return _operator; }
            }

            public Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context)
            {
                return _factoryMethod(xmlNode);
            }

            #endregion
        }

        #endregion

        private Dictionary<string, IXmlSpecificationCompilerOperator> _operatorMap = new Dictionary<string, IXmlSpecificationCompilerOperator>();
        private ISpecificationProvider _resolver;
        private readonly IExpressionFactory _defaultExpressionFactory;
        private readonly IXmlSpecificationCompilerContext _compilerContext;

        public XmlSpecificationCompiler(ISpecificationProvider resolver, IExpressionFactory defaultExpressionFactory)
        {
            _resolver = resolver;

            _defaultExpressionFactory = defaultExpressionFactory;
            _compilerContext = new Context(this);

            // declare built-in operators
            AddOperator(new BuiltInOperator("true", CreateTrue));
            AddOperator(new BuiltInOperator("false", CreateFalse));
            AddOperator(new BuiltInOperator("equal", CreateEqual));
            AddOperator(new BuiltInOperator("not-equal", CreateNotEqual));
            AddOperator(new BuiltInOperator("greater-than", CreateGreaterThan));
            AddOperator(new BuiltInOperator("less-than", CreateLessThan));
            AddOperator(new BuiltInOperator("and", CreateAnd));
            AddOperator(new BuiltInOperator("or", CreateOr));
            AddOperator(new BuiltInOperator("regex", CreateRegex));
            AddOperator(new BuiltInOperator("null", CreateIsNull));
            AddOperator(new BuiltInOperator("not-null", CreateNotNull));
            AddOperator(new BuiltInOperator("count", CreateCount));
            AddOperator(new BuiltInOperator("each", CreateEach));
            AddOperator(new BuiltInOperator("any", CreateAny));
            AddOperator(new BuiltInOperator("defined", CreateDefined));

            // add extension operators
            XmlSpecificationCompilerOperatorExtensionPoint xp = new XmlSpecificationCompilerOperatorExtensionPoint();
            foreach (IXmlSpecificationCompilerOperator compilerOperator in xp.CreateExtensions())
            {
                AddOperator(compilerOperator);
            }
        }

        public XmlSpecificationCompiler(ISpecificationProvider resolver, string defaultExpressionLanguage)
            : this(resolver, CreateExpressionFactory(defaultExpressionLanguage))
        {
        }


        public XmlSpecificationCompiler(IExpressionFactory defaultExpressionFactory)
            : this(null, defaultExpressionFactory)
        {
        }

        public XmlSpecificationCompiler(string defaultExpressionLanguage)
            : this(null, CreateExpressionFactory(defaultExpressionLanguage))
        {
        }

        public ISpecification Compile(XmlElement containingNode)
        {
            return CreateImplicitAnd(GetChildElements(containingNode));
        }

        private void AddOperator(IXmlSpecificationCompilerOperator op)
        {
            _operatorMap.Add(op.OperatorTag, op);
        }

        private Specification BuildNode(XmlElement node)
        {
            IXmlSpecificationCompilerOperator op = _operatorMap[node.Name];
            Specification spec = op.Compile(node, _compilerContext);

            string test = GetAttributeOrNull(node, "test");
            if(test != null)
            {
                spec.TestExpression = CreateExpression(test, GetAttributeOrNull(node, "expressionLanguage"));
            }

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

        private Specification CreateEqual(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            EqualSpecification s = new EqualSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
            return s;
        }

        private Specification CreateNotEqual(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            NotEqualSpecification s = new NotEqualSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
            return s;
        }

        private Specification CreateGreaterThan(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            GreaterThanSpecification s = new GreaterThanSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));

            string inclusive = GetAttributeOrNull(node, "inclusive");
            if (inclusive != null)
                s.Inclusive = bool.Parse(inclusive);
            return s;
        }

        private Specification CreateLessThan(XmlElement node)
        {
            string refValue = GetAttributeOrNull(node, "refValue");
            if (refValue == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'refValue' is required.");

            LessThanSpecification s = new LessThanSpecification();
            s.RefValueExpression = CreateExpression(refValue, GetAttributeOrNull(node, "expressionLanguage"));
            string inclusive = GetAttributeOrNull(node, "inclusive");
            if (inclusive != null)
                s.Inclusive = bool.Parse(inclusive);
            return s;
        }

        private Specification CreateDefined(XmlElement node)
        {
            string id = GetAttributeOrNull(node, "spec");
            if (id == null)
                throw new XmlSpecificationCompilerException("Xml attribute 'spec' is required.");

            return new DefinedSpecification(ResolveSpecification(id));
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
                AndSpecification spec = new AndSpecification();
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
            return string.IsNullOrEmpty(val) ? null : val;
        }

        private ISpecification ResolveSpecification(string id)
        {
            if (_resolver == null)
                throw new XmlSpecificationCompilerException(string.Format("Cannot resolve reference {0} because no resolver was provided.", id));
            return _resolver.GetSpecification(id);
        }

        private Expression CreateExpression(string text, string language)
        {
            IExpressionFactory exprFactory = _defaultExpressionFactory;
            if (language != null)
                exprFactory = CreateExpressionFactory(language);

            return exprFactory.CreateExpression(text);
        }

        private static IExpressionFactory CreateExpressionFactory(string language)
        {
            return new ExpressionFactoryExtensionPoint().CreateExtension(language);
        }
    }
}
