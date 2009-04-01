using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes
{
	public sealed class Not : FilterNodeBase
	{
		private readonly FilterNodeBase _operand;

		public Not( FilterNodeBase operand)
		{
			_operand = operand;
		}

		public override bool Evaluate(StudyItem item)
		{
			return !_operand.Evaluate(item);
		}
	}
}