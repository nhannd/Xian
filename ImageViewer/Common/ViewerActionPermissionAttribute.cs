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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Associates authority tokens with a viewer-specific action.
	/// </summary>
	/// <remarks>
	/// <para>
	/// All tokens specified will be ANDed with <see cref="AuthorityTokens.General"/>, which is a global token
	/// intended to limit access to all viewer-specific functionality.
	/// </para>
	/// <para>
	/// This attribute sets the action permissibility via the <see cref="Action.SetPermissibility(ISpecification)"/> method.
	/// If multiple authority tokens are supplied in an array to a single instance of the attribute, those tokens will be combined using AND.  If
	/// multiple instances of this attribute are specified, the tokens associated with each instance are combined
	/// using OR logic.  This allows for the possibility of constructing a permission specification based on a complex boolean
	/// combination of authority tokens.
	/// </para>
	/// </remarks>
	public class ViewerActionPermissionAttribute : ActionPermissionAttribute
	{
		/// <summary>
		/// Constructor - the specified authority token will be associated with the specified action ID.
		/// </summary>
		public ViewerActionPermissionAttribute(string actionID, string authorityToken)
            : this(actionID, new string[] { authorityToken })
        {
        }

		/// <summary>
		/// Constructor - all of the specified tokens will combined using AND and associated with the specified action ID.
		/// </summary>
		public ViewerActionPermissionAttribute(string actionID, params string[] authorityTokens)
			: base(actionID, CreateViewerTokens(authorityTokens))
        {
        }

		private static string[] CreateViewerTokens(string[] authorityTokens)
		{
			authorityTokens = authorityTokens ?? new string[0];
			string[] viewerTokens = new string[authorityTokens.Length + 1];
			
			viewerTokens[0] = AuthorityTokens.General;
			
			for (int i = 0; i < authorityTokens.Length; ++i)
				viewerTokens[i + 1] = authorityTokens[i];

			return viewerTokens;
		}
	}
}
