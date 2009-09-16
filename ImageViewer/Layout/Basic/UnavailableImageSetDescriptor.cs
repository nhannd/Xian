using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class UnavailableImageSetDescriptor : DicomImageSetDescriptor
	{
		internal UnavailableImageSetDescriptor(StudyItem studyItem, Exception loadStudyError)
			: base(studyItem)
		{
			LoadStudyError = loadStudyError;

			string serverName;
			if (studyItem.Server == null)
				serverName = SR.LabelUnknownServer;
			else
				serverName = studyItem.Server.ToString();

			base.Name = String.Format("({0}) {1}", serverName, base.Name);
		}

		public readonly Exception LoadStudyError;

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