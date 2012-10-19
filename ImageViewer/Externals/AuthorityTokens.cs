#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Externals
{
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the External Applications feature.")]
		public const string Externals = "Viewer/Externals";
	}
}