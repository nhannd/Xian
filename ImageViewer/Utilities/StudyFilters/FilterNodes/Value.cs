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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes
{
	public abstract class Value : FilterNodeBase
	{
		protected readonly StudyFilterColumn Column;

		protected Value(StudyFilterColumn column)
		{
			this.Column = column;
		}
	}

	public sealed class ValueEquals : Value
	{
		private readonly string _compare;

		public ValueEquals(StudyFilterColumn column, string compare) : base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).Equals(_compare);
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueGreater : Value
	{
		private readonly string _compare;

		public ValueGreater(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetText(item).CompareTo(_compare) > 0;
		}

		public override string ToString()
		{
			return string.Format("{0} > {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueLesser : Value
	{
		private readonly string _compare;

		public ValueLesser(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetText(item).CompareTo(_compare) < 0;
		}

		public override string ToString()
		{
			return string.Format("{0} < {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueGreaterOrEqual : Value
	{
		private readonly string _compare;

		public ValueGreaterOrEqual(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetText(item).CompareTo(_compare) >= 0;
		}

		public override string ToString()
		{
			return string.Format("{0} >= {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueLesserOrEqual : Value
	{
		private readonly string _compare;

		public ValueLesserOrEqual(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetText(item).CompareTo(_compare) <= 0;
		}

		public override string ToString()
		{
			return string.Format("{0} <= {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueStartsWith : Value
	{
		private readonly string _compare;

		public ValueStartsWith(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetText(item).StartsWith(_compare);
		}

		public override string ToString()
		{
			return string.Format("{0} STARTS WITH {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueEndsWith : Value
	{
		private readonly string _compare;

		public ValueEndsWith(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetText(item).EndsWith(_compare);
		}

		public override string ToString()
		{
			return string.Format("{0} ENDS WITH {1}", Column.ToString(), _compare);
		}
	}
}