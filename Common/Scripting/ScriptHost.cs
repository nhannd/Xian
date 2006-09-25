using System;
using System.Text;
using System.Collections;

namespace ClearCanvas.Common.Scripting
{
    /// <summary>
    /// Extension point that defines a script engine for a given language
    /// </summary>
    [ExtensionPoint]
    public class ScriptEngineExtensionPoint : ExtensionPoint<IScriptEngine>
    {
    }

    /// <summary>
    /// This class allows scripts to be executed and to refer to objects within the calling process.  Internally,
    /// this class looks for an extension of <see cref="ScriptEngineExtensionPoint"/> that is capable of running scripts in the specified
    /// language.  In theory, any scripting language is supported, as long as a script engine extension exists for that language.
    /// </summary>
    public class ScriptHost
    {
        private IScriptEngine _engine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">The script language - case insensitive, so jscript is equivalent to JScript</param>
        public ScriptHost(string language)
        {
            try
            {
                ScriptEngineExtensionPoint xp = new ScriptEngineExtensionPoint();
                _engine = (IScriptEngine)xp.CreateExtension(
                    new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(string.Format("No script engine available for language {0}", language), e);
            }
        }

        /// <summary>
        /// Executes the specified script in the specified context.  The context is simply a dictionary of named
        /// objects to be injected into the scripting environment.  How the object are accessed from within the script
        /// is left up to the implementation of the <see cref="IScriptEngine"/> that will process the script, and may
        /// therefore depend on the scripting language used.  Note that the script may only return a single object, which
        /// is returned as the return value of this method.
        /// </summary>
        /// <param name="script">The script to execute</param>
        /// <param name="context">A set of objects to be made accessible to the script</param>
        /// <returns>The object returned by the script</returns>
        public object Run(string script, IDictionary context)
        {
            return _engine.Run(script, context);
        }
    }
}
