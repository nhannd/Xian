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

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	public partial class RenderingSurface : UserControl, INotifyPropertyChanged
	{
		private readonly TestImageRenderer _renderer;
		private int _draws;
		private TimeSpan _time;

		public RenderingSurface()
		{
			InitializeComponent();
			_renderer = new TestImageRenderer();
			DoubleBuffered = !_renderer.CustomBackBuffer;
		}

		public IList<PixelFormat> GetPixelFormats()
		{
			return _renderer.GetPixelFormats();
		}

		public IList<GraphicsSource> GetGraphicsSources()
		{
			return _renderer.GetGraphicsSources();
		}

		public PixelFormat Format
		{
			get { return _renderer.Format; }
			set
			{
				_renderer.Format = value;
				Invalidate();
				FirePropertyChanged("PixelFormat");
			}
		}

		public GraphicsSource Source
		{
			get { return _renderer.Source; }
			set
			{
				_renderer.Source = value;
				Invalidate();
				FirePropertyChanged("GraphicsSource");
			}
		}

		public bool CustomBackBuffer
		{
			get { return _renderer.CustomBackBuffer; }
			set
			{
				_renderer.CustomBackBuffer = value;
				DoubleBuffered = !_renderer.CustomBackBuffer;

				Invalidate();
				FirePropertyChanged("CustomBackBuffer");
			}
		}

		public bool UseBufferedGraphics
		{
			get { return _renderer.UseBufferedGraphics; }
			set
			{
				_renderer.UseBufferedGraphics = value;
				Invalidate();
				FirePropertyChanged("UseBufferedGraphics");
			}
		}

		public Bitmap Bitmap
		{
			get { return _renderer.CustomImage; }
			set 
			{ 
				_renderer.CustomImage = value;
				Invalidate();
			}
		}

		public int Draws
		{
			get { return _draws; }	
		}

		public string Time
		{
			get { return _time.TotalSeconds.ToString("F2"); }	
		}

		public void ClearStats()
		{
			_draws = 0;
			_time = TimeSpan.Zero;
			_renderer.ResetStats();

			FirePropertyChanged("Draws");
			FirePropertyChanged("Time");
		}

		public void ReportStats()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("Render: {0:F2}", _renderer.RenderTime.TotalSeconds);
			builder.AppendLine();
			builder.AppendFormat("Blit: {0:F2}", _renderer.BlitTime.TotalSeconds);
			MessageBox.Show(this, builder.ToString());
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				DateTime start = DateTime.Now;
				_renderer.RenderTo(e.Graphics, this.Size);
				
				DateTime end = DateTime.Now;
				TimeSpan duration = end.Subtract(start);
				_time = _time.Add(duration);
				++_draws;

				FirePropertyChanged("Draws");
				FirePropertyChanged("Time");
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			base.OnPaint(e);
		}

		private void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
