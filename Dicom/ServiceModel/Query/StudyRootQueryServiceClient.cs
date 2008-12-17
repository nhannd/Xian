using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// WCF client proxy for <see cref="IStudyRootQuery"/> services.
	/// </summary>
	public class StudyRootQueryServiceClient : ClientBase<IStudyRootQuery>, IStudyRootQuery
	{
		/// <summary>
		/// Constructor - uses default configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyRootQueryServiceClient()
		{
		}

		/// <summary>
		/// Constructor - uses input configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyRootQueryServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint and binding.
		/// </summary>
		public StudyRootQueryServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint, loads bindings from given configuration name.
		/// </summary>
		public StudyRootQueryServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
		}

		#region IStudyRootQuery Members

		/// <summary>
		/// Performs a STUDY level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			return base.Channel.StudyQuery(queryCriteria);
		}

		/// <summary>
		/// Performs a SERIES level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			return base.Channel.SeriesQuery(queryCriteria);
		}

		/// <summary>
		/// Performs an IMAGE level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			return base.Channel.ImageQuery(queryCriteria);
		}

		#endregion
	}
}
