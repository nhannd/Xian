using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public interface IDisplaySetFactory
	{
		void SetStudyTree(StudyTree studyTree);

		List<IDisplaySet> CreateDisplaySets(Series series);
	}

	public abstract class DisplaySetFactory : IDisplaySetFactory
	{
		private StudyTree _studyTree;
		private readonly IPresentationImageFactory _presentationImageFactory;

		protected DisplaySetFactory()
			: this(new PresentationImageFactory())
		{
		}

		protected DisplaySetFactory(IPresentationImageFactory presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
			_presentationImageFactory = presentationImageFactory;
		}

		protected StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		protected IPresentationImageFactory PresentationImageFactory
		{
			get { return _presentationImageFactory; }	
		}

		#region IDisplaySetFactory Members

		public void SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			PresentationImageFactory.SetStudyTree(_studyTree);
		}

		public abstract List<IDisplaySet> CreateDisplaySets(Series series);

		#endregion
	}
}