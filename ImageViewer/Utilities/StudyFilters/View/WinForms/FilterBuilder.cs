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