using System.Collections.Generic;
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//TODO: add Request/Reply contracts?
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IStudyRootQuery" , Namespace = QueryNamespace.Value)]
	public interface IStudyRootQuery
	{
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria);
	}
}
