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
	/// An extension filter that checks for equality with the extension class name.
	/// </summary>
    public class ClassNameExtensionFilter : ExtensionFilter
    {
        private string _name;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The extension class name that will be a match for this filter.</param>
        public ClassNameExtensionFilter(string name)
        {
            _name = name;
        }

		/// <summary>
		/// Tests whether or not the input <see cref="ExtensionInfo.ExtensionClass"/>' full name matches 
		/// the name supplied to the filter constructor.
		/// </summary>
        public override bool Test(ExtensionInfo extension)
        {
            return extension.ExtensionClass.FullName.Equals(_name);
        }
    }
}
