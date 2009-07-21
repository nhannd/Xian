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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Helper class for determining whether or not the current user has certain viewer-specific permissions.
	/// </summary>
	/// <remarks>
	/// Regardless of what is input, each of the methods in this class will unconditionally check for
	/// the <see cref="AuthorityTokens.ViewerVisible"/> token, which is a global token intended to limit
	/// access to all viewer components.
	/// </remarks>
	public static class PermissionsHelper
	{
		/// <summary>
		/// Checks whether the current user has the correct permissions based on the provided authority token.
		/// </summary>
		public static bool IsInRole(string authorityToken)
		{
			return IsInRoles(authorityToken);
		}

		/// <summary>
		/// Checks whether the current user has the correct permissions based on the provided authority tokens.
		/// </summary>
		public static bool IsInRoles(params string[] authorityTokens)
		{
			return IsInRoles((IEnumerable<string>) (authorityTokens ?? new string[0]));
		}

		/// <summary>
		/// Checks whether the current user has the correct permissions based on the provided authority tokens.
		/// </summary>
		public static bool IsInRoles(IEnumerable<string> authorityTokens)
		{
			if (Thread.CurrentPrincipal == null || !Thread.CurrentPrincipal.Identity.IsAuthenticated)
				return true;

			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.ViewerVisible))
				return false;

			authorityTokens = authorityTokens ?? new string[0];

			foreach (string authorityToken in authorityTokens)
			{
				if (!Thread.CurrentPrincipal.IsInRole(authorityToken))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Checks whether the current user is in at least one of the specified roles/authority tokens.
		/// </summary>
		public static bool IsInAnyRole(params string[] authorityTokens)
		{
			return IsInAnyRole((IEnumerable<string>)(authorityTokens ?? new string[0]));
		}

		/// <summary>
		/// Checks whether the current user is in at least one of the specified roles/authority tokens.
		/// </summary>
		public static bool IsInAnyRole(IEnumerable<string> authorityTokens)
		{
			if (Thread.CurrentPrincipal == null || !Thread.CurrentPrincipal.Identity.IsAuthenticated)
				return true;

			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.ViewerVisible))
				return false; //doesn't matter which roles they're in.

			authorityTokens = authorityTokens ?? new string[0];

			foreach (string authorityToken in authorityTokens)
			{
				if (Thread.CurrentPrincipal.IsInRole(authorityToken))
					return true;
			}

			return false;
		}
	}
}
