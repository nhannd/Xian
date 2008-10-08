using System.Collections.Generic;
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IStudyRootQuery" , Namespace = QueryNamespace.Value)]
	public interface IStudyRootQuery
	{
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryQriteria);

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
