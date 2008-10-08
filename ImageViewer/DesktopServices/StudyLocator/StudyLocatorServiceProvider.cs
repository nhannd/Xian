using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.StudyLocator;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class StudyLocatorServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IStudyLocator))
				return new StudyLocatorServiceClient();

			return null;
		}

		#endregion
	}

	internal class StudyLocatorServiceClient : ClientBase<IStudyLocator>, IStudyLocator
	{
		public StudyLocatorServiceClient()
		{
		}

		#region IStudyLocator Members

		public IList<StudyRootStudyIdentifier> FindByStudyInstanceUid(string[] studyInstanceUids)
		{
			return base.Channel.FindByStudyInstanceUid(studyInstanceUids);
		}

		public IList<StudyRootStudyIdentifier> FindByAccessionNumber(string accessionNumber)
		{
			return base.Channel.FindByAccessionNumber(accessionNumber);
		}

		#endregion

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryQriteria)
		{
			return base.Channel.StudyQuery(queryQriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return base.Channel.SeriesQuery(queryQriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria)
		{
			return base.Channel.ImageQuery(queryQriteria);
		}

		#endregion
	}
}
