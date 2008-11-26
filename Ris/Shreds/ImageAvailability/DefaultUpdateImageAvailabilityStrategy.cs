using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// The default update strategy does not depend on the presence of MPPS from the modality.  The strategy queries DICOM 
	/// server for a list of studies with a given accession number.  The NumberOfStudyRelatedInstances for all studies are
	/// added together.  The NumberOfSeriesRelatedInstances from all the DicomSeries in an order are also added as a different 
	/// sum.  The ImageAvailability of all procedures in the order are updated based on the comparison of these two sums.
	/// </summary>
	public class DefaultUpdateImageAvailabilityStrategy : IUpdateImageAvailabilityStrategy
	{
		private const string ProcedureOIDKey = "ProcedureOID";

		#region IUpdateImageAvailabilityStrategy Members

		public string ScheduledWorkQueueItemType
		{
			get { return "Image Availability"; }
		}

		public WorkQueueItem ScheduleWorkQueueItem(Procedure p, IPersistenceContext context)
		{
			ImageAvailabilitySettings settings = new ImageAvailabilitySettings();

			WorkQueueItem item = new WorkQueueItem(this.ScheduledWorkQueueItemType);
			item.ExpirationTime = DateTime.Now.AddHours(settings.DefaultStrategyExpirationTimeInHours);
			item.ExtendedProperties.Add(ProcedureOIDKey, p.GetRef().Serialize());
			context.Lock(item, DirtyState.New);

			return item;
		}

		public void Update(WorkQueueItem item, IPersistenceContext context)
		{
			ImageAvailabilitySettings settings = new ImageAvailabilitySettings();

			EntityRef procedureRef = new EntityRef(item.ExtendedProperties[ProcedureOIDKey]);
			Procedure procedure = context.Load<Procedure>(procedureRef, EntityLoadFlags.Proxy);

			// Find the number of instances recorded in the DicomSeries
			bool hasIncompleteDicomSeries;
			int numberOfInstancesFromDocumentation = QueryDocumentation(procedure.Order, out hasIncompleteDicomSeries);

			if (hasIncompleteDicomSeries)
			{
				UpdateImageAvailability(procedure.Order, Healthcare.ImageAvailability.N);
			}
			else
			{
				bool studiesNotFound;
				int numberOfInstancesFromDicomServer;

				numberOfInstancesFromDicomServer = QueryDicomServer(procedure.Order,
					settings.CallingAETitle,
					settings.DicomServerAETitle,
					settings.DicomServerHost,
					settings.DicomServerPort,
					out studiesNotFound);
				// Compare recorded result with the result from Dicom Query 
				if (studiesNotFound || numberOfInstancesFromDicomServer == 0)
					UpdateImageAvailability(procedure.Order, Healthcare.ImageAvailability.Z);
				else if (numberOfInstancesFromDicomServer < numberOfInstancesFromDocumentation)
					UpdateImageAvailability(procedure.Order, Healthcare.ImageAvailability.P);
				else if (numberOfInstancesFromDicomServer == numberOfInstancesFromDocumentation)
					UpdateImageAvailability(procedure.Order, Healthcare.ImageAvailability.C);
				else
					UpdateImageAvailability(procedure.Order, Healthcare.ImageAvailability.N);
			}

			// update WorkQueueItem Status and the next ScheduledTime
			switch (procedure.ImageAvailability)
			{
				// ImageAvailability.X should never get pass into this method
				// case Healthcare.ImageAvailability.X:
				//     break;
				case Healthcare.ImageAvailability.N:
					item.ScheduledTime = DateTime.Now.AddMinutes(settings.DefaultStrategyNextScheduledTimeForUnknownAvailabilityInMinutes);
					break;
				case Healthcare.ImageAvailability.Z:
					item.ScheduledTime = DateTime.Now.AddMinutes(settings.DefaultStrategyNextScheduledTimeForZeroAvailabilityInMinutes);
					break;
				case Healthcare.ImageAvailability.P:
					item.ScheduledTime = DateTime.Now.AddMinutes(settings.DefaultStrategyNextScheduledTimeForPartialAvailabilityInMinutes);
					break;
				case Healthcare.ImageAvailability.C:
					item.Status = WorkQueueStatus.CM;
					break;
				default:
					break;
			}
		}

		#endregion

		private static void UpdateImageAvailability(Order order, Healthcare.ImageAvailability availability)
		{
			// Update image availability of all the procedures in the order
			CollectionUtils.ForEach(order.Procedures,
				delegate(Procedure p)
					{
						p.ImageAvailability = availability;
					});
		}

		private static int QueryDocumentation(Order order, out bool hasIncompleteDicomSeries)
		{
			List<DicomSeries> dicomSeries = new List<DicomSeries>();
			bool isMissingDicomSeries = false;

			// Find all the DicomSeries for this order
			CollectionUtils.ForEach(order.Procedures,
				delegate(Procedure procedure)
					{
						CollectionUtils.ForEach(procedure.ModalityProcedureSteps,
							delegate(ModalityProcedureStep mps)
							{
								List<PerformedStep> mppsList = CollectionUtils.Select(mps.PerformedSteps,
									delegate(PerformedStep ps) { return ps is ModalityPerformedProcedureStep; });

								if (mppsList.Count == 0)
								{
									isMissingDicomSeries = true;
								}
								else
								{
									CollectionUtils.ForEach(mps.PerformedSteps,
										delegate(PerformedStep ps)
										{
											if (ps is ModalityPerformedProcedureStep)
											{
												ModalityPerformedProcedureStep mpps = (ModalityPerformedProcedureStep)ps;
												if (mpps.DicomSeries == null || mpps.DicomSeries.Count == 0)
													isMissingDicomSeries = true;
												else
													dicomSeries.AddRange(mpps.DicomSeries);
											}
										});
								}
							});
					});

			// Sum the number of instances for all DicomSeries
			hasIncompleteDicomSeries = isMissingDicomSeries;
			int numberOfInstancesFromDocumentation = CollectionUtils.Reduce<DicomSeries, int>(
				dicomSeries, 0,
				delegate(DicomSeries series, int totalInstances)
					{
						return totalInstances + series.NumberOfSeriesRelatedInstances;
					});

			return numberOfInstancesFromDocumentation;
		}

		private static int QueryDicomServer(Order order, 
			string shredAETitle,
			string dicomServerAETitle,
			string dicomServerHost,
			int dicomServerPort,
			out bool studiesNotFound)
		{
			DicomAttributeCollection requestCollection = new DicomAttributeCollection();
			requestCollection[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
			requestCollection[DicomTags.StudyInstanceUid].SetStringValue("");
			requestCollection[DicomTags.AccessionNumber].SetStringValue(order.AccessionNumber);
			requestCollection[DicomTags.NumberOfStudyRelatedInstances].SetStringValue("");

			int numberOfInstancesFromDicomServer = 0;
			using (StudyRootFindScu scu = new StudyRootFindScu())
			{
				IList<DicomAttributeCollection> results = scu.Find(
					shredAETitle,
					dicomServerAETitle,
					dicomServerHost,
					dicomServerPort,
					requestCollection);

				// Wait for a response
				scu.Join(new TimeSpan(0, 0, 0, 0, 1000));

				if (scu.Status == ScuOperationStatus.Canceled)
				{
					String message = String.Format(SR.MessageFormatRemoteServerCancelledFind,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}
				if (scu.Status == ScuOperationStatus.ConnectFailed)
				{
					String message = String.Format(SR.MessageFormatConnectionFailed,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}
				if (scu.Status == ScuOperationStatus.Failed)
				{
					String message = String.Format(SR.MessageFormatQueryOperationFailed,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}
				if (scu.Status == ScuOperationStatus.TimeoutExpired)
				{
					String message = String.Format(SR.MessageFormatConnectTimeoutExpired,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}

				foreach (DicomAttributeCollection result in results)
				{
					numberOfInstancesFromDicomServer += (int) result[DicomTags.NumberOfStudyRelatedInstances].GetUInt32(0, 0);
				}

				studiesNotFound = results.Count == 0;
			}

			return numberOfInstancesFromDicomServer;
		}
	}
}
