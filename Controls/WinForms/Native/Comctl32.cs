using System;
using System.Runtime.InteropServices;

namespace ClearCanvas.Controls.WinForms
{
	partial class Native
	{
		public static class Comctl32
		{
			[DllImport("comctl32.dll")]
			[return : MarshalAs(UnmanagedType.Bool)]
			public static extern bool ImageList_Destroy(IntPtr hImageList);
		}
	}
}