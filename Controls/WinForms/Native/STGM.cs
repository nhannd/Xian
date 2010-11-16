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
	internal enum STGM
	{
		DIRECT = 0x00000000,
		TRANSACTED = 0x00010000,
		SIMPLE = 0x08000000,
		READ = 0x00000000,
		WRITE = 0x00000001,
		READWRITE = 0x00000002,
		SHARE_DENY_NONE = 0x00000040,
		SHARE_DENY_READ = 0x00000030,
		SHARE_DENY_WRITE = 0x00000020,
		SHARE_EXCLUSIVE = 0x00000010,
		PRIORITY = 0x00040000,
		DELETEONRELEASE = 0x04000000,
		NOSCRATCH = 0x00100000,
		CREATE = 0x00001000,
		CONVERT = 0x00020000,
		FAILIFTHERE = 0x00000000,
		NOSNAPSHOT = 0x00200000,
		DIRECT_SWMR = 0x00400000,
	}
}

// ReSharper restore InconsistentNaming