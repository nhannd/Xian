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
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class ViewerFrameEnumerator : IBlockingEnumerator<Frame>, IDisposable
	{
		private readonly IImageViewer _viewer;

		private readonly object _syncLock = new object();
		private readonly Queue<Frame> _framesToProcess;
		private readonly Dictionary<IImageBox, ImageBoxFrameSelectionStrategy> _imageBoxStrategies;
		private IEnumerator<KeyValuePair<IImageBox, ImageBoxFrameSelectionStrategy>> _strategyEnumerator;
		private IImageBox _selectedImageBox;

		private readonly int _selectedWeight;
		private readonly int _unselectedWeight;
		private readonly int _imageWindow;

		private volatile bool _continueBlocking = true;

		public ViewerFrameEnumerator(IImageViewer viewer, int selectedWeight, int unselectedWeight, int imageWindow)
		{
			_viewer = viewer;
			_selectedWeight = selectedWeight;
			_unselectedWeight = unselectedWeight;
			_imageWindow = imageWindow;

			_framesToProcess = new Queue<Frame>();

			_viewer.PhysicalWorkspace.ImageBoxes.ItemAdded += OnImageBoxAdded;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemRemoved += OnImageBoxRemoved;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanging += OnImageBoxChanging;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanged += OnImageBoxChanged;

			_viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			_viewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;

			_imageBoxStrategies = new Dictionary<IImageBox, ImageBoxFrameSelectionStrategy>();

			foreach(IImageBox imageBox in _viewer.PhysicalWorkspace.ImageBoxes)
				_imageBoxStrategies[imageBox] = new ImageBoxFrameSelectionStrategy(imageBox, _imageWindow, OnImageBoxDataChanged);

			_selectedImageBox = _viewer.SelectedImageBox;
		}

		private void OnImageBoxRemoved(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				if (_imageBoxStrategies.ContainsKey(e.Item))
					_imageBoxStrategies[e.Item].Dispose();

				_imageBoxStrategies.Remove(e.Item);

				OnImageBoxesChanged();
			}
		}

		private void OnImageBoxChanging(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				if (_imageBoxStrategies.ContainsKey(e.Item))
					_imageBoxStrategies[e.Item].Dispose();

				_imageBoxStrategies.Remove(e.Item);

				OnImageBoxesChanged();
			}
		}

		private void OnImageBoxChanged(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				_imageBoxStrategies[e.Item] = new ImageBoxFrameSelectionStrategy(e.Item, _imageWindow, OnImageBoxDataChanged);

				OnImageBoxesChanged();
			}
		}

		private void OnImageBoxAdded(object sender, ListEventArgs<IImageBox> e)
		{
			lock (_syncLock)
			{
				_imageBoxStrategies[e.Item] = new ImageBoxFrameSelectionStrategy(e.Item, _imageWindow, OnImageBoxDataChanged);

				OnImageBoxesChanged();
			}
		}


		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			lock (_syncLock)
			{
				if (_imageBoxStrategies.ContainsKey(e.SelectedImageBox))
					_selectedImageBox = e.SelectedImageBox;
				else
					_selectedImageBox = null;

				Monitor.PulseAll(_syncLock);
			}
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			lock(_syncLock)
			{
				if (e.NewDisplaySet != null)
				{
					if (_imageBoxStrategies.ContainsKey(e.NewDisplaySet.ImageBox))
						_imageBoxStrategies[e.NewDisplaySet.ImageBox].OnDisplaySetChanged();
				}

				Monitor.PulseAll(_syncLock);
			}
		}

		private void OnImageBoxesChanged()
		{
			_strategyEnumerator = null;
			_framesToProcess.Clear();
			Monitor.PulseAll(_syncLock);
		}

		private void OnImageBoxDataChanged()
		{
			lock (_syncLock)
			{
				Monitor.PulseAll(_syncLock);
			}
		}

		private IEnumerable<Frame> GetFrames()
		{
			while (_continueBlocking)
			{
				Frame nextFrame = null;
				lock(_syncLock)
				{
					if (_framesToProcess.Count == 0)
					{
						GetNextRoundOfFrames();
						if (_framesToProcess.Count == 0)
							Monitor.Wait(_syncLock);
					}

					if (_framesToProcess.Count > 0)
						nextFrame = _framesToProcess.Dequeue();
				}

				if (nextFrame != null)
					yield return nextFrame;
			}
		}

		private void GetNextRoundOfFrames()
		{
			lock (_syncLock)
			{
				if (_strategyEnumerator == null)
					_strategyEnumerator = _imageBoxStrategies.GetEnumerator();

				while (_strategyEnumerator != null)
				{
					AddSelectedImageBoxFrames();

					int count = 0;
					while (true)
					{
						if (!_strategyEnumerator.MoveNext())
						{
							_strategyEnumerator = null;
							break;
						}

						if (_strategyEnumerator.Current.Key != _selectedImageBox)
						{
							if (_imageBoxStrategies.ContainsKey(_strategyEnumerator.Current.Key))
							{
								Frame frame = _imageBoxStrategies[_strategyEnumerator.Current.Key].GetNextFrame();
								if (frame != null)
								{
									_framesToProcess.Enqueue(frame);
									Monitor.Pulse(_syncLock);

									if (++count >= _unselectedWeight)
										break;
								}
							}
						}
					}
				}
			}
		}

		private void AddSelectedImageBoxFrames()
		{
			ImageBoxFrameSelectionStrategy strategy;
			if (_selectedImageBox != null && _imageBoxStrategies.ContainsKey(_selectedImageBox))
			{
				strategy = _imageBoxStrategies[_selectedImageBox];
				for (int i = 0; i < _selectedWeight; ++i)
				{
					Frame nextFrame = strategy.GetNextFrame();
					if (nextFrame != null)
					{
						_framesToProcess.Enqueue(nextFrame);
						Monitor.Pulse(_syncLock);
					}
					else
					{
						break;
					}
				}
			}
		}

		#region IBlockingEnumerator<Frame> Members

		public bool ContinueBlocking
		{
			get { return _continueBlocking; }
			set
			{
				if (_continueBlocking == value)
					return;

				_continueBlocking = value;
				if (!_continueBlocking)
				{
					lock (_syncLock)
					{
						Monitor.PulseAll(_syncLock);
					}
				}
			}
		}

		#endregion

		#region IEnumerable<Frame> Members

		public IEnumerator<Frame> GetEnumerator()
		{
			return GetFrames().GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetFrames().GetEnumerator();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			ContinueBlocking = false;

			_viewer.PhysicalWorkspace.ImageBoxes.ItemAdded -= OnImageBoxAdded;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemRemoved -= OnImageBoxRemoved;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanging -= OnImageBoxChanging;
			_viewer.PhysicalWorkspace.ImageBoxes.ItemChanged -= OnImageBoxChanged;

			_viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			_viewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;

			if (_imageBoxStrategies != null)
			{
				foreach (ImageBoxFrameSelectionStrategy strategy in _imageBoxStrategies.Values)
				{
					try
					{
						strategy.Dispose();
					}
					catch(Exception e)
					{
						Platform.Log(LogLevel.Warn, e, "Error disposing frame selection strategy.");
					}
				}

				_imageBoxStrategies.Clear();
			}
		}

		#endregion
	}
}
