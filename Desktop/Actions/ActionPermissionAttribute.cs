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

using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Associates authority tokens with an action.
    /// </summary>
    /// <remarks>
    /// This attribute sets the action permissibility via the <see cref="Action.SetPermissibility(ISpecification)"/> method.
    /// If multiple authority tokens are supplied in an array to a single instance of the attribute, those tokens will be combined using AND.  If
    /// multiple instances of this attribute are specified, the tokens associated with each instance are combined
    /// using OR logic.  This allows for the possibility of constructing a permission specification based on a complex boolean
    /// combination of authority tokens.
    /// </remarks>
    public class ActionPermissionAttribute : ActionDecoratorAttribute
    {
        private string[] _authorityTokens;

        /// <summary>
        /// Constructor - the specified authority token will be associated with the specified action ID.
        /// </summary>
        public ActionPermissionAttribute(string actionID, string authorityToken)
            : this(actionID, new string[] { authorityToken })
        {
        }

        /// <summary>
        /// Constructor - all of the specified tokens will combined using AND and associated with the specified action ID.
        /// </summary>
        public ActionPermissionAttribute(string actionID, params string[] authorityTokens)
            :base(actionID)
        {
            _authorityTokens = authorityTokens;
        }

		/// <summary>
		/// Applies permissions represented by this attribute to an action instance, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
        {
            // if this is the first occurence of this attribute, create the parent spec
            if (builder.Action.PermissionSpecification == null)
                builder.Action.PermissionSpecification = new OrSpecification();

            // combine the specified tokens with AND logic
            AndSpecification and = new AndSpecification();
            foreach (string token in _authorityTokens)
            {
                and.Add(new PrincipalPermissionSpecification(token));
            }

            // combine this spec with any previous occurence of this attribute using OR logic
            ((OrSpecification)builder.Action.PermissionSpecification).Add(and);
        }
    }
}
