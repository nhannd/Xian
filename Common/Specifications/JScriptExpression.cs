using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
    [ExtensionOf(typeof(ExpressionFactoryExtensionPoint))]
    [LanguageSupport("jscript")]
    public class JScriptExpressionFactory : IExpressionFactory
    {
        #region IExpressionFactory Members

        public Expression CreateExpression(string text)
        {
            return new JScriptExpression(text);
        }

        #endregion
    }

    public class JScriptExpression : Expression
    {
        [ThreadStatic]
        private static IScriptEngine _scriptEngine;
        private static readonly string AUTOMATIC_VARIABLE_TOKEN = "$";
        private IExecutableScript _script;

        public JScriptExpression(string text)
            :base(text)
        {
        }

        public override object Evaluate(object arg)
        {
            if(string.IsNullOrEmpty(this.Text))
                return null;

            if (this.Text == AUTOMATIC_VARIABLE_TOKEN)
                return arg;

            try
            {
                // create the script if not yet created
                if (_script == null)
                    _script = CreateScript(this.Text);

                // evaluate the test expression
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add(AUTOMATIC_VARIABLE_TOKEN, arg);
                return _script.Run(context);
            }
            catch (Exception e)
            {
                throw new SpecificationException(string.Format(SR.ExceptionJScriptEvaluation, this.Text), e);
            }
        }
        
        private static IExecutableScript CreateScript(string expression)
        {
            return ScriptEngine.CreateScript("return " + expression, new string[] { AUTOMATIC_VARIABLE_TOKEN });
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
