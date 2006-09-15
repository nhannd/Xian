using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
	public class NonEmptyNumericUpDown: NumericUpDown
	{
		protected override void UpdateEditText()
		{
			base.UpdateEditText();

			if (String.IsNullOrEmpty(Text) && !String.IsNullOrEmpty(Minimum.ToString()))
				Text = Minimum.ToString();
		}
	}
}
