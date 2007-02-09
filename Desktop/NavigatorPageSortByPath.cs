using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
	public class NavigatorPageSortByPath : IComparer<NavigatorPage>
	{
		#region IComparer<NavigatorPage> Members

		public int Compare(NavigatorPage x, NavigatorPage y)
		{
			if (x == null)
			{
				if (y == null)
					return 0;
				else
					return -1;
			}

			if (y == null)
				return 1;

			return x.Path.LocalizedPath.CompareTo(y.Path.LocalizedPath);
		}

		#endregion
	}
}
