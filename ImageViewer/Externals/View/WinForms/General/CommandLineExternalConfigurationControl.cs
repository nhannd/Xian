using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Externals.General;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms.General
{
	public partial class CommandLineExternalConfigurationControl : UserControl
	{
		public CommandLineExternalConfigurationControl(CommandLineExternal launcher)
		{
			InitializeComponent();

			_txtCommand.DataBindings.Add("Text", launcher, "Command", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtWorkingDir.DataBindings.Add("Text", launcher, "WorkingDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtArguments.DataBindings.Add("Text", launcher, "ArgumentString", false, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}