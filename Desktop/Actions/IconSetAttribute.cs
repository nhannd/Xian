#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
