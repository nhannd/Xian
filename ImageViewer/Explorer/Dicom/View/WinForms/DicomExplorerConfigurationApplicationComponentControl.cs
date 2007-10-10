using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomExplorerConfigurationApplicationComponent"/>
    /// </summary>
    public partial class DicomExplorerConfigurationApplicationComponentControl : ApplicationComponentUserControl
    {
        private DicomExplorerConfigurationApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomExplorerConfigurationApplicationComponentControl(DicomExplorerConfigurationApplicationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_showPhoneticIdeographicNames.DataBindings.Add("Checked", bindingSource, "ShowPhoneticIdeographicNames", true, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
