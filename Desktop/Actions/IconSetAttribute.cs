#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a set of icon resources to associate with an action.  
    /// </summary>
    /// <remarks>
    /// <para>
	/// The icon resources must refer to different sized versions of the same icon.
	/// </para>
	/// <para>
    /// Icons should be supplied in several sizes so that different displays can be accomodated without
    /// having to scale the images:
    /// <list type="bullet">
    /// <item>small: 24 x 24</item>
    /// <item>medium: 48 x 48</item>
    /// <item>large: 64 x 64</item>
    /// </list>
    /// The attribute may appear more than once for a given action ID, in order to specify 
    /// a different scheme.
	/// </para>
    /// </remarks>
    public class IconSetAttribute : ActionDecoratorAttribute
    {
        private readonly IconSet _iconSet;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to which this attribute applies.</param>
        /// <param name="scheme">The scheme of this icon set.</param>
        /// <param name="smallIcon">The resource name of the small icon.</param>
        /// <param name="mediumIcon">The resource name of the medium icon.</param>
        /// <param name="largeIcon">The resource name of the large icon.</param>
        public IconSetAttribute(string actionID, IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
            : base(actionID)
        {
            _iconSet = new IconSet(scheme, smallIcon, mediumIcon, largeIcon);
        }

		///<summary>
		/// Attribute constructor.
		///</summary>
		/// <param name="actionID">The logical action identifier to which this attribute applies.</param>
		///<param name="icon">The resource name of the icon.</param>
		public IconSetAttribute(string actionID, string icon)
			: base(actionID)
		{
			_iconSet = new IconSet(icon);
		}

        /// <summary>
        /// The <see cref="IconSet"/> derived from this attribute.
        /// </summary>
        public IconSet IconSet { get { return _iconSet; } }

		/// <summary>
		/// Sets the icon set for an <see cref="IAction"/>, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
        public override void Apply(IActionBuildingContext builder)
        {
            // note that only one IconSet is currently supported, although it may be desirable to 
            // allow the IconSetAttribute to appear more than once to provide multiple icon schemes
            builder.Action.IconSet = this.IconSet;
        }
    }
}
