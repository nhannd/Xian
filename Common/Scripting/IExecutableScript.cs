using System;
using System.Collections;
using System.Text;

namespace ClearCanvas.Common.Scripting
{
    /// <summary>
    /// Defines the interface to an executable script returned by an instance of an <see cref="IScriptEngine"/>.
    /// </summary>
    public interface IExecutableScript
    {
        /// <summary>
        /// Executes this script, using the supplied values to initialize any variables in the script.
        /// </summary>
        /// <param name="context">The set of values to substitute into the script.</param>
        /// <returns>The return value of the script.</returns>
        object Run(IDictionary context);
    }
}
