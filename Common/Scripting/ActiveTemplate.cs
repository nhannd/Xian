using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClearCanvas.Common.Scripting
{
    /// <summary>
    /// Represents an instance of an active template.  An active template is equivalent to a classic ASP page: that is,
    /// it is a template that contains snippets of script code that can callback into the context in which the script
    /// is being evaluated.  Currently only the Jscript language is supported.
    /// 
    /// Initialize the template context via the constructor.  The template
    /// can then be evaluated within a given context by calling the <see cref="Template.Evaluate"/> method.
    /// </summary>
    public class ActiveTemplate
    {
        private string _inversion;
        private ScriptHost _scriptHost;

        /// <summary>
        /// Constructs a template from the specified content.
        /// </summary>
        /// <param name="content"></param>
        public ActiveTemplate(TextReader content)
        {
            _inversion = ComputeInversion(content);
        }

        /// <summary>
        /// Overload that allows the output of the template evaluation to be written directly to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="context">A dictionary of objects to pass into the script</param>
        /// <param name="output">A text writer to which the output should be written</param>
        public void Evaluate(IDictionary context, TextWriter output)
        {
            try
            {
                if (_scriptHost == null)
                {
                    _scriptHost = new ScriptHost("jscript");
                }

                context["__out__"] = output;
                _scriptHost.Run(_inversion, context);
            }
            catch (Exception e)
            {
                throw new ActiveTemplateException("Template evaluation generated an exception.", e);
            }
        }

        /// <summary>
        /// Evaluates this template in the specified context.  The context parameter allows a set of
        /// named objects to be passed into the scripting environment.  Within the scripting environment
        /// these objects can be referenced directly as properties of "this".  For example,
        /// <code>
        ///     Hashtable scriptingContext = new Hashtable();
        ///     scriptingContext["Patient"] = patient;  // add a reference to an existing instance of a patient object
        /// 
        ///     Template template = new Template(...);
        ///     template.Evaluate(scriptingContext);
        /// 
        ///     // now, in the template, the script would access the object as shown
        ///     &lt;%= this.Patient.Name %&gt;
        ///     
        /// </code>
        /// </summary>
        /// <param name="context">A dictionary of objects to pass into the script</param>
        /// <returns>The result of the template evaluation as a string</returns>
        public string Evaluate(IDictionary context)
        {
            StringWriter output = new StringWriter();
            Evaluate(context, output);
            return output.ToString();
        }

        /// <summary>
        /// Inverts the template content, returning a Jscript script that, when evaluated, will return
        /// the full result of the template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private string ComputeInversion(TextReader template)
        {
            StringBuilder inversion = new StringBuilder();
            string line = null;
            bool inCode = false;    // keep track of whether we are inside a <% %> or not

            // process each line of the template
            while ((line = template.ReadLine()) != null)
            {
                inCode = ProcessLine(line, inCode, inversion);
                
                // preserve the formatting of the original template by writing new lines appropriately
                if(!inCode)
                    inversion.AppendLine("this.__out__.WriteLine();");
            }

            return inversion.ToString();
        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inCode"></param>
        /// <param name="inversion"></param>
        /// <returns></returns>
        private bool ProcessLine(string line, bool inCode, StringBuilder inversion)
        {
            inCode = !inCode;   // just make the loop work correctly

            // break the line up into code/non-code parts
            string[] parts = line.Split(new string[] { "<%", "%>" }, StringSplitOptions.None);
            foreach (string part in parts)
            {
                inCode = !inCode;
                if (inCode)
                {
                    if (part.StartsWith("="))
                    {
                        inversion.AppendLine(string.Format("this.__out__.Write({0});", part.Substring(1)));
                    }
                    else
                    {
                        inversion.Append(part);
                        inversion.AppendLine();
                    }
                }
                else
                {
                    string escaped = part.Replace("\"", "\\\"");  // escape any " characters
                    inversion.AppendLine(string.Format("this.__out__.Write(\"{0}\");", escaped));
                }
            }
            return inCode;
        }
    }
}
