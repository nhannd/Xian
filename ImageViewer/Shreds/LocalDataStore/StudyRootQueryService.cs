using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, Namespace = ClearCanvas.Dicom.ServiceModel.Query.QueryNamespace.Value)]
	public class StudyRootQueryService : IStudyRootQuery
	{
		public StudyRootQueryService()
		{
		}

		#region IStudyRootQuery Members

		IList<StudyRootStudyIdentifier> IStudyRootQuery.StudyQuery(StudyRootStudyIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		IList<SeriesIdentifier> IStudyRootQuery.SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		IList<ImageIdentifier> IStudyRootQuery.ImageQuery(ImageIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		#endregion
	}
}
