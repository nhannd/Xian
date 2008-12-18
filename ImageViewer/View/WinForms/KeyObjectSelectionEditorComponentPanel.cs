using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.KeyObjects;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class KeyObjectSelectionEditorComponentPanel : UserControl
	{
		public KeyObjectSelectionEditorComponentPanel()
		{
			InitializeComponent();

			foreach (object title in KeyObjectSelectionEditorComponent.StandardDocumentTitles)
			{
				cboTitle.Items.Add(title);
			}
		}

		public KeyObjectSelectionEditorComponentPanel(KeyObjectSelectionEditorComponent component) : this()
		{
			cboTitle.DataBindings.Add("SelectedItem", component, "DocumentTitle");
			txtDesc.DataBindings.Add("Text", component, "Description");
			txtSeriesDesc.DataBindings.Add("Text", component, "SeriesDescription");
			txtSeriesNum.DataBindings.Add("Text", component, "SeriesNumber");
			txtDateTime.DataBindings.Add("Text", component, "DateTime");
		}

		private void txtSeriesNum_Validating(object sender, CancelEventArgs e)
		{
			int result;
			if (!int.TryParse(txtSeriesNum.Text, out result))
				e.Cancel = true;
		}
	}
}