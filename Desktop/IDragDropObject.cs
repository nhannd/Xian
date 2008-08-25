using System;

namespace ClearCanvas.Desktop
{
	public interface IDragDropObject
	{
		string[] GetFormats();

		object GetData(Type type);
		object GetData(string format);
		T GetData<T>();

		bool HasData(Type type);
		bool HasData(string format);
		bool HasData<T>();

		void SetData(object data);
		void SetData(string format, object data);
		void SetData(Type type, object data);
	}
}