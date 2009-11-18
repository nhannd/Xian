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
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Abstract base implementation of <see cref="ITableCellEditor"/>, providing common functionality.
	/// </summary>
	public abstract class TableCellEditor : ITableCellEditor
	{
		private ITableColumn _column;
		private event EventHandler _valueChanged;
		private object _item;

		#region ITableCellEditor members

		/// <summary>
		/// Called by the framework to associate this editor with the specified column.
		/// </summary>
		/// <param name="column"></param>
		public void SetColumn(ITableColumn column)
		{
			_column = column;
		}

		/// <summary>
		/// Informs the editor that it is going to begin an edit on the specified item.
		/// </summary>
		/// <param name="item"></param>
		public void BeginEdit(object item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets or sets the value (e.g. content) of the editor.
		/// </summary>
		public object Value
		{
			get { return _item == null ? null : _column.GetValue(_item); }
			set
			{
				if (_item != null)
				{
					// check for no-op
					if(Equals(_column.GetValue(_item), value))
						return;

					_column.SetValue(_item, value);
					NotifyValueChanged();
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="ITableCellEditor.Value"/> property is modified.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { _valueChanged += value; }
			remove { _valueChanged -= value; }
		}

		#endregion


		private void NotifyValueChanged()
		{
			EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
		}
	}
}
