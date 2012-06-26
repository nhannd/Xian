using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class AttachDocumentComponentControl : ApplicationComponentUserControl
	{
		private readonly AttachDocumentComponent _component;

		public AttachDocumentComponentControl(AttachDocumentComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_file.DataBindings.Add("Value", _component, "FilePath", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _browseButton_Click(object sender, EventArgs e)
		{
			_component.BrowseFiles();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			using(new CursorManager(this, Cursors.WaitCursor))
			{
				_component.Accept();
			}
		}
	}
}
