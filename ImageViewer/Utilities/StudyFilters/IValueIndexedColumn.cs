using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IValueIndexedColumn
	{
		IEnumerable UniqueValues { get; }
	}

	public interface IValueIndexedColumn<T> : IValueIndexedColumn
	{
		new IEnumerable<T> UniqueValues { get; }
	}
}