using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Represents an error that occured in parsing command line arguments.
    /// </summary>
    public class CommandLineException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public CommandLineException(string message)
            :base(message)
        {
        }
    }
}
