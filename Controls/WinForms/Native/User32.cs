#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// ReSharper disable InconsistentNaming

using System;
using System.Runtime.InteropServices;

namespace ClearCanvas.Controls.WinForms.Native
{
	internal static class User32
	{
		[DllImport("user32.dll")]
		public static extern Int32 SendMessage(IntPtr pWnd, UInt32 uMsg, UInt32 wParam, IntPtr lParam);
	}
}

// ReSharper restore InconsistentNaming