#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Represents a change made to a property value of an entity.
	/// </summary>
	public class PropertyChange
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="isCollection"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		public PropertyChange(string propertyName, bool isCollection, object oldValue, object newValue)
		{
			PropertyName = propertyName;
			OldValue = oldValue;
			NewValue = newValue;
			IsCollection = isCollection;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		public string PropertyName { get; private set; }

		/// <summary>
		/// Gets the old value of the property.
		/// </summary>
		public object OldValue { get; private set; }

		/// <summary>
		/// Gets the new value of the property.
		/// </summary>
		public object NewValue { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the property is a collection property.
		/// </summary>
		public bool IsCollection { get; private set; }

		/// <summary>
		/// Returns a new <see cref="PropertyChange"/> that is the result of adding this change
		/// to <paramref name="previousChange"/>.
		/// </summary>
		/// <param name="previousChange"></param>
		/// <returns></returns>
		/// <remarks>
		/// This operation is not commutative.
		/// </remarks>
		public PropertyChange AddTo(PropertyChange previousChange)
		{
			return new PropertyChange(PropertyName, IsCollection, previousChange.OldValue, NewValue);
		}
	}
}
