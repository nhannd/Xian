#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Controls.WinForms
{
	internal partial class Native
	{
		public static class ListView
		{
			public const int LVS_SMALLICON = 0x0002;
			public const int LVS_SHAREIMAGELISTS = 0x0040;
			public const int LVM_SETIMAGELIST = 0x1003;

			public const int LVSIL_NORMAL = 0;
			public const int LVSIL_SMALL = 1;
			public const int LVSIL_STATE = 2;
		}
	}
}