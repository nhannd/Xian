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
using System.Drawing;
using System.Threading;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	internal delegate Image IconCreatorDelegate();

	internal delegate void IconCreatedCallback();

	internal sealed class IconCache
	{
		private static readonly Image _placeHolderImage;
		private static readonly Image _nullImage;
		private readonly Dictionary<string, Image> _images;
		private readonly ProducerConsumerQueue<Task> _loadQueue;
		private readonly Thread _loaderThread;

		static IconCache()
		{
			_placeHolderImage = new Bitmap(64, 64);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_placeHolderImage))
			{
				StringFormat sf = new StringFormat();
				Font font = new Font(FontFamily.GenericSansSerif, 8);
				RectangleF rect = new RectangleF(PointF.Empty, _placeHolderImage.Size);
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.FillRectangle(Brushes.Black, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawEllipse(Pens.White, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawString("Loading...", font, Brushes.White, rect, sf);
				font.Dispose();
				sf.Dispose();
			}

			_nullImage = new Bitmap(64, 64);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_nullImage))
			{
				StringFormat sf = new StringFormat();
				Font font = new Font(FontFamily.GenericSansSerif, 8);
				RectangleF rect = new RectangleF(PointF.Empty, _nullImage.Size);
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.FillRectangle(Brushes.Black, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawRectangle(Pens.White, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawLine(Pens.Red, rect.Left, rect.Top, rect.Right, rect.Bottom);
				g.DrawLine(Pens.Red, rect.Right, rect.Top, rect.Left, rect.Bottom);
				g.DrawString("No Image", font, Brushes.White, rect, sf);
				font.Dispose();
				sf.Dispose();
			}
		}

		public IconCache()
		{
			_images = new Dictionary<string, Image>();
			_loadQueue = new ProducerConsumerQueue<Task>();
			_loaderThread = new Thread(new ThreadStart(Dequeue));
			_loaderThread.IsBackground = true;
			_loaderThread.Priority = ThreadPriority.BelowNormal;
			_loaderThread.Start();
		}

		public void Clear()
		{
			lock (_images)
			{
				_loadQueue.Clear();
				_images.Clear();
			}
		}

		public Image this[string key]
		{
			get
			{
				lock (_images)
				{
					if (_images.ContainsKey(key))
					{
						return _images[key];
					}
					else
					{
						return _nullImage;
					}
				}
			}
			private set
			{
				lock (_images)
				{
					if (_images.ContainsKey(key))
					{
						if (_images[key] != value)
						{
							if (value == null)
								value = _nullImage;
							_images[key] = value;
						}
					}
					else
					{
						_images.Add(key, value);
					}
				}
			}
		}

		public void LoadIcon(string key, Image icon)
		{
			this[key] = icon;
		}

		public void LoadIcon(string key, IconCreatorDelegate creatorDelegate, IconCreatedCallback onCreated)
		{
			this[key] = _placeHolderImage;
			_loadQueue.Produce(new Task(key, creatorDelegate, onCreated));
		}

		private void Dequeue()
		{
			Task task;
			while ((task = _loadQueue.Consume()) != null)
			{
				this[task.Key] = task.CreateImage();

				// TODO Remove this sleep
				//Thread.Sleep(500);

				task.NotifyCreated();
			}
		}

		private class Task
		{
			private IconCreatorDelegate _creatorDelegate;
			private IconCreatedCallback _onCreated;
			private string _key;

			public Task(string key, IconCreatorDelegate creatorDelegate, IconCreatedCallback onCreated)
			{
				_key = key;
				_creatorDelegate = creatorDelegate;
				_onCreated = onCreated;
			}

			public string Key
			{
				get { return _key; }
			}

			public Image CreateImage()
			{
				try
				{
					return _creatorDelegate();
				}
				catch (Exception)
				{
					return null;
				}
			}

			public void NotifyCreated()
			{
				_onCreated();
			}
		}

		/// <summary>
		/// A producer/consumer queue pattern class from http://www.yoda.arachsys.com/csharp/threads/deadlocks.shtml
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private class ProducerConsumerQueue<T>
		{
			private readonly object listLock = new object();
			private readonly Queue<T> queue = new Queue<T>();

			public void Produce(T item)
			{
				lock (listLock)
				{
					queue.Enqueue(item);

					// We always need to pulse, even if the queue wasn't
					// empty before. Otherwise, if we add several items
					// in quick succession, we may only pulse once, waking
					// a single thread up, even if there are multiple threads
					// waiting for items.            
					Monitor.Pulse(listLock);
				}
			}
			
			public void Clear()
			{
				lock (listLock)
				{
					queue.Clear();
					Monitor.Pulse(listLock);
				}
			}

			public T Consume()
			{
				lock (listLock)
				{
					// If the queue is empty, wait for an item to be added
					// Note that this is a while loop, as we may be pulsed
					// but not wake up before another thread has come in and
					// consumed the newly added object. In that case, we'll
					// have to wait for another pulse.
					while (queue.Count == 0)
					{
						// This releases listLock, only reacquiring it
						// after being woken up by a call to Pulse
						Monitor.Wait(listLock);
					}
					return queue.Dequeue();
				}
			}
		}
	}
}