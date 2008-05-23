using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public partial class StackTool
	{
		public interface ISortMenuItem
		{
			string Name { get; }

			string Description { get; }

			IComparer<IPresentationImage> Comparer { get; }
		}

		public class SortMenuItem : ISortMenuItem
		{
			private readonly string _name;
			private readonly string _description;
			private readonly IComparer<IPresentationImage> _comparer;

			public SortMenuItem(string name, string description, IComparer<IPresentationImage> comparer)
			{
				Platform.CheckForEmptyString(name, "name");
				Platform.CheckForEmptyString(description, "description");
				Platform.CheckForNullReference(comparer, "comparer");

				_name = name;
				_description = description;
				_comparer = comparer;
			}

			#region ISortMenuItem Members

			public string Name
			{
				get { return _name; }
			}

			public string Description
			{
				get { return _description; }
			}

			public IComparer<IPresentationImage> Comparer
			{
				get { return _comparer; }
			}

			#endregion
		}
	}
}