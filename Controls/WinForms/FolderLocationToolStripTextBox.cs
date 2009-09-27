using System;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
	public class FolderLocationToolStripTextBox : ToolStripTextBox
	{
		private event EventHandler _keyEnterPressed;

		public FolderLocationToolStripTextBox() : base() {}

		public FolderLocationToolStripTextBox(Control c)
			: base(c) {}

		public FolderLocationToolStripTextBox(string name)
			: base(name) {}

		public event EventHandler KeyEnterPressed
		{
			add { _keyEnterPressed += value; }
			remove { _keyEnterPressed -= value; }
		}

		protected virtual void OnKeyEnterPressed(EventArgs e)
		{
			if (_keyEnterPressed != null)
				_keyEnterPressed.Invoke(this, e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
				return true;
			return base.IsInputKey(keyData);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.OnKeyEnterPressed(EventArgs.Empty);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			base.OnKeyDown(e);
		}

		protected override void OnSubscribeControlEvents(Control control)
		{
			base.OnSubscribeControlEvents(control);
			control.GotFocus += OnHostedControlGotFocus;
		}

		protected override void OnUnsubscribeControlEvents(Control control)
		{
			control.GotFocus -= OnHostedControlGotFocus;
			base.OnUnsubscribeControlEvents(control);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.SelectAll();
		}

		private void OnHostedControlGotFocus(object sender, EventArgs e)
		{
			this.SelectAll();
		}
	}
}