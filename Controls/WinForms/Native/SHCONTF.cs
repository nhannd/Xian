#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// ReSharper disable InconsistentNaming

using System;

namespace ClearCanvas.Controls.WinForms.Native
{
	[Flags]
	internal enum SHCONTF : uint
	{
		SHCONTF_FOLDERS = 0x0020, // Only want folders enumerated (SFGAO_FOLDER)
		SHCONTF_NONFOLDERS = 0x0040, // Include non folders
		SHCONTF_INCLUDEHIDDEN = 0x0080, // Show items normally hidden
		SHCONTF_INIT_ON_FIRST_NEXT = 0x0100, // Allow EnumObject() to return before validating enum
		SHCONTF_NETPRINTERSRCH = 0x0200, // Hint that client is looking for printers
		SHCONTF_SHAREABLE = 0x0400, // Hint that client is looking sharable resources (remote shares)
		SHCONTF_STORAGE = 0x0800, // Include all items with accessible storage and their ancestors
	}
}

// ReSharper restore InconsistentNaming