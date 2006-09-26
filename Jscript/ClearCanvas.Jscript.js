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
    /// The context IDictionary is accessible to the script as "this".  For example, if
    /// the dictionary contains "Name" => "JoJo", then from the script,
    ///
    ///     this['Name'] => "JoJo"
    ///     or
    ///     this.Name => "JoJo"     (the entries are directly accessible as properties of this)
    ///
    /// The script must use the "return" keyword to return an object to the caller. This is because
    /// the Run(...) method packages the script into its own function before executing it.
    public 
    ClearCanvas.Common.ExtensionOf(ScriptEngineExtensionPoint)
    ClearCanvas.Common.Scripting.LanguageSupport("jscript")
    class Engine implements IScriptEngine
    {
	    function Run(script: String, context: IDictionary)
	    {
	        var ctx = new Object();
	        
	        for(var entry in context)
	        {
	            ctx[entry.Key] = entry.Value;
	        }
	        
	        ctx.__scriptFunction__ = new Function(script);
	        return ctx.__scriptFunction__();
	    }
    }
}