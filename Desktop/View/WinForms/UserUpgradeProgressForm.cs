using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class UserUpgradeProgressForm : DotNetMagicForm
	{
		public UserUpgradeProgressForm(int totalSteps)
		{
			InitializeComponent();

			Text = SR.TitleUpdatingPreferences;
			_progressBar.Minimum = 0;
			_progressBar.Maximum = totalSteps;
			_progressBar.Step = 1;
		}

		public string Message
		{
			get { return _message.Text; }
			set { _message.Text = value; }
		}

		public int Progress
		{
			get { return _progressBar.Value; }
			set { _progressBar.Value = value; }
		}
	}
}
