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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class TrackSlider
	{
		/// <summary>
		/// The internal track bar control.
		/// </summary>
		private class TrackBar : IDisposable
		{
			private TrackSlider _owner;
			private Rectangle _clientBounds;

			private Rectangle _thumbBounds = Rectangle.Empty;
			private Rectangle _trackStartBounds = Rectangle.Empty;
			private Rectangle _trackEndBounds = Rectangle.Empty;
			private Rectangle _trackBounds = Rectangle.Empty;
			private bool _recomputeBounds = true;

			private Bitmap _buffer;

			public TrackBar(TrackSlider owner)
			{
				_owner = owner;
			}

			public void Dispose()
			{
				this.Invalidate();

				_owner = null;
			}

			/// <summary>
			/// Updates the region of the control that the track bar can use.
			/// </summary>
			public void UpdateClientBounds(Rectangle clientRectangle, Padding padding)
			{
				_clientBounds = new Rectangle(new Point(padding.Left, padding.Top),
				                              new Size(clientRectangle.Width - padding.Horizontal, clientRectangle.Height - padding.Vertical));
				this.Invalidate();
			}

			/// <summary>
			/// Invalidates the buffer, causing everything to be recreated.
			/// </summary>
			public void Invalidate()
			{
				_recomputeBounds = true;

				if (_buffer != null)
				{
					_buffer.Dispose();
					_buffer = null;
				}
			}

			/// <summary>
			/// Gets the bounds of the thumb slider.
			/// </summary>
			public Rectangle ThumbBounds
			{
				get
				{
					this.TryRecomputeBounds();
					return _thumbBounds;
				}
			}

			/// <summary>
			/// Gets the bounds of the decrement arrow at the start of the track.
			/// </summary>
			public Rectangle TrackStartBounds
			{
				get
				{
					this.TryRecomputeBounds();
					return _trackStartBounds;
				}
			}

			/// <summary>
			/// Gets the bounds of the increment arrow at the end of the track.
			/// </summary>
			public Rectangle TrackEndBounds
			{
				get
				{
					this.TryRecomputeBounds();
					return _trackEndBounds;
				}
			}

			/// <summary>
			/// Gets the bounds of the track bar excluding the increment/decrement arrows.
			/// </summary>
			public Rectangle TrackBounds
			{
				get
				{
					this.TryRecomputeBounds();
					return _trackBounds;
				}
			}

			private bool IsVertical
			{
				get { return _owner._orientation == Orientation.Vertical; }
			}

			private Image GetVisualElement(TrackSliderVisualElement element)
			{
				return _owner.ReferencedStyle.GetVisualElement(element, _owner._orientation == Orientation.Vertical);
			}

			private void TryRecomputeBounds()
			{
				if (_recomputeBounds)
				{
					Image elementStart = GetVisualElement(TrackSliderVisualElement.TrackStart);
					Image elementEnd = GetVisualElement(TrackSliderVisualElement.TrackEnd);

					if (this.IsVertical)
					{
						_trackStartBounds = new Rectangle(Point.Empty, new Size(_clientBounds.Width, elementStart.Height));
						_trackBounds = new Rectangle(new Point(0, elementStart.Height), new Size(_clientBounds.Width, _clientBounds.Height - elementStart.Height - elementEnd.Height));
						_trackEndBounds = new Rectangle(new Point(0, _clientBounds.Height - elementEnd.Height), new Size(_clientBounds.Width, elementEnd.Height));
					}
					else
					{
						_trackStartBounds = new Rectangle(Point.Empty, new Size(elementStart.Width, _clientBounds.Height));
						_trackBounds = new Rectangle(new Point(elementStart.Width, 0), new Size(_clientBounds.Width - elementStart.Width - elementEnd.Width, _clientBounds.Height));
						_trackEndBounds = new Rectangle(new Point(_clientBounds.Width - elementEnd.Width, 0), new Size(elementEnd.Width, _clientBounds.Height));
					}

					if (_trackBounds.Width >= 0 && _trackBounds.Height >= 0)
					{
						float percentile = 0.5f;
						if (_owner._maximumValue > _owner._minimumValue)
							percentile = Math.Min(1, Math.Max(0, 1f*(_owner._value - _owner._minimumValue)/(_owner._maximumValue - _owner._minimumValue)));
						Size thumbSize = GetVisualElement(TrackSliderVisualElement.Thumb).Size;

						if (this.IsVertical)
						{
							int y = (int) (_trackBounds.Height*percentile) - thumbSize.Height/2;
							_thumbBounds = new Rectangle(_trackBounds.Location + new Size(0, y), new Size(_trackBounds.Width, thumbSize.Height));
						}
						else
						{
							int x = (int) (_trackBounds.Width*percentile) - thumbSize.Width/2;
							_thumbBounds = new Rectangle(_trackBounds.Location + new Size(x, 0), new Size(thumbSize.Width, _trackBounds.Height));
						}
					}
					else
					{
						_trackBounds = Rectangle.Empty;
						_thumbBounds = Rectangle.Empty;
					}

					_recomputeBounds = false;
				}
			}

			/// <summary>
			/// Gets a cached buffer graphic of the track bar.
			/// </summary>
			private Bitmap Buffer
			{
				get
				{
					if (_buffer == null)
					{
						this.TryRecomputeBounds();

						_buffer = new Bitmap(_clientBounds.Width, _clientBounds.Height, PixelFormat.Format32bppArgb);

						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_buffer))
						{
							Image elementStart = GetVisualElement(TrackSliderVisualElement.TrackStart);
							Image elementTrack = GetVisualElement(TrackSliderVisualElement.Track);
							Image elementEnd = GetVisualElement(TrackSliderVisualElement.TrackEnd);
							Image elementThumb = GetVisualElement(TrackSliderVisualElement.Thumb);

							// for whatever reason, the GDI+ DrawImage causes weird uneven interpolation around the end of the track when stretching it
							// so we render a track 10% larger than needed, then clip the good part only when doing the actual painting
							Image oversizeTrack = null;

							bool valid = (_trackBounds.Width > 0 && _trackBounds.Height > 0);

							if (valid)
							{
								if (this.IsVertical)
								{
									oversizeTrack = new Bitmap(_trackBounds.Width, (int) (_trackBounds.Height*1.1));
									using (System.Drawing.Graphics gTrack = System.Drawing.Graphics.FromImage(oversizeTrack))
									{
										gTrack.DrawImage(elementTrack, new Rectangle(Point.Empty, oversizeTrack.Size));
									}
								}
								else
								{
									oversizeTrack = new Bitmap((int) (_trackBounds.Width*1.1), _trackBounds.Height);
									using (System.Drawing.Graphics gTrack = System.Drawing.Graphics.FromImage(oversizeTrack))
									{
										gTrack.DrawImage(elementTrack, new Rectangle(Point.Empty, oversizeTrack.Size));
									}
								}

								g.DrawImageUnscaledAndClipped(oversizeTrack, _trackBounds);
								g.DrawImage(elementStart, _trackStartBounds);
								g.DrawImage(elementEnd, _trackEndBounds);
								g.DrawImage(elementThumb, _thumbBounds);
							}

							if (oversizeTrack != null)
								oversizeTrack.Dispose();
						}
					}
					return _buffer;
				}
			}

			/// <summary>
			/// Draws the current state of the track bar to the specified graphics context with a given alpha level.
			/// </summary>
			public void DrawTrackBar(System.Drawing.Graphics g, int alpha)
			{
				if (alpha >= 255)
				{
					// if the alpha value is 255, skip the secondary buffer since the primary buffer has exactly what we need!
					g.DrawImageUnscaled(this.Buffer, _clientBounds.Location);
				}
				else if (alpha > 0)
				{
					// if the alpha value is 0, we don't need to draw anything!

					Bitmap source = this.Buffer;
					Bitmap buffer = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
					Rectangle rectangle = new Rectangle(Point.Empty, source.Size);

					BitmapData sourceData = source.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					BitmapData bufferData = buffer.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

					try
					{
						unsafe
						{
							int length = sourceData.Height*sourceData.Stride;
							byte* sourceDataArray = (byte*) sourceData.Scan0.ToPointer();
							byte* bufferDataArray = (byte*) bufferData.Scan0.ToPointer();
							for (int n = 0; n < length; n += 4)
							{
								bufferDataArray[n + 0] = sourceDataArray[n + 0];
								bufferDataArray[n + 1] = sourceDataArray[n + 1];
								bufferDataArray[n + 2] = sourceDataArray[n + 2];
								bufferDataArray[n + 3] = (byte) (alpha*(sourceDataArray[n + 3]/255f));
							}
						}
					}
					finally
					{
						source.UnlockBits(sourceData);
						buffer.UnlockBits(bufferData);
					}

					g.DrawImageUnscaled(buffer, _clientBounds.Location);
				}
			}
		}
	}
}