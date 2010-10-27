#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    /// Implements an extension filter that performs matching on attributes.
    /// </summary>
    /// <remarks>
    /// For each attribute that is supplied to the constructor of this filter, the filter
    /// will check if the extension is marked with at least one matching attribute.  A matching attribute is an
    /// attribute for which the <see cref="Attribute.Match"/> method returns true.  This allows
    /// for quite sophisticated matching capabilities, as the attribute itself decides what constitutes
    /// a match.
    /// </remarks>
    public class AttributeExtensionFilter : ExtensionFilter
    {
        private Attribute[] _attributes;

        /// <summary>
        /// Creates a filter to match on multiple attributes.
        /// </summary>
        /// <remarks>
		/// The extension must test true on each attribute.
		/// </remarks>
        /// <param name="attributes">The attributes to be used as test criteria.</param>
        public AttributeExtensionFilter(Attribute[] attributes)
        {
            _attributes = attributes;
        }

        /// <summary>
        /// Creates a filter to match on a single attribute.
        /// </summary>
        /// <param name="attribute">The attribute to be used as test criteria.</param>
        public AttributeExtensionFilter(Attribute attribute)
            :this(new Attribute[] { attribute })
        {
        }

        /// <summary>
        /// Checks whether the specified extension is marked with attributes that 
        /// match every test attribute supplied as criteria to this filter.
        /// </summary>
        /// <param name="extension">The information about the extension to test.</param>
        /// <returns>true if the test succeeds.</returns>
        public override bool Test(ExtensionInfo extension)
        {
            foreach (Attribute a in _attributes)
            {
                object[] candidates = extension.ExtensionClass.GetCustomAttributes(a.GetType(), true);
                if (!AnyMatch(a, candidates))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AnyMatch(Attribute a, object[] candidates)
        {
            foreach (Attribute c in candidates)
            {
                if (c.Match(a))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
