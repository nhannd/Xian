using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public interface IImageSetFactory
	{
		void SetStudyTree(StudyTree studyTree);

		IImageSet CreateImageSet(Study study);
	}

	public abstract class ImageSetFactory : IImageSetFactory
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
			IImageSet imageSet = null;
			List<IDisplaySet> displaySets = CreateDisplaySets(study);

			if (displaySets.Count > 0)
			{
				imageSet = CreateImageSet(study.GetStudyItem());

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

		protected virtual ImageSet CreateImageSet(IStudyRootData study)
		{
			return CreateImageSet<ImageSet>(study);
		}

		public static ImageSet CreateImageSet<T>(IStudyRootData study) where T : ImageSet, new()
		{
			ImageSet imageSet = new T();

			DateTime studyDate;
			DateParser.Parse(study.StudyDate, out studyDate);
			DateTime studyTime;
			TimeParser.Parse(study.StudyTime, out studyTime);

			string modalitiesInStudy = StringUtilities.Combine(study.ModalitiesInStudy, ", ");

			imageSet.Name = String.Format("{0} {1} [{2}] {3}",
			                              studyDate.ToString(Format.DateFormat),
			                              studyTime.ToString(Format.TimeFormat),
			                              modalitiesInStudy ?? "",
			                              study.StudyDescription);

			imageSet.PatientInfo = String.Format("{0} · {1}",
			                                     new PersonName(study.PatientsName).FormattedName,
			                                     study.PatientId);

			imageSet.Uid = study.StudyInstanceUid;

			return imageSet;
		}
	}
}