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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal delegate void NotifyListChangedDelegate();

	internal class ListObserver<T> : IDisposable where T : class
	{
		private readonly ObservableList<T> _list;
		private readonly NotifyListChangedDelegate _callback;
		private bool _suppressChangedEvent = false;

		public ListObserver(ObservableList<T> list, NotifyListChangedDelegate callback)
		{
			_callback = callback;
			_list = list;
			SuppressChangedEvent = false;
		}

		public bool SuppressChangedEvent
		{
			get { return _suppressChangedEvent; }
			set
			{
				if (_suppressChangedEvent == value)
					return;

				_suppressChangedEvent = value;
				if (_suppressChangedEvent)
				{
					_list.ItemAdded -= OnChanged;
					_list.ItemRemoved -= OnChanged;
					_list.ItemChanged -= OnChanged;
				}
				else
				{
					_list.ItemAdded += OnChanged;
					_list.ItemRemoved += OnChanged;
					_list.ItemChanged += OnChanged;
				}
			}
		}

		private void OnChanged(object sender, ListEventArgs<T> e)
		{
			_callback();
		}

		#region IDisposable Members

		public void Dispose()
		{
			SuppressChangedEvent = true;
		}

		#endregion
	}
}
