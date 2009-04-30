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
using ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public sealed class FilterBuilder
	{
		public static IEnumerable<object> ListOperators()
		{
			yield return new EqualsOperator();
			yield return new GTOperator();
			yield return new LTOperator();
			yield return new GEOperator();
			yield return new LEOperator();
			yield return new StartsOperator();
			yield return new EndsOperator();
		}

		public static FilterNodeBase BuildFilterNode(StudyFilterColumn column, object @operator, string value)
		{
			Operator op = (Operator) @operator;
			return op.Build(column, value);
		}

		private abstract class Operator
		{
			public abstract FilterNodeBase Build(StudyFilterColumn column, string value);
		}

		private class EqualsOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueEquals(column, value);
			}

			public override string ToString()
			{
				return "=";
			}
		}

		private class GTOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueGreater(column, value);
			}

			public override string ToString()
			{
				return ">";
			}
		}

		private class LTOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueLesser(column, value);
			}

			public override string ToString()
			{
				return "<";
			}
		}

		private class GEOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueGreaterOrEqual(column, value);
			}

			public override string ToString()
			{
				return ">=";
			}
		}

		private class LEOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueLesserOrEqual(column, value);
			}

			public override string ToString()
			{
				return "<=";
			}
		}

		private class StartsOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueStartsWith(column, value);
			}

			public override string ToString()
			{
				return "STARTS WITH";
			}
		}

		private class EndsOperator : Operator
		{
			public override FilterNodeBase Build(StudyFilterColumn column, string value)
			{
				return new ValueEndsWith(column, value);
			}

			public override string ToString()
			{
				return "ENDS WITH";
			}
		}
	}
}