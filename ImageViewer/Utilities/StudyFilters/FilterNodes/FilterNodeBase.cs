namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes
{
	public abstract class FilterNodeBase
	{
		public abstract bool Evaluate(StudyItem item);
	}
}