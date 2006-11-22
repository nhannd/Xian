using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Volume;

namespace ClearCanvas.ImageViewer.Tools.Volume.View.WinForms
{
	public partial class TissueControl : UserControl
	{
		private TissueSettings _tissueSettings;
		private BindingSource _bindingSource;

		public TissueControl(TissueSettings tissueSettings)
		{
			_tissueSettings = tissueSettings;
			InitializeComponent();

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _tissueSettings;

			_visibleCheckBox.DataBindings.Add("Checked", _bindingSource, "TissueVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_opacityControl.DataBindings.Add("Enabled", _bindingSource, "TissueVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Minimum", _bindingSource, "OpacityMinValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Maximum", _bindingSource, "OpacityMaxValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Value", _bindingSource, "OpacityValue", true, DataSourceUpdateMode.OnPropertyChanged);

			_windowControl.DataBindings.Add("Enabled", _bindingSource, "TissueVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Minimum", _bindingSource, "WindowMinValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Maximum", _bindingSource, "WindowMaxValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Value", _bindingSource, "WindowValue", true, DataSourceUpdateMode.OnPropertyChanged);

			_levelControl.DataBindings.Add("Enabled", _bindingSource, "TissueVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Minimum", _bindingSource, "LevelMinValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Maximum", _bindingSource, "LevelMaxValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Value", _bindingSource, "LevelValue", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
