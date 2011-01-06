#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class NonEmptyNumericUpDown: NumericUpDown
	{
		// To keep in line with the control's default behaviour,
		// verify the value when the control loses focus and change
		// it if necessary.
		protected override void UpdateEditText()
		{
			base.UpdateEditText();

			if (String.IsNullOrEmpty(Text) && !String.IsNullOrEmpty(Minimum.ToString()))
				Text = Minimum.ToString();
		}
	}
}
