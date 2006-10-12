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
    /// Factory for creating instances of script engines that support a given language  
    /// </summary>
    public static class ScriptEngineFactory
    {
        /// <summary>
        /// Attempts to instantiate a script engine for the specified language. Internally,
        /// this class looks for an extension of <see cref="ScriptEngineExtensionPoint"/> that is capable of running scripts in the specified
        /// language.  In theory, any scripting language is supported, as long as a script engine extension exists for that language.
        /// </summary>
        /// <param name="language">The script language - case insensitive, so jscript is equivalent to JScript</param>
        public static IScriptEngine CreateEngine(string language)
        {
            try
            {
                ScriptEngineExtensionPoint xp = new ScriptEngineExtensionPoint();
                return (IScriptEngine)xp.CreateExtension(
                    new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(string.Format("No script engine available for language {0}", language), e);
            }
        }
    }
}
