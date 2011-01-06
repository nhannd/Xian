#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using Gtk;
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop.Tools;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	public class ToolViewHostDialog : Dialog
	{
		private ToolViewProxy _view;
		
		public ToolViewHostDialog(ToolViewProxy view, Window parent)
			:base(view.Title, parent, Gtk.DialogFlags.DestroyWithParent)
		{
			_view = view;
			this.VBox.PackStart((Widget)_view.View.GuiElement, false, false, 0);
		}
		
		
		protected override bool OnDeleteEvent(Gdk.Event e)
		{
			_view.Active = false;
			this.Hide();
			return true;	// don't destroy the dialog
		}
	}
}
