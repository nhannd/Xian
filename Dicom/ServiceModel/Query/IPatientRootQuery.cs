using System.Collections.Generic;
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = QueryNamespace.Value)]
	public interface IPatientRootQuery
	{
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<PatientRootPatientIdentifier> PatientQuery(PatientRootPatientIdentifier queryQriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<PatientRootStudyIdentifier> StudyQuery(PatientRootStudyIdentifier queryQriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria);
	}
}
