using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	internal interface IClickableHostedControl
	{
		event EventHandler ResetDropDownFocusRequested;
		event EventHandler CloseDropDownRequested;
	}

	internal sealed class ClickableToolStripControlHost : ToolStripControlHost
	{
		public ClickableToolStripControlHost(Control hostedControl) : base(hostedControl) {}
		public ClickableToolStripControlHost(Control hostedControl, string name) : base(hostedControl, name) {}

		protected override bool DismissWhenClicked
		{
			get { return false; }
		}

		protected override void OnSubscribeControlEvents(Control control)
		{
			base.OnSubscribeControlEvents(control);
			if (control is IClickableHostedControl)
			{
				((IClickableHostedControl) control).ResetDropDownFocusRequested += Control_ResetDropDownFocusRequested;
				((IClickableHostedControl) control).CloseDropDownRequested += Control_CloseDropDownRequested;
			}
		}

		protected override void OnUnsubscribeControlEvents(Control control)
		{
			if (control is IClickableHostedControl)
			{
				((IClickableHostedControl) control).CloseDropDownRequested -= Control_CloseDropDownRequested;
				((IClickableHostedControl) control).ResetDropDownFocusRequested -= Control_ResetDropDownFocusRequested;
			}
			base.OnUnsubscribeControlEvents(control);
		}

		private void Control_ResetDropDownFocusRequested(object sender, EventArgs e)
		{
			if (base.IsOnDropDown && base.Parent != null)
				base.Parent.Focus();
		}

		private void Control_CloseDropDownRequested(object sender, EventArgs e)
		{
			if (base.IsOnDropDown && base.Parent is ToolStripDropDown)
				((ToolStripDropDown) base.Parent).Close(ToolStripDropDownCloseReason.ItemClicked);
		}
	}
}