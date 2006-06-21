using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Declares a set of icon resources to associate with an action.  The icon resources must
    /// refer to different sized versions of the same icon.
    /// </summary>
    /// <remarks>
    /// Icons should be supplied in several sizes so that different displays can be accomodated without
    /// having to scale the images:
    /// <list type="bullet">
    /// <item>small: 16 x 16</item>
    /// <item>medium: 32 x 32</item>
    /// <item>large: 64 x 64</item>
    /// </list>
    /// The attribute may appear more than once for a given action ID, in order to specify 
    /// a different scheme.
    /// </remarks>
    public class IconSetAttribute : ActionDecoratorAttribute
    {
        private IconSet _iconSet;

        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to which this attribute applies</param>
        /// <param name="scheme">The scheme of this icon set</param>
        /// <param name="smallIcon">The resource name of the small icon</param>
        /// <param name="mediumIcon">The resource name of the medium icon</param>
        /// <param name="largeIcon">The resource name of the large icon</param>
        public IconSetAttribute(string actionID, IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
            : base(actionID)
        {
            _iconSet = new IconSet(scheme, smallIcon, mediumIcon, largeIcon);
        }

        /// <summary>
        /// The <see cref="IconSet"/> derived from this attribute.
        /// </summary>
        public IconSet IconSet { get { return _iconSet; } }

        internal override void Apply(IActionBuilder builder)
        {
            builder.Apply(this);
        }
    }
}
