using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LayoutSettingsApplicationComponent"/>
    /// </summary>
    public partial class LayoutConfigurationApplicationComponentControl : UserControl
    {
        private LayoutConfigurationApplicationComponent _component;

        public LayoutConfigurationApplicationComponentControl(LayoutConfigurationApplicationComponent component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component.LayoutConfigurations;
			_comboBoxModality.DataSource = bindingSource;
			_comboBoxModality.DisplayMember = "Text";

			//these values are just constants, so we won't databind them, it's unnecessary.
			_imageBoxRows.Minimum = 1;
			_imageBoxColumns.Minimum = 1;
			_tileRows.Minimum = 1;
			_tileColumns.Minimum = 1;

			_imageBoxRows.Maximum = _component.MaximumImageBoxRows;
			_imageBoxColumns.Maximum = _component.MaximumImageBoxColumns;
			_tileRows.Maximum = _component.MaximumTileRows;
			_tileColumns.Maximum = _component.MaximumTileColumns;

			_imageBoxRows.DataBindings.Add("Value", bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageBoxColumns.DataBindings.Add("Value", bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileRows.DataBindings.Add("Value", bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileColumns.DataBindings.Add("Value", bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
