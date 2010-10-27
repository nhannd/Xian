#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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