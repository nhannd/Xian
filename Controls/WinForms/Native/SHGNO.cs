using System;

namespace ClearCanvas.Controls.WinForms
{
	partial class Native
	{
		[Flags]
		public enum SHGNO : uint
		{
			SHGDN_NORMAL = 0x0000, // Default (display purpose)
			SHGDN_INFOLDER = 0x0001, // Displayed under a folder (relative)
			SHGDN_FOREDITING = 0x1000, // For in-place editing
			SHGDN_FORADDRESSBAR = 0x4000, // UI friendly parsing name (remove ugly stuff)
			SHGDN_FORPARSING = 0x8000, // Parsing name for ParseDisplayName()
		}
	}
}