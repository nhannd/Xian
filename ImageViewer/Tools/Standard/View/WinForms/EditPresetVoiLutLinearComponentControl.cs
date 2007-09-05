
using System.Windows.Forms;
namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class EditPresetVoiLutLinearComponentControl : ClearCanvas.Desktop.View.WinForms.ApplicationComponentUserControl
	{
		private EditPresetVoiLutLinearComponent _component;

		public EditPresetVoiLutLinearComponentControl(EditPresetVoiLutLinearComponent component)
		{
			_component = component;
			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			_nameField.DataBindings.Add("Value", source, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowWidth.DataBindings.Add("Value", source, "WindowWidth", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowCenter.DataBindings.Add("Value", source, "WindowCenter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
