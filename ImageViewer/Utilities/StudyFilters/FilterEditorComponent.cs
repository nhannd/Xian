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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class FilterEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(FilterEditorComponentViewExtensionPoint))]
	public class FilterEditorComponent : ApplicationComponent, IStudyFilterColumnCollectionCallbacks
	{
		private readonly StudyFilterColumnCollection _availableColumns;
		private readonly List<FilterNodeBase> _filters;

		public FilterEditorComponent()
		{
			_availableColumns = new StudyFilterColumnCollection(this);
			_filters = new List<FilterNodeBase>();
		}

		public FilterEditorComponent(IEnumerable<StudyFilterColumn> columns, IEnumerable<FilterNodeBase> filters)
			: this()
		{
			foreach (StudyFilterColumn column in columns)
			{
				_availableColumns.Add(column);
			}

			_filters.AddRange(filters);
		}

		public StudyFilterColumnCollection AvailableColumns
		{
			get { return _availableColumns; }
		}

		public IList<FilterNodeBase > Filters
		{
			get { return _filters; }
		}

		#region IStudyFilterColumnCollectionCallbacks Members

		void IStudyFilterColumnCollectionCallbacks.ColumnInserted(int index, StudyFilterColumn newColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnRemoved(int index, StudyFilterColumn oldColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn) {}
		void IStudyFilterColumnCollectionCallbacks.ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns) {}

		#endregion
	}
}