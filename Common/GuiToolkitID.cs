#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common
{
    /// <summary>
    /// Defines a set of identifiers representing different GUI toolkits.
    /// </summary>
    public class GuiToolkitID
    {
        /// <summary>
        /// WinForms Gui Toolkit.
        /// </summary>
		public const string WinForms = "WinForms";

		/// <summary>
		/// GTK (unix based platforms) Gui Toolkit.
		/// </summary>
		public const string GTK = "GTK";

        /// <summary>
        /// Generic Web Gui Toolkit.
        /// </summary>
        public const string Web = "Web";

    }
}
