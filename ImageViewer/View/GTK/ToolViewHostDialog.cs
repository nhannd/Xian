using System;
using Gtk;
using ClearCanvas.Workstation.Model.Tools;

namespace ClearCanvas.Workstation.View.GTK
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
