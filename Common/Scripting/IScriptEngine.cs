using System;
using System.Text;
using System.Collections;

namespace ClearCanvas.Common.Scripting
{
    /// <summary>
    /// Defines the interface to a script engine.  
    /// </summary>
    public interface IScriptEngine
    {
        /// <summary>
        /// Asks the script engine to run the specified script given the specified context information.
        /// The context is a dictionary of named objects that the engine must make available to the script.
        /// It is left up to the implementation of the engine to decide how these objects are made available.
        /// </summary>
        /// <param name="script">The script to run</param>
        /// <param name="context">A set of objects which the script can access</param>
        /// <returns>The return value of the script</returns>
        object Run(string script, IDictionary context);
    }
}
