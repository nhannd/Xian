using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions
{
	public interface IFilterMenuActionOwner
	{
		StudyFilterColumn Column { get; }
		CompositeFilterPredicate ParentFilterPredicate { get; }
	}
}