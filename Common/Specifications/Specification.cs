using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
    public abstract class Specification : ISpecification
    {
        private static readonly string AUTOMATIC_VARIABLE_TOKEN = "$";
        private static readonly string AUTOMATIC_VARIABLE_IMPLEMENTATION = "__root__";

        [ThreadStatic]
        private static IScriptEngine _scriptEngine;

        private string _testExpr;
        private string _failureMessage;

        private IExecutableScript _testExprScript;

        public Specification(string testExpression, string failureMessage)
        {
            _testExpr = testExpression;
            _failureMessage = failureMessage;
        }

        public string TestExpression
        {
            get { return _testExpr; }
            set
            {
                _testExpr = value;
                _testExprScript = null; // invalidate the script object so that it will be re-created
            }
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
            set { _failureMessage = value; }
        }

        #region ISpecification Members

        public abstract IEnumerable<ISpecification> SubSpecs { get; }

        public TestResult Test(object obj)
        {
            if (this.TestExpressionScript != null)
            {
                try
                {
                    // evaluate the test expression
                    Dictionary<string, object> context = new Dictionary<string, object>();
                    context.Add(AUTOMATIC_VARIABLE_IMPLEMENTATION, obj);
                    obj = this.TestExpressionScript.Run(context);
                }
                catch (Exception e)
                {
					throw new SpecificationException(string.Format(SR.ExceptionJScriptEvaluation, _testExpr), e);
                }
            }

            return InnerTest(obj);
        }

        #endregion

        protected abstract TestResult InnerTest(object exp);

        private IExecutableScript TestExpressionScript
        {
            get
            {
                if (_testExprScript == null && _testExpr != null)
                {
                    _testExprScript = CreateScript(_testExpr);
                }
                return _testExprScript;
            }
        }

        private static IExecutableScript CreateScript(string expression)
        {

            if (expression == null || expression.Length == 0 || expression == AUTOMATIC_VARIABLE_TOKEN)
                return null;

            StringBuilder script = new StringBuilder();
            script.Append("return ");
            script.Append(expression.Replace(AUTOMATIC_VARIABLE_TOKEN, string.Format("this.{0}", AUTOMATIC_VARIABLE_IMPLEMENTATION)));

            return ScriptEngine.CreateScript(script.ToString());
        }

        private static IScriptEngine ScriptEngine
        {
            get
            {
                if (_scriptEngine == null)
                {
                    _scriptEngine = ScriptEngineFactory.CreateEngine("jscript");
                }
                return _scriptEngine;
            }
        }

    }
}
