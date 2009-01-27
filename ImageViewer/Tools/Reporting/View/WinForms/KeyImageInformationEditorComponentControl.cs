using System.ComponentModel;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Tools.Reporting.KeyObjects;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
	public partial class KeyImageInformationEditorComponentControl : ApplicationComponentUserControl
	{
		private KeyImageInformationEditorComponent _component;

		public KeyImageInformationEditorComponentControl(KeyImageInformationEditorComponent component) 
			: base(component)
		{
			_component = component;
			InitializeComponent();

			foreach (KeyObjectSelectionDocumentTitle title in KeyImageInformationEditorComponent.StandardDocumentTitles)
			{
				cboTitle.Items.Add(title);
			}

			cboTitle.DataBindings.Add("SelectedItem", component, "DocumentTitle");
			txtDesc.DataBindings.Add("Text", component, "Description");
			txtSeriesDesc.DataBindings.Add("Text", component, "SeriesDescription");
			txtSeriesNum.DataBindings.Add("Text", component, "SeriesNumber");
			txtDateTime.DataBindings.Add("Text", component, "DateTime");
		}

		private void OnValidatingSeriesNumber(object sender, CancelEventArgs e)
		{
			int result;
			if (!int.TryParse(txtSeriesNum.Text, out result))
				e.Cancel = true;
		}

		private void OnOk(object sender, System.EventArgs e)
		{
			_component.Accept();
		}

		private void OnCancel(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}
	}
}