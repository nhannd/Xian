using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions
{
	public interface IFilterMenuActionOwner
	{
		StudyFilterColumn Column { get; }
		CompositeFilterPredicate ParentFilterPredicate { get; }
	}
}