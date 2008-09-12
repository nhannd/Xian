using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Standard.Config;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms {
	public partial class StandardToolsConfigComponentControl : UserControl {
		private readonly StandardToolsConfigComponent _component;

		public StandardToolsConfigComponentControl() {
			InitializeComponent();
		}

		public StandardToolsConfigComponentControl(StandardToolsConfigComponent component) : this() {
			_component = component;

			chkShowCTRawPixelValue.DataBindings.Add("Checked", _component, "ShowCTRawPixelValue", false, DataSourceUpdateMode.OnPropertyChanged);
			chkShowNonCTModPixelValue.DataBindings.Add("Checked", _component, "ShowNonCTModPixelValue", false, DataSourceUpdateMode.OnPropertyChanged);
			chkShowVOIPixelValue.DataBindings.Add("Checked", _component, "ShowVOIPixelValue", false, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
