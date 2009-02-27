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
