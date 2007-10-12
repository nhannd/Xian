#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Abstract base implementation of <see cref="ITableColumn"/> for use with the <see cref="Table"/> class.
    /// Application code should use the concrete <see cref="TableColumn"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item on which the table is based</typeparam>
    public abstract class TableColumnBase<TItem> : ITableColumn
    {
        /// <summary>
        /// Comparer for sorting operations
        /// </summary>
        class SortComparer : IComparer
        {
            private int _direction;
            private Comparison<TItem> _comp;

            public SortComparer(Comparison<TItem> comparison, bool ascending)
            {
                _comp = comparison;
                _direction = ascending ? 1 : -1;
            }

            public int Compare(object x, object y)
            {
                return _comp((TItem)x, (TItem)y) * _direction;
            }
        }

        private Table<TItem> _table;
        private string _name;
		private bool _visible = true;
        private Type _columnType;
        private float _widthFactor;

        private Comparison<TItem> _comparison;
		private event EventHandler _visibilityChangedEvent;

        private IResourceResolver _resolver;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="columnType">The type of value that the column holds</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column</param>
        /// <param name="comparison">A custom comparison operator that is used for sorting based on this column</param>
        public TableColumnBase(
			string columnName, 
			Type columnType, 
			float widthFactor, 
			Comparison<TItem> comparison)
        {
            _name = columnName;
            _widthFactor = widthFactor;
            _columnType = columnType;
            _comparison = comparison;

            // if no comparison operator was specified, assign a default comparison
            if (_comparison == null)
            {
                // if TColumn implements IComparable, can compare by column value
                if (typeof(IComparable).IsAssignableFrom(_columnType))
                    _comparison = ValueComparsion;
                else
                    _comparison = NopComparison;    // no comparison is possible
            }
        }

        /// <summary>
        /// Gets or sets the comparison delegate that will be used to sort the table according to this column.
        /// </summary>
        public Comparison<TItem> Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
        }

        /// <summary>
        /// Used by the framework to associate this column with a parent <see cref="Table"/>
        /// </summary>
        internal Table<TItem> Table
        {
            get { return _table; }
            set { _table = value; }
        }

        #region ITableColumn members

        public string Name
        {
            get { return _name; }
        }

        public Type ColumnType
        {
            get { return _columnType; }
        }

		public bool Visible
		{
			get { return _visible; }
			set 
			{ 
				_visible = value;
				EventsHelper.Fire(_visibilityChangedEvent, this, EventArgs.Empty);
			}
		}

        public IResourceResolver ResourceResolver
        {
            get { return _resolver; }
            set { _resolver = value; }
        }
        
        public event EventHandler VisibilityChanged
		{
			add { _visibilityChangedEvent += value; }
			remove { _visibilityChangedEvent -= value; }
		}
		
		/// <summary>
        /// Gets or sets a factor that determines the width of this column relative to other columns in the table.
        /// The default value is 1.0.  
        /// </summary>
        public float WidthFactor
        {
            get { return _widthFactor; }
            set
            {
                _widthFactor = value;
                if (_table != null)
                {
                    _table.NotifyColumnChanged(this);
                }
            }
        }

        public int WidthPercent
        {
            get
            {
                if (_table == null)
					throw new InvalidOperationException(SR.ExceptionTableColumnMustBeAddedToDetermineWidth);
                
                return _table.GetColumnWidthPercent(this);
            }
        }

        public abstract bool ReadOnly { get; }

        public abstract object GetValue(object item);

        public abstract void SetValue(object item, object value);

        public IComparer GetComparer(bool ascending)
        {
            return new SortComparer(_comparison, ascending);
        }

        #endregion

        /// <summary>
        /// Default comparison used when TColumn is IComparable
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int ValueComparsion(TItem x, TItem y)
        {
            object valueX = GetValue(x);
            object valueY = GetValue(y);
            if (valueX == null)
            {
                if (valueY == null)
                    return 0;
                else
                    return -1;
            }
            else if (valueY == null)
            {
                return 1;
            }

            return ((IComparable)valueX).CompareTo(valueY);
        }

        /// <summary>
        /// Default comparison used when TColumn is not IComparable (in which case, sorting is not possible)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int NopComparison(TItem x, TItem y)
        {
            return 0;
        }
   }
}
