#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Utility class for working with <see cref="DialogSizeHint"/>.
	/// </summary>
	public static class SizeHintHelper
	{
		/// <summary>
		/// Translates the specified size hint into an absolute size.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="defaultSize"></param>
		/// <returns></returns>
		public static Size TranslateHint(DialogSizeHint hint, Size defaultSize)
		{
			switch (hint)
			{
				case DialogSizeHint.Small:
					return new Size(320, 240);
				case DialogSizeHint.Medium:
					return new Size(640, 480);
				case DialogSizeHint.Large:
					return new Size(800, 600);
				default:
					return defaultSize;
			}
		}
	}
}
