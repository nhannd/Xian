using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class LinearPresetVoiLutApplicatorComponentControl : ClearCanvas.Desktop.View.WinForms.ApplicationComponentUserControl
	{
		private readonly LinearPresetVoiLutApplicatorComponent _component;

		public LinearPresetVoiLutApplicatorComponentControl(LinearPresetVoiLutApplicatorComponent component)
		{
			_component = component;
			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			_nameField.DataBindings.Add("Value", source, "PresetName", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowWidth.DataBindings.Add("Value", source, "WindowWidth", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowCenter.DataBindings.Add("Value", source, "WindowCenter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
