using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
    internal class Expression
    {
        public static readonly Expression Null = new Expression(null);

        [ThreadStatic]
        private static IScriptEngine _scriptEngine;
        private static readonly string AUTOMATIC_VARIABLE_TOKEN = "$";
        private static readonly string AUTOMATIC_VARIABLE_IMPLEMENTATION = "__root__";


        private string _text;
        private IExecutableScript _script;

        public Expression(string text)
        {
            // treat "" as null
            _text = (text == "") ? null : text;
        }

        public string Text
        {
            get { return _text; }
        }

        public object Evaluate(object arg)
        {
            if (_text == null || _text.Length == 0)
                return null;

            if (_text == AUTOMATIC_VARIABLE_TOKEN)
                return arg;

            try
            {
                // create the script if not yet created
                if (_script == null)
                    _script = CreateScript(_text);

                // evaluate the test expression
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add(AUTOMATIC_VARIABLE_IMPLEMENTATION, arg);
                return _script.Run(context);
            }
            catch (Exception e)
            {
                throw new SpecificationException(string.Format(SR.ExceptionJScriptEvaluation, _text), e);
            }
        }

        public override bool Equals(object obj)
        {
            Expression other = obj as Expression;
            return other != null && other._text == this._text;
        }

        public override int GetHashCode()
        {
            return _text == null ? 0 : _text.GetHashCode();
        }


        private static IExecutableScript CreateScript(string expression)
        {
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
