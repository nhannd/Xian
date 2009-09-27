using System;

namespace ClearCanvas.Controls.WinForms
{
	partial class Native
	{
		[Flags]
		public enum STRRET : uint
		{
			STRRET_WSTR = 0,
			STRRET_OFFSET = 0x1,
			STRRET_CSTR = 0x2,
		}
	}
}