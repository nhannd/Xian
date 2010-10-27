#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer.DesktopServices
{
	public class DesktopServiceHostPermissionAttribute : Attribute
	{
		public ReadOnlyCollection<string> AuthorityTokens;
		
		public DesktopServiceHostPermissionAttribute(params string[] authorityTokens)
		{
			List<string> viewerTokens = new List<string>();
			viewerTokens.Add(ImageViewer.AuthorityTokens.ViewerVisible);
			viewerTokens.AddRange(authorityTokens);
			AuthorityTokens = viewerTokens.AsReadOnly();
		}
	}
}
