using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class DragDropObject : IDragDropObject
	{
		private IDataObject _dataObject;

		public DragDropObject(IDataObject dataObject)
		{
			_dataObject = dataObject;
		}

		public string[] GetFormats()
		{
			return _dataObject.GetFormats();
		}

		public object GetData(string format)
		{
			return _dataObject.GetData(format);
		}

		public object GetData(Type type)
		{
			return _dataObject.GetData(type);
		}

		public T GetData<T>()
		{
			return (T) GetData(typeof (T));
		}

		public bool HasData(string format)
		{
			return _dataObject.GetDataPresent(format);
		}

		public bool HasData(Type type)
		{
			return _dataObject.GetDataPresent(type);
		}

		public bool HasData<T>()
		{
			return HasData(typeof (T));
		}

		public void SetData(object data)
		{
			_dataObject.SetData(data);
		}

		public void SetData(string format, object data)
		{
			_dataObject.SetData(format, data);
		}

		public void SetData(Type type, object data)
		{
			_dataObject.SetData(type, data);
		}
	}
}