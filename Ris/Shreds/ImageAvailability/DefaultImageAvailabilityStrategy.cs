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
	/// The strategy queries DICOM server for a list of studies with a given accession number.
    /// The NumberOfStudyRelatedInstances for all studies are added together.
    /// The NumberOfSeriesRelatedInstances from all the DicomSeries in an order are also added as a different 
	/// sum.  The ImageAvailability of all procedures in the order are updated based on the comparison of these two sums.
	/// </summary>
	public class DefaultImageAvailabilityStrategy : IImageAvailabilityStrategy
	{
        private readonly ImageAvailabilityShredSettings _settings;

        public DefaultImageAvailabilityStrategy()
        {
            _settings = new ImageAvailabilityShredSettings();
        }

		#region IImageAvailabilityStrategy Members

		public Healthcare.ImageAvailability ComputeProcedureImageAvailability(Procedure procedure, IReadContext context)
		{
			// Find the number of instances recorded in the DicomSeries
			bool hasIncompleteDicomSeries;
			int numberOfInstancesFromDocumentation = QueryDocumentation(procedure.Order, out hasIncompleteDicomSeries);

			if (hasIncompleteDicomSeries)
			{
                return Healthcare.ImageAvailability.N;
			}
			else
			{
				bool studiesNotFound;
				int numberOfInstancesFromDicomServer;

                numberOfInstancesFromDicomServer = QueryDicomServer(procedure.Order,
                    _settings.DicomCallingAETitle,
                    _settings.DicomServerAETitle,
                    _settings.DicomServerHost,
                    _settings.DicomServerPort,
                    out studiesNotFound);

				// Compare recorded result with the result from Dicom Query 
				if (studiesNotFound || numberOfInstancesFromDicomServer == 0)
                {
                    return Healthcare.ImageAvailability.Z;
                }
                else if (numberOfInstancesFromDicomServer < numberOfInstancesFromDocumentation)
                {
                    return Healthcare.ImageAvailability.P;
                }
                else if (numberOfInstancesFromDicomServer == numberOfInstancesFromDocumentation)
                {
                    return Healthcare.ImageAvailability.C;
                }
                else if (numberOfInstancesFromDicomServer > numberOfInstancesFromDocumentation)
                {
                    // there are more images on the PACS than were recorded
                    // by the tech - perhaps documentation is incomplete
                    // consider this an 'indeterminate' scenario
                    return Healthcare.ImageAvailability.N;
                }
                else
                {
                    return Healthcare.ImageAvailability.N;
                }
			}
		}

		#endregion

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
									delegate(PerformedStep ps) { return ps.Is<ModalityPerformedProcedureStep>(); });

								if (mppsList.Count == 0)
								{
									isMissingDicomSeries = true;
								}
								else
								{
									CollectionUtils.ForEach(mps.PerformedSteps,
										delegate(PerformedStep ps)
										{
											if (ps.Is<ModalityPerformedProcedureStep>())
											{
												ModalityPerformedProcedureStep mpps = ps.As<ModalityPerformedProcedureStep>();
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
					String message = String.Format(SR.MessageFormatDicomRemoteServerCancelledFind,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}
				if (scu.Status == ScuOperationStatus.ConnectFailed)
				{
					String message = String.Format(SR.MessageFormatDicomConnectionFailed,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}
				if (scu.Status == ScuOperationStatus.Failed)
				{
					String message = String.Format(SR.MessageFormatDicomQueryOperationFailed,
					                               scu.FailureDescription ?? "no failure description provided");
					throw new DicomException(message);
				}
				if (scu.Status == ScuOperationStatus.TimeoutExpired)
				{
					String message = String.Format(SR.MessageFormatDicomConnectTimeoutExpired,
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
