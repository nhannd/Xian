using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	public abstract class PriorStudyFinder : IPriorStudyFinder
	{
		private class NullPriorStudyFinder : IPriorStudyFinder
		{
			public NullPriorStudyFinder()
			{
			}

			public StudyItemList FindPriorStudies()
			{
				return new StudyItemList();
			}


			#region IPriorStudyFinder Members

			public void SetImageViewer(IImageViewer viewer)
			{
			}

			public void Cancel()
			{
			}

			#endregion
		}

		public static readonly IPriorStudyFinder Null = new NullPriorStudyFinder();

		private IImageViewer _viewer;

		protected PriorStudyFinder()
		{
		}

		protected IImageViewer Viewer
		{
			get { return _viewer; }	
		}

		#region IPriorStudyFinder Members

		public void SetImageViewer(IImageViewer viewer)
		{
			_viewer = viewer;
		}

		public abstract StudyItemList FindPriorStudies();

		public abstract void Cancel();

		#endregion
	}
}
