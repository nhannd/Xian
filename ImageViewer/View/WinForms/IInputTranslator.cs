using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public interface IInputTranslator
	{
		IInputMessage OnMouseMove(MouseEventArgs e);

		IInputMessage OnMouseDown(MouseEventArgs e);

		IInputMessage OnMouseUp(MouseEventArgs e);

		IInputMessage OnMouseWheel(MouseEventArgs e);

		IInputMessage OnKeyDown(KeyEventArgs e);

		IInputMessage OnKeyUp(KeyEventArgs e);

		//void PreProcessKeyStrokes(ref Message m);
	}
}
