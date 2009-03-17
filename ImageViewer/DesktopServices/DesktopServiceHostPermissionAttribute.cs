using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer.DesktopServices
{
	public class DesktopServiceHostPermissionAttribute : Attribute
	{
		public ReadOnlyCollection<string> AuthorityTokens;
		
		public DesktopServiceHostPermissionAttribute(string[] authorityTokens)
		{
			AuthorityTokens = new ReadOnlyCollection<string>(authorityTokens);
		}
	}
}
