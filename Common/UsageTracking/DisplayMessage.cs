using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Display message for client.
    /// </summary>
    public class DisplayMessage
    {
        /// <summary>
        /// The title of the message to be displayed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Message { get; set; }
    }
}
