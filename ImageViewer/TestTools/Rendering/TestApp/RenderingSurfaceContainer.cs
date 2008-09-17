using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Drawing.Imaging;

namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	public partial class RenderingSurfaceContainer : UserControl
	{
		private int _remainingDraws;

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

			_time.DataBindings.Add("Text", _renderingSurface, "Time", true, DataSourceUpdateMode.OnPropertyChanged);
			_draws.DataBindings.Add("Text", _renderingSurface, "Draws", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _useBitmap_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			DialogResult res = dlg.ShowDialog();
			if (res == System.Windows.Forms.DialogResult.OK)
			{
				_renderingSurface.Bitmap = (Bitmap)Bitmap.FromFile(dlg.FileName);
			}
		}

		private void _clearBitmap_Click(object sender, EventArgs e)
		{
			_renderingSurface.Bitmap = null;
		}

		private void _clearStats_Click(object sender, EventArgs e)
		{
			_renderingSurface.ClearStats();
		}

		private void _draw50_Click(object sender, EventArgs e)
		{
			this.Enabled = false;

			_renderingSurface.Paint += new PaintEventHandler(OnSurfacePainted);
			_remainingDraws = 50;
			_renderingSurface.Invalidate();
		}

		private void OnSurfacePainted(object sender, PaintEventArgs e)
		{
			--_remainingDraws;
			if (_remainingDraws == 0)
			{
				_renderingSurface.Paint -= new PaintEventHandler(OnSurfacePainted);
				this.Enabled = true;
			}
			else
			{
				_renderingSurface.Invalidate();
			}
		}
	}
}
