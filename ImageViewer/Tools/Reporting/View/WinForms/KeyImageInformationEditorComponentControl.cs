using System.ComponentModel;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Tools.Reporting.KeyImages;
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

			base.CancelButton = _cancelButton;
			base.AcceptButton = _okButton;

			cboTitle.DataBindings.Add("SelectedItem", component, "DocumentTitle");
			txtDesc.DataBindings.Add("Text", component, "Description");
			txtSeriesDesc.DataBindings.Add("Text", component, "SeriesDescription");
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