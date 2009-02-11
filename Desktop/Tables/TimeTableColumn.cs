#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Subclass of the <see cref="TableColumn{TItem,TColumn}"/> class for a nullable DateTime column type.
	/// The value is formatted as time only.
	/// </summary>
	/// <typeparam name="TItem">The type of item on which the table is based.</typeparam>
	public class TimeTableColumn<TItem> : DateTimeTableColumn<TItem>
	{
		/// <summary>
		/// Constructs a read-only single-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public TimeTableColumn(string columnName, GetColumnValueDelegate<TItem, DateTime?> valueGetter, float widthFactor)
			: base(columnName, valueGetter, widthFactor)
		{
			this.ValueFormatter = delegate(DateTime? value) { return Format.Time(value); };
			this.Comparison = delegate(TItem item1, TItem item2) { return Nullable.Compare(valueGetter(item1), valueGetter(item2)); };
		}
	}
}
