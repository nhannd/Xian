using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyLocator
{
	internal abstract class DicomQueryClient : IDisposable
	{
		private readonly string _localAE;
		private readonly string _remoteAE;
		private readonly string _remoteHost;
		private readonly int _remotePort;

		internal DicomQueryClient(string localAE, string remoteAE, string remoteHost, int remotePort)
		{
			Platform.CheckForEmptyString(localAE, "localAE");
			Platform.CheckForEmptyString(remoteAE, "remoteAE");
			Platform.CheckForEmptyString(remoteHost, "remoteHost");
			Platform.CheckArgumentRange(remotePort, 1, 65535, "remotePort");

			_localAE = localAE;
			_remoteAE = remoteAE;
			_remoteHost = remoteHost;
			_remotePort = remotePort;
		}

		protected IList<TIdentifier> Query<TIdentifier, TFindScu>(TIdentifier queryCriteria) where TIdentifier : Identifier, new() where TFindScu : FindScuBase, new()
		{
			if (queryCriteria == null)
			{
				string message = "The query identifier cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IList<DicomAttributeCollection> scuResults;
			using (TFindScu scu = new TFindScu())
			{
				DicomAttributeCollection criteria;

				try
				{
					criteria = queryCriteria.ToDicomAttributeCollection();
				}
				catch(DicomException e)
				{
					DataValidationFault fault = new DataValidationFault();
					fault.Description = "Failed to convert contract object to DicomAttributeCollection.";
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}
				catch (Exception e)
				{
					DataValidationFault fault = new DataValidationFault();
					fault.Description = "Unexpected exception when converting contract object to DicomAttributeCollection.";
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}

				try
				{
					scuResults = scu.Find(_localAE, _remoteAE, _remoteHost, _remotePort, criteria);
					scu.Join();

					if (scu.Status == ScuOperationStatus.Canceled)
					{
						String message = String.Format("The remote server cancelled the query ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.ConnectFailed)
					{
						String message = String.Format("Connection failed ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.Failed)
					{
						String message = String.Format("The query operation failed ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.TimeoutExpired)
					{
						String message = String.Format("The connection timeout expired ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
				}
				catch(FaultException)
				{
					throw;
				}
				catch (Exception e)
				{
					QueryFailedFault fault = new QueryFailedFault();
					fault.Description = String.Format("An unexpected error has occurred ({0})",
												   scu.FailureDescription ?? "no failure description provided");
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
				}
			}

			List<TIdentifier> results = new List<TIdentifier>();
			foreach (DicomAttributeCollection result in scuResults)
			{
				TIdentifier identifier = Identifier.FromDicomAttributeCollection<TIdentifier>(result);
				if (String.IsNullOrEmpty(identifier.RetrieveAeTitle))
					identifier.RetrieveAeTitle = _remoteAE;

				results.Add(identifier);
			}

			return results;
		}

		public override string ToString()
		{
			return _remoteAE;
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}

	internal class StudyRootQueryClient : DicomQueryClient, IStudyRootQuery
	{
		public StudyRootQueryClient(string localAE, string remoteAE, string remoteHost, int remotePort)
			: base(localAE, remoteAE, remoteHost, remotePort)
		{
		}

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryQriteria)
		{
			return Query<StudyRootStudyIdentifier, StudyRootFindScu>(queryQriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return Query<SeriesIdentifier, StudyRootFindScu>(queryQriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria)
		{
			return Query<ImageIdentifier, StudyRootFindScu>(queryQriteria);
		}

		#endregion
	}

	internal class PatientRootQueryClient : DicomQueryClient, IPatientRootQuery
	{
		public PatientRootQueryClient(string localAE, string remoteAE, string remoteHost, int remotePort)
			: base(localAE, remoteAE, remoteHost, remotePort)
		{
		}

		#region IPatientRootQuery Members

		public IList<PatientRootPatientIdentifier>  PatientQuery(PatientRootPatientIdentifier queryQriteria)
		{
			return Query<PatientRootPatientIdentifier, PatientRootFindScu>(queryQriteria);
		}

		public IList<PatientRootStudyIdentifier>  StudyQuery(PatientRootStudyIdentifier queryQriteria)
		{
			return Query<PatientRootStudyIdentifier, PatientRootFindScu>(queryQriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return Query<SeriesIdentifier, PatientRootFindScu>(queryQriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria)
		{
			return Query<ImageIdentifier, PatientRootFindScu>(queryQriteria);
		}

		#endregion
	}
}