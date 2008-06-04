using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="MergeComponentBase"/>
	/// </summary>
	public partial class MergeComponentBaseControl : ApplicationComponentUserControl
	{
		private readonly MergeComponentBase _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public MergeComponentBaseControl(MergeComponentBase component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_duplicateLookupField.LookupHandler = _component.DuplicateLookupHandler;
			_duplicateLookupField.DataBindings.Add("Value", _component, "SelectedDuplicate", true, DataSourceUpdateMode.OnPropertyChanged);

			_originalLookupField.LookupHandler = _component.OriginalLookupHandler;
			_originalLookupField.DataBindings.Add("Value", _component, "SelectedOriginal", true, DataSourceUpdateMode.OnPropertyChanged);

			_report.DataBindings.Add("Value", _component, "MergeReport", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _switchButton_Click(object sender, EventArgs e)
		{
			_component.Switch();
		}
	}
}
