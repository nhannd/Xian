using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer
{
	//NOTE: keep this internal for now, as I'm not too sure of their usefulness right now.

	internal interface IImageSetFactory
	{
		void SetStudyTree(StudyTree studyTree);

		IImageSet CreateImageSet(Study study);
	}

	internal abstract class ImageSetFactory : IImageSetFactory
	{
		private StudyTree _studyTree;
		private readonly IDisplaySetFactory _displaySetFactory;

		public ImageSetFactory()
			: this(new BasicDisplaySetFactory())
		{
		}

		public ImageSetFactory(IDisplaySetFactory displaySetFactory)
		{
			_displaySetFactory = displaySetFactory;
		}

		#region IImageSetFactory Members

		void IImageSetFactory.SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			_displaySetFactory.SetStudyTree(studyTree);
		}

		IImageSet IImageSetFactory.CreateImageSet(Study study)
		{
			Platform.CheckForNullReference(study, "study");
			Platform.CheckMemberIsSet(_studyTree, "_studyTree");

			return CreateImageSet(study);
		}

		#endregion

		protected virtual IImageSet CreateImageSet(Study study)
		{
			ImageSet imageSet = null;
			List<IDisplaySet> displaySets = CreateDisplaySets(study);

			if (displaySets.Count > 0)
			{
				imageSet = new ImageSet(CreateImageSetDescriptor(study.GetIdentifier()));
				
				foreach (IDisplaySet displaySet in displaySets)
					imageSet.DisplaySets.Add(displaySet);
			}

			return imageSet;
		}

		protected virtual List<IDisplaySet> CreateDisplaySets(Study study)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			foreach (Series series in study.Series)
				displaySets.AddRange(_displaySetFactory.CreateDisplaySets(series));

			return displaySets;
		}

		protected virtual DicomImageSetDescriptor CreateImageSetDescriptor(IStudyRootStudyIdentifier studyIdentifier)
		{
			return new DicomImageSetDescriptor(studyIdentifier);
		}
	}
}