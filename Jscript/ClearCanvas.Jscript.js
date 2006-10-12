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
	        return CreateScript(script).Run(context);
	    }
	    
        function CreateScript(script: String) : IExecutableScript
        {
 	        var ctx = new Object();
	        ctx.__scriptFunction__ = new Function(script);
	        return new ExecutableScript(ctx);
        }
    }
    
    internal class ExecutableScript implements IExecutableScript
    {
        private var _ctx;
        
        // constructor 
        function ExecutableScript(ctx)
        {
            _ctx = ctx;
        }
        
        function Run(context: IDictionary)
        {
 	        for(var entry in context)
	        {
	            _ctx[entry.Key] = entry.Value;
	        }
	        return _ctx.__scriptFunction__();
        }
    }
}