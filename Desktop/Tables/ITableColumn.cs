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
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines a column in an <see cref="ITable"/>.
    /// </summary>
    public interface ITableColumn
    {
        /// <summary>
        /// The name or heading of the column.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of data that the column holds.
        /// </summary>
        Type ColumnType { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this column is visible.
		/// </summary>
		bool Visible { get; }

        /// <summary>
        /// Gets or sets a resource resolver.
        /// </summary>
        IResourceResolver ResourceResolver { get; }
        
        /// <summary>
		/// Occurs when the <see cref="Visible"/> property has changed.
		/// </summary>
		event EventHandler VisibleChanged;

        /// <summary>
        /// A factor that influences the width of the column relative to other columns.
        /// </summary>
        /// <remarks>
		/// A value of 1.0 is default.
        /// </remarks>
        float WidthFactor { get; }

        /// <summary>
        /// Gets the width of this column as a percentage of the overall table width.
        /// </summary>
        int WidthPercent { get; }

        /// <summary>
        /// Indicates whether this column is read-only.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Indicates whether this column is clickable.
        /// </summary>
        bool HasClickableLink { get; }

		/// <summary>
		/// Gets the tooltip of this column for the specified item.
		/// </summary>
		string GetTooltipText(object item);

        /// <summary>
        /// Gets the value of this column for the specified item.
        /// </summary>
        /// <param name="item">The item from which the value is to be obtained</param>
        object GetValue(object item);

        /// <summary>
        /// Sets the value of this column on the specified item, assuming this is not a read-only column.
        /// </summary>
        /// <param name="item">The item on which the value is to be set.</param>
        /// <param name="value">The value.</param>
        void SetValue(object item, object value);

		/// <summary>
		/// Format the value of this column for the specified item.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The formatted value</returns>
    	object FormatValue(object value);

        /// <summary>
        /// Sets the click action of this column on the specified item.
        /// </summary>
        /// <param name="item">The item on which the value is to be set.</param>
        void ClickLink(object item);

        /// <summary>
        /// Get a comparer that can be used to sort items in the specified direction.
        /// </summary>
        IComparer GetComparer(bool ascending);

        /// <summary>
        /// Gets the cell row for which this column will be displayed in.
        /// </summary>
        int CellRow { get; }
    }
}
