import System;
import System.Collections;

import ClearCanvas.Common;
import ClearCanvas.Common.Scripting;

[assembly: ClearCanvas.Common.Plugin()]

package ClearCanvas.Jscript
{
    /// This class provides an implementation of ClearCanvas.Common.Scripting.ScriptEngineExtensionPoint
    /// for JScript
    ///
    /// The context IDictionary is accessible to the script as "this.Context".  For example, if
    /// the dictionary contains "Name" => "JoJo", then from the script,
    ///
    ///     this.Context['Name'] => "JoJo"
    ///     or
    ///     this.Context.Name => "JoJo"     (the entries are directly accessible as properties of the Context object)
    /// 
    public 
    ClearCanvas.Common.ExtensionOf(ScriptEngineExtensionPoint)
    ClearCanvas.Common.Scripting.LanguageSupport("jscript")
    class Engine implements IScriptEngine
    {
        var Context: Object;
    
	    function Run(script: String, context: IDictionary)
	    {
	        this.Context = new Object();
	        
	        for(var entry in context)
	        {
	            this.Context[entry.Key] = entry.Value;
	        }
	        
		    return eval(script);
	    }
    }
}