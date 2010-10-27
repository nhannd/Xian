#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class StandardIconSizes
	{
		public static readonly Size Small = new Size(24, 24);
		public static readonly Size Medium = new Size(32, 32);
		public static readonly Size Large = new Size(48, 48);

		public static Size GetSize(IconSize iconSize)
		{
			if (iconSize == IconSize.Small)
				return Small;
			if (iconSize == IconSize.Medium)
				return Medium;
			return Large;
		}
	}
}
