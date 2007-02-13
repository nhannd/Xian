using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Controls.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a way for application level code to use a wait cursor.
    /// </summary>
    [ExtensionOf(typeof(BlockingOperationViewExtensionPoint))]
	public class BlockingOperationView : WinFormsView, IBlockingOperationView
    {
		#region IBlockingOperationView Members

		public void Run(BlockingOperationDelegate operation)
		{
			Cursor previousCursor = Cursor.Current;

			try
			{
				Cursor.Current = (Cursor)GuiElement;

				operation();
			}
			catch
			{
				throw;
			}
			finally
			{
				Cursor.Current = previousCursor;
			}
		}

		#endregion

		public override object GuiElement
		{
			get { return System.Windows.Forms.Cursors.WaitCursor; }
		}
	}
}
