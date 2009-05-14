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
using ClearCanvas.ImageViewer.StudyManagement;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal delegate void NotifyChangedDelegate();

	internal class ImageBoxFrameSelectionStrategy : IDisposable
	{
		private readonly IImageBox _imageBox;
		private readonly int _window;
		private readonly NotifyChangedDelegate _notifyChanged;

		private readonly object _syncLock = new object();
		private readonly Queue<Frame> _frames;

		private int _currentIndex = -1;

		public ImageBoxFrameSelectionStrategy(IImageBox imageBox, int window, NotifyChangedDelegate notifyChanged)
		{
			_notifyChanged = notifyChanged;
			_imageBox = imageBox;
			_imageBox.Drawing += OnImageBoxDrawing;

			_window = window;
			_frames = new Queue<Frame>();
			Refresh(true);
		}

		private void OnImageBoxDrawing(object sender, EventArgs e)
		{
			Refresh(false);
		}

		public void OnDisplaySetChanged()
		{
			Refresh(true);
		}

		private void Refresh(bool force)
		{
			lock (_syncLock)
			{
				if (_imageBox.DisplaySet == null || _imageBox.DisplaySet.PresentationImages.Count == 0)
				{
					_frames.Clear();
					return;
				}

				if (!force && _currentIndex == _imageBox.TopLeftPresentationImageIndex)
					return;

				_frames.Clear();

				_currentIndex = _imageBox.TopLeftPresentationImageIndex;
				int numberOfImages = _imageBox.DisplaySet.PresentationImages.Count;
				int lastImageIndex = numberOfImages - 1;

				int selectionWindow;
				if (_window >= 0)
				{
					selectionWindow = 2 * _window + 1;
				}
				else 
				{
					//not terribly efficient, but by default will end up including all images.
					selectionWindow = 2 * numberOfImages + 1;
				}

				int offsetFromCurrent = 0;
				for (int i = 0; i < selectionWindow; ++i)
				{
					int index = _currentIndex + offsetFromCurrent;

					if (index >= 0 && index <= lastImageIndex)
					{
						IPresentationImage image = _imageBox.DisplaySet.PresentationImages[index];
						if (image is IImageSopProvider)
						{
							Frame frame = ((IImageSopProvider) image).Frame;
							if (frame.ParentImageSop.DataSource is StreamingSopDataSource)
							{
								StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
								IStreamingSopFrameData frameData = (IStreamingSopFrameData) (dataSource.GetFrameData(frame.FrameNumber));
								if (!frameData.PixelDataRetrieved)
									_frames.Enqueue(frame);
							}
						}
					}

					if (offsetFromCurrent == 0)
						++offsetFromCurrent;
					else if (offsetFromCurrent > 0)
						offsetFromCurrent = -offsetFromCurrent;
					else
						offsetFromCurrent = -offsetFromCurrent + 1;
				}

				string message = String.Format("Current: {0}, Window: {1}, Queue: {2}", _currentIndex, selectionWindow, _frames.Count);
				Trace.WriteLine(message);
			}

			//trigger another round of retrievals.
			_notifyChanged();
		}

		public Frame GetNextFrame()
		{
			lock (_syncLock)
			{
				if (_frames.Count == 0)
					return null;

				return _frames.Dequeue();
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			_imageBox.Drawing -= OnImageBoxDrawing;
		}

		#endregion
	}
}
