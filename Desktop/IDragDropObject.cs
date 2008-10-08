using System;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Interface for objects representing a generic, multi-format object wrapper.
	/// </summary>
	/// <remarks>
	/// This interface is used in situations where interactions between components
	/// require the runtime selection of a data format suitable for both components
	/// involved, such as a drag and drop or clipboard copy paste to unknown components.
	/// </remarks>
	public interface IDragDropObject
	{
		/// <summary>
		/// Gets a string array of format descriptors available in this wrapper.
		/// </summary>
		/// <returns>A string array of format descriptors.</returns>
		string[] GetFormats();

		/// <summary>
		/// Gets the data encapsulated in this wrapper in the specified format.
		/// </summary>
		/// <param name="type">The type of the data to extract.</param>
		/// <returns>An object of type <paramref name="type"/>, or null if the data is not available in the specified format.</returns>
		object GetData(Type type);

		/// <summary>
		/// Gets the data encapsulated in this wrapper in the specified format.
		/// </summary>
		/// <param name="format">The format descriptor of the data to extract.</param>
		/// <returns>An object matching the specified <paramref name="format"/>, or null if the data is not available in the specified format.</returns>
		object GetData(string format);

		/// <summary>
		/// Gets the data encapsulated in this wrapper in the specified format.
		/// </summary>
		/// <typeparam name="T">The type of the data to extract.</typeparam>
		/// <returns>An object of type <typeparamref name="T"/>, or null if the data is not available in the specified format.</returns>
		T GetData<T>();

		/// <summary>
		/// Checks if the data encapsulated in this wrapper is available in the specified format.
		/// </summary>
		/// <param name="type">The type of the data to check for.</param>
		/// <returns>True if an object of type <paramref name="type"/> is available; False if the data is not available in the specified format.</returns>
		bool HasData(Type type);

		/// <summary>
		/// Checks if the data encapsulated in this wrapper is available in the specified format.
		/// </summary>
		/// <param name="format">The format descriptor of the data to check for.</param>
		/// <returns>True if an object matching the specified <paramref name="format"/> is available; False if the data is not available in the specified format.</returns>
		bool HasData(string format);

		/// <summary>
		/// Checks if the data encapsulated in this wrapper is available in the specified format.
		/// </summary>
		/// <typeparam name="T">The type of the data to check for.</typeparam>
		/// <returns>True if an object of type <typeparamref name="T"/> is available; False if the data is not available in the specified format.</returns>
		bool HasData<T>();

		/// <summary>
		/// Sets the data in the wrapper using the object's type as the format.
		/// </summary>
		/// <param name="data">The data object to encapsulate.</param>
		void SetData(object data);

		/// <summary>
		/// Sets the data in the wrapper using the specified format descriptor.
		/// </summary>
		/// <param name="format">The format descriptor to encapsulate the data as.</param>
		/// <param name="data">The data object to encapsulate.</param>
		void SetData(string format, object data);

		/// <summary>
		/// Sets the data in the wrapper using the specified type as the format.
		/// </summary>
		/// <param name="type">The type to encapsulate the data as.</param>
		/// <param name="data">The data object to encapsulate.</param>
		void SetData(Type type, object data);
	}
}