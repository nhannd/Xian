#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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