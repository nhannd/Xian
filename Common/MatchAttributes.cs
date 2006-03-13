using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Implements an extension filter that performs matching on attributes.
    /// </summary>
    /// <remarks>
    /// For each attribute that is supplied to the constructor of this filter, the filter
    /// will check if the extension is marked with an attribute that is equal.  That is, the extension
    /// must have at least one attribute of the same type, and that attribute must be equal to the test
    /// attribute, where equality is tested with the <see cref="Object.Equals"/> method.  This allows
    /// for quite sophisticated matching capabilities, since attributes are defined as classes, and hence
    /// equality of attributes is defined by the attribute class.
    /// </remarks>
    public class MatchAttributes : ExtensionFilter
    {
        private Attribute[] _attributes;

        /// <summary>
        /// Creates a filter to match on multiple attributes.  The extension must test true on each attribute.
        /// </summary>
        /// <param name="attributes">The attributes to be used as test criteria</param>
        public MatchAttributes(Attribute[] attributes)
        {
            _attributes = attributes;
        }

        /// <summary>
        /// Creates a filter to match on a single attribute.
        /// </summary>
        /// <param name="attribute">The attribute to be used as test criteria</param>
        public MatchAttributes(Attribute attribute)
            :this(new Attribute[] { attribute })
        {
        }

        /// <summary>
        /// Checks whether the specified <see cref="Extension"/> is marked with attributes that 
        /// test equal to every test attribute supplied as criteria to this filter.
        /// </summary>
        /// <param name="extension">The extension to test</param>
        /// <returns>true if the test succeeds</returns>
        public override bool Test(Extension extension)
        {
            foreach (Attribute a in _attributes)
            {
                object[] candidates = extension.ExtensionType.GetCustomAttributes(a.GetType(), true);
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
                if (c.Equals(a))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
