using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, Namespace = ClearCanvas.Dicom.ServiceModel.Query.QueryNamespace.Value)]
	public class PatientRootQueryService : IPatientRootQuery
	{
		public PatientRootQueryService()
		{
		}

		#region IPatientRootQuery Members

		public IList<PatientRootPatientIdentifier> PatientQuery(PatientRootPatientIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		public IList<PatientRootStudyIdentifier> StudyQuery(PatientRootStudyIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria)
		{
			return QueryHelper.Query(queryQriteria);
		}

		#endregion
	}
}
