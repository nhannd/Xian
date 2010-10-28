#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Common;

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
