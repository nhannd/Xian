using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    ///<summary>
    ///Exception that indicates a problem with the way action attributes are applied to a tool.
    ///</summary>
    public class ActionBuilderException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public ActionBuilderException(string message)
            : base(message)
        {
        }
    }
}
