using System.Collections.Generic;
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Service contract modelling a Dicom Hierarchical Study Root Query.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IStudyRootQuery" , Namespace = QueryNamespace.Value)]
	public interface IStudyRootQuery
	{
		/// <summary>
		/// Performs a STUDY level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria);

		/// <summary>
		/// Performs a SERIES level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria);

		/// <summary>
		/// Performs an IMAGE level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria);
	}
}
