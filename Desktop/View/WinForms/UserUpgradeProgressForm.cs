using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class UserUpgradeProgressForm : DotNetMagicForm
	{
		public UserUpgradeProgressForm(string title)
		{
			InitializeComponent();

			Text = title;
			_progressBar.Minimum = 0;
			_progressBar.Maximum = 100;
			_progressBar.Step = 1;
		}

		public string Message
		{
			get { return _message.Text; }
			set { _message.Text = value; }
		}

		public int ProgressPercent
		{
			get { return _progressBar.Value; }
			set { _progressBar.Value = value; }
		}
	}
}
