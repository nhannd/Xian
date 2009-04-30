#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

		private void button1_Click(object sender, EventArgs e)
		{
			_renderingSurface.ReportStats();
		}
	}
}
