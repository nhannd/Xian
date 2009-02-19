using System.Collections.Generic;
using System;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class DisplaySetNumberComparer : DisplaySetComparer
	{
		public DisplaySetNumberComparer()
		{
		}

		public DisplaySetNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		protected IEnumerable<IComparable> GetCompareValues(DisplaySet displaySet)
		{
			yield return displaySet.Number;
			yield return displaySet.Name;
		}

		public override int Compare(IDisplaySet x, IDisplaySet y)
		{
			if (x == y)
				return 0; //same reference object

			DisplaySet displaySet1 = x as DisplaySet;
			DisplaySet displaySet2 = y as DisplaySet;
			
			if (ReferenceEquals(displaySet1, displaySet2))
				return 0; //same object or both are null

			//at this point, at least one of x or y is non-null and they are not the same object

			if (displaySet1 == null)
				return -ReturnValue; // x > y (because we want x at the end for non-reverse sorting)
			if (displaySet2 == null)
				return ReturnValue; // x < y (because we want y at the end for non-reverse sorting)

			return base.Compare(GetCompareValues(displaySet1), GetCompareValues(displaySet2));
		}
	}
}
