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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public abstract class SortPredicate : IComparer<StudyItem>
	{
		public readonly StudyFilterColumn Column;

		public SortPredicate(StudyFilterColumn column)
		{
			this.Column = column;
		}

		public override sealed bool Equals(object obj)
		{
			SortPredicate other = obj as SortPredicate;
			if (other != null)
				return this.Column == other.Column && this.GetType() == other.GetType();
			return false;
		}

		public override sealed int GetHashCode()
		{
			return 0x00DBFF0B ^ this.GetType().GetHashCode() ^ this.Column.GetHashCode();
		}

		public virtual int Compare(StudyItem x, StudyItem y)
		{
			return this.Column.Compare(x, y);
		}
	}

	public sealed class AscendingSortPredicate : SortPredicate
	{
		public AscendingSortPredicate(StudyFilterColumn column) : base(column) {}
	}

	public sealed class DescendingSortPredicate : SortPredicate
	{
		public DescendingSortPredicate(StudyFilterColumn column) : base(column) {}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return base.Compare(y, x);
		}
	}
}