using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class HtmlFormSelector
    {
        private string _scriptSource;
        private string[] _scriptVariableNames;

        private IExecutableScript _script;

        public HtmlFormSelector(string selectorScript, string[] variableNames)
        {
            _scriptSource = selectorScript;
            _scriptVariableNames = variableNames;
        }

        public string SelectForm(params object[] arguments)
        {
            if (_script == null)
                LoadScript();

            Dictionary<string, object> context = new Dictionary<string,object>();
            for(int i = 0; i < _scriptVariableNames.Length; i++)
            {
                context[_scriptVariableNames[i]] = arguments[i];
            }
            
            object result = _script.Run(context);

            return result as string;    // null if not a string
        }

        private void LoadScript()
        {
            IScriptEngine engine = ScriptEngineFactory.CreateEngine("jscript");
            _script = engine.CreateScript(_scriptSource, _scriptVariableNames);
        }
    }
}
