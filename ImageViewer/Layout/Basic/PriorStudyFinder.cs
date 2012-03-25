#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerTree;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExceptionPolicyFor(typeof (LoadPriorStudiesException))]
	[ExtensionOf(typeof (ExceptionPolicyExtensionPoint))]
	public class PriorStudyLoaderExceptionPolicy : IExceptionPolicy
	{
        private static readonly object _lock = new object();

        private static bool _everShownPriorsMessage;
        private static bool _successfulQuerySinceLastPriorsMessage;
        private static bool _mostRecentQuerySuccessful = true;
        
        #region IExceptionPolicy Members

		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (e is LoadPriorStudiesException)
			{
				exceptionHandlingContext.Log(LogLevel.Error, e);

				Handle(e as LoadPriorStudiesException, exceptionHandlingContext);
			}
		}

		#endregion

#if UNIT_TESTS

        internal static void Reset()
        {
            lock (_lock)
            {
                _everShownPriorsMessage = false;
                _successfulQuerySinceLastPriorsMessage = false;
                _mostRecentQuerySuccessful = true;
            }
        }
#endif

        internal static void NotifySuccessfulQuery()
        {
            lock (_lock)
            {
                _mostRecentQuerySuccessful = true;
                if (_everShownPriorsMessage)
                    _successfulQuerySinceLastPriorsMessage = true;
            }
        }

        internal static void NotifyFailedQuery()
        {
            lock (_lock)
            {
                _mostRecentQuerySuccessful = false;
            }
        }
        
        private static void Handle(LoadPriorStudiesException exception, IExceptionHandlingContext context)
		{
			if (exception.FindFailed)
			{
                lock (_lock)
                {
                    if (!_mostRecentQuerySuccessful && (!_everShownPriorsMessage || _successfulQuerySinceLastPriorsMessage))
                    {
                        context.ShowMessageBox(SR.MessageSearchForPriorsFailed);
                        _everShownPriorsMessage = true;
                        _successfulQuerySinceLastPriorsMessage = false;
                    }
                }

			    return;
			}

            var summary = new StringBuilder();
            if (!exception.FindResultsComplete)
            {
                lock (_lock)
                {
                    if (!_mostRecentQuerySuccessful && (!_everShownPriorsMessage || _successfulQuerySinceLastPriorsMessage))
                    {
                        summary.Append(SR.MessagePriorsIncomplete);
                        _everShownPriorsMessage = true;
                        _successfulQuerySinceLastPriorsMessage = false;
                    }
                }
            }

		    if (ShouldShowLoadErrorMessage(exception))
            {
                if (summary.Length > 0)
                {
                    summary.AppendLine(); summary.AppendLine("----"); summary.AppendLine();
                }

                summary.AppendLine(SR.MessageLoadPriorsErrorPrefix);
                summary.Append(exception.GetExceptionSummary());
            }

            if (summary.Length > 0)
                context.ShowMessageBox(summary.ToString());
		}

		private static bool ShouldShowLoadErrorMessage(LoadMultipleStudiesException exception)
		{
			if (exception.IncompleteCount > 0)
				return true;

			if (exception.NotFoundCount > 0)
				return true;

			if (exception.UnknownFailureCount > 0)
				return true;

			return false;
		}
    }

	[ExtensionOf(typeof (PriorStudyFinderExtensionPoint))]
	public class PriorStudyFinder : ImageViewer.PriorStudyFinder
	{
        private volatile bool _cancel;

		public override PriorStudyFinderResult FindPriorStudies()
		{
			_cancel = false;
			var results = new Dictionary<string, StudyItem>();

			IPatientReconciliationStrategy reconciliationStrategy = new DefaultPatientReconciliationStrategy();
			reconciliationStrategy.SetStudyTree(Viewer.StudyTree);

			var patientIds = new Dictionary<string, string>();
			foreach (Patient patient in Viewer.StudyTree.Patients)
			{
				if (_cancel)
					break;

				IPatientData reconciled = reconciliationStrategy.ReconcileSearchCriteria(patient);
				patientIds[reconciled.PatientId] = reconciled.PatientId;
			}

		    int failedCount = 0;
		    int successCount = 0;

		    foreach (var query in DefaultServers.GetQueryInterfaces(true))
		    {
                if (_cancel)
                    break;

		        try
		        {
                    using (var bridge = new StudyRootQueryBridge(query))
                    {
                        foreach (string patientId in patientIds.Keys)
                        {
                            var identifier = new StudyRootStudyIdentifier { PatientId = patientId };

                            IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
                            foreach (StudyRootStudyIdentifier study in studies)
                            {
                                if (_cancel)
                                    break;

                                //Eliminate false positives right away.
                                IPatientData reconciled = reconciliationStrategy.ReconcilePatientInformation(study);
                                if (reconciled == null)
                                    continue;

                                StudyItem studyItem = ConvertToStudyItem(study);
                                if (studyItem == null || results.ContainsKey(studyItem.StudyInstanceUid))
                                    continue;

                                if (!results.ContainsKey(studyItem.StudyInstanceUid))
                                    results[studyItem.StudyInstanceUid] = studyItem;
                            }
                        }
                    }

		            ++successCount;
		        }
		        catch (Exception e)
		        {
		            ++failedCount;
                    Platform.Log(LogLevel.Error, e, "Failed to query server: {0}", query);
		        }
		    }

            if (_cancel)
            {
                //Just pretend the query never happened.
                return new PriorStudyFinderResult(new StudyItemList(), true);
            }

            if (failedCount > 0)
            {
                PriorStudyLoaderExceptionPolicy.NotifyFailedQuery();

                if (successCount == 0)
                    throw new Exception("The search for prior studies has failed.");
            }
            else
            {
                //Even if success count is zero, we'll still consider it "successful".
                PriorStudyLoaderExceptionPolicy.NotifySuccessfulQuery();
            }


            return new PriorStudyFinderResult(new StudyItemList(results.Values), failedCount == 0);
        }

		public override void Cancel()
		{
			_cancel = true;
		}

		private static StudyItem ConvertToStudyItem(IStudyRootStudyIdentifier study)
		{
			string studyLoaderName;
            IApplicationEntity applicationEntity = null;

		    // TODO (Marmot): Change to use the node to determine capabilities.
			var node = FindServer(study.RetrieveAeTitle);
			if (node.IsLocal)
			{
				studyLoaderName = "DICOM_LOCAL";
			}
			else
			{
				studyLoaderName = node.StreamingParameters != null ? "CC_STREAMING" : "DICOM_REMOTE";
			    applicationEntity = node.ToDataContract();
			}

			var item = new StudyItem(study, applicationEntity, studyLoaderName){ InstanceAvailability = study.InstanceAvailability };
			if (String.IsNullOrEmpty(item.InstanceAvailability))
				item.InstanceAvailability = "ONLINE";

			return item;
		}

		private static IDicomServiceNode FindServer(string aeTitle)
		{
            using (var bridge = new ServerDirectoryBridge())
            {
                var local = bridge.GetLocalServer();
                if (local.AETitle == aeTitle)
                    return local;
            }

            using (var bridge = new ServerDirectoryBridge())
                return bridge.GetServersByAETitle(aeTitle).FirstOrDefault();
		}
	}
}
