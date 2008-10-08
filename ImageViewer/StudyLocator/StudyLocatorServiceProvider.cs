using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyLocator
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class StudyLocatorServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IStudyRootQuery))
				return new StudyLocatorQueryClient();

			return null;
		}

		#endregion
	}

	internal class StudyLocatorQueryClient : ClientBase<IStudyRootQuery>, IStudyRootQuery
	{
		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryQriteria)
		{
			return Channel.StudyQuery(queryQriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return Channel.SeriesQuery(queryQriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria)
		{
			return Channel.ImageQuery(queryQriteria);
		}

		#endregion
	}
}
