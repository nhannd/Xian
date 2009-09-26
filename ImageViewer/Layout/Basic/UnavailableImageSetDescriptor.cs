using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal class UnavailableImageSetDescriptor : DicomImageSetDescriptor
	{
		internal UnavailableImageSetDescriptor(StudyItem studyItem, Exception loadStudyError)
			: base(studyItem)
		{
			LoadStudyError = loadStudyError;
		}

		public readonly Exception LoadStudyError;

		public new StudyItem SourceStudy
		{
			get { return (StudyItem)base.SourceStudy; }
		}

		protected override string GetName()
		{
			string serverName;
			if (SourceStudy.Server == null)
				serverName = SR.LabelUnknownServer;
			else
				serverName = SourceStudy.Server.ToString();

			return String.Format("({0}) {1}", serverName, base.GetName());
		}

		public bool IsOffline
		{
			get { return LoadStudyError is OfflineLoadStudyException; }
		}

		public bool IsNearline
		{
			get { return LoadStudyError is NearlineLoadStudyException; }
		}

		public bool IsInUse
		{
			get { return LoadStudyError is InUseLoadStudyException; }
		}

		public bool IsNotLoadable
		{
			get { return LoadStudyError is StudyLoaderNotFoundException; }
		}
	}
}