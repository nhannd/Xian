#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// A display message related to a ClearCanvas application.
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
