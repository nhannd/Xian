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