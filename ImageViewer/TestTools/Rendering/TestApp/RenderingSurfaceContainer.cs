using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Drawing.Imaging;

namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	public partial class RenderingSurfaceContainer : UserControl
	{
		public RenderingSurfaceContainer()
		{
			InitializeComponent();

			_comboFormat.DataSource = _renderingSurface.GetPixelFormats();
			_comboFormat.DataBindings.Add("SelectedValue", _renderingSurface, "Format", true,
										  DataSourceUpdateMode.OnPropertyChanged);

			_comboSource.DataSource = _renderingSurface.GetGraphicsSources();
			_comboSource.DataBindings.Add("SelectedValue", _renderingSurface, "Source", true,
			                              DataSourceUpdateMode.OnPropertyChanged);

			_customBackBuffer.DataBindings.Add("Checked", _renderingSurface, "CustomBackBuffer", true,
			                                   DataSourceUpdateMode.OnPropertyChanged);

			_useBufferedGraphics.DataBindings.Add("Enabled", _customBackBuffer, "Checked", true,
			                                      DataSourceUpdateMode.OnPropertyChanged);
			_useBufferedGraphics.DataBindings.Add("Checked", _renderingSurface, "UseBufferedGraphics", true,
												  DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
