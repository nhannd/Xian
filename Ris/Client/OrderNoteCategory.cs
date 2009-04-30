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

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines order note categories.  These categories are entirely in the domain of the client -
	/// the server is ignorant of the order note categories.
	/// </summary>
	public class OrderNoteCategory : IEquatable<OrderNoteCategory>
	{
		/// <summary>
		/// General category.
		/// </summary>
		public static readonly OrderNoteCategory General = new OrderNoteCategory("General", SR.OrderNoteCategoryGeneral);

		/// <summary>
		/// Preliminary Diagnosis category.
		/// </summary>
		public static readonly OrderNoteCategory PreliminaryDiagnosis = new OrderNoteCategory("PrelimDiagnosis", SR.OrderNoteCategoryPrelimDiagnosis);

		/// <summary>
		/// Preliminary Diagnosis category.
		/// </summary>
		public static readonly OrderNoteCategory Protocol = new OrderNoteCategory("Protocol", SR.OrderNoteCategoryProtocol);

		/// <summary>
		/// Gets the <see cref="OrderNoteCategory"/> object corresponding to the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static OrderNoteCategory FromKey(string key)
		{
			if (key.Equals(General.Key, StringComparison.InvariantCultureIgnoreCase))
				return General;

			if (key.Equals(PreliminaryDiagnosis.Key, StringComparison.InvariantCultureIgnoreCase))
				return PreliminaryDiagnosis;

			throw new ArgumentOutOfRangeException("key");
		}


		private readonly string _key;
		private readonly string _displayValue;

		private OrderNoteCategory(string key, string displayValue)
		{
			_key = key;
			_displayValue = displayValue;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		public string Key
		{
			get { return _key; }
		}

		/// <summary>
		/// Gets the display value.
		/// </summary>
		public string DisplayValue
		{
			get { return _displayValue; }
		}


		#region Equality Operators


		public static bool operator !=(OrderNoteCategory orderNoteCategory1, OrderNoteCategory orderNoteCategory2)
		{
			return !Equals(orderNoteCategory1, orderNoteCategory2);
		}

		public static bool operator ==(OrderNoteCategory orderNoteCategory1, OrderNoteCategory orderNoteCategory2)
		{
			return Equals(orderNoteCategory1, orderNoteCategory2);
		}

		public bool Equals(OrderNoteCategory orderNoteCategory)
		{
			if (orderNoteCategory == null) return false;
			return Equals(_key, orderNoteCategory._key);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as OrderNoteCategory);
		}

		public override int GetHashCode()
		{
			return _key.GetHashCode();
		}

		#endregion
	}
}
