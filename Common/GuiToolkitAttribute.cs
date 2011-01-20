#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark a class as using a specific GUI toolkit.
    /// </summary>
    /// <remarks>
	/// Typically this attribute is used on an extension class (in addition to the <see cref="ExtensionOfAttribute"/>) 
	/// to allow plugin code to determine at runtime if the given extension is compatible with the GUI toolkit
	/// that is currently in use by the main window.
	/// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class GuiToolkitAttribute : Attribute
    {
        private string _toolkitID;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="toolkitID">A string identifier for the Gui Toolkit.</param>
		public GuiToolkitAttribute(string toolkitID)
        {
            _toolkitID = toolkitID;
        }

		/// <summary>
		/// Gets the Gui Toolkit ID.
		/// </summary>
        public string ToolkitID
        {
            get { return _toolkitID; }
        }

		/// <summary>
		/// Determines whether or not this attribute is a match for (or is the same as) <paramref name="obj"/>,
		/// which is itself an <see cref="Attribute"/>.
		/// </summary>
        public override bool Match(object obj)
        {
            if (obj != null && obj is GuiToolkitAttribute)
            {
                return (obj as GuiToolkitAttribute).ToolkitID == this.ToolkitID;
            }
            else
            {
                return false;
            }
        }
    }
}
