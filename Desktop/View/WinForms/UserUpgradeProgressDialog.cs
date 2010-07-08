using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common.Utilities;
using System.Drawing;
using ClearCanvas.Common;
using System.Windows.Forms;
using System;
using System.Threading;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(UserUpgradeProgressDialogExtensionPoint))]
	internal class UserUpgradeProgressDialog : IUserUpgradeProgressDialog
	{
		private SynchronizationContext _context;
		private IUserUpgradeStrategy _strategy;
		private UserUpgradeProgressForm _form;

		#region IUserUpgradeProgressDialog Members

		public void Show(IUserUpgradeStrategy strategy)
		{
			_strategy = strategy;
			_context = SynchronizationContext.Current;
			if (_context == null || _strategy.TotalSteps < 5)
			{
				_strategy.Run();
				return;
			}

			System.Windows.Forms.Application.EnableVisualStyles();

			Thread thread = new Thread(RunThread);
			thread.Start();

			_form = new UserUpgradeProgressForm(_strategy.TotalSteps);
			_form.Message = SR.MessageUpdatingPreferences;
			_form.ShowDialog();

			if (_strategy.FailedCount > 0)
			{
				MessageBox box = new MessageBox();
				box.Show(SR.MessageUserUpgradeFailures);
			}
		}

		private void UpdateProgress(int step)
		{
			_context.Post(ignored => _form.Progress = step, null);
		}

		private void Complete()
		{
			_context.Post(ignored => _form.Close(), null);
		}

		public void RunThread()
		{
			_strategy.ProgressChanged += delegate { UpdateProgress(_strategy.CurrentStep); };
			_strategy.Run();
			Complete();
		}

		#endregion
	}
}
