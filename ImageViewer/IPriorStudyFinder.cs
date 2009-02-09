using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	public interface IPriorStudyFinder
	{
		void SetImageViewer(IImageViewer viewer);

		StudyItemList FindPriorStudies();

		void Cancel();
	}
}
