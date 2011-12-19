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
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct SHFILEINFO
	{
		public IntPtr hIcon;
		public int iIcon;
		public uint dwAttributes;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}
}

// ReSharper restore InconsistentNaming