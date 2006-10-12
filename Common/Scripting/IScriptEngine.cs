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
        /// It is left up to the implementation of the engine to decide how these objects are made available to the script.
        /// </summary>
        /// <param name="script">The script to run</param>
        /// <param name="context">A set of objects which the script can access</param>
        /// <returns>The return value of the script</returns>
        object Run(string script, IDictionary context);

        /// <summary>
        /// Asks the script engine to create an instance of a <see cref="IExecutableScript"/> object based on the 
        /// specified string.  This may offer better performance than calling <see cref="IScriptEngine.Run"/> in the case
        /// where the same script is to be run multiple times, as the script engine may be able to compile portions of 
        /// the script.  This is entirely dependent on the implementation of the script engine, and there are no guarantees
        /// of improved performance.
        /// </summary>
        /// <param name="script">The script to create</param>
        /// <returns>A script object that can be run multiple times</returns>
        IExecutableScript CreateScript(string script);
    }
}
