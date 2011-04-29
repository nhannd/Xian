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
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.StudyManagement;
using Patient=ClearCanvas.ImageViewer.StudyManagement.Patient;

namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{
    [ExtensionOf(typeof(PriorStudyFinderExtensionPoint))]
	public class PriorStudyFinder : ImageViewer.PriorStudyFinder
	{
		private volatile bool _cancel;
        private List<IStudyRootQuery> _defaultQueryList = new List<IStudyRootQuery>();
        
        public PriorStudyFinder()
        {
        }

        public override StudyItemList FindPriorStudies()
		{
			_cancel = false;

            _defaultQueryList = new List<IStudyRootQuery>();
            foreach (ServerPartition partition in ServerPartitionMonitor.Instance)
            {
                using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    IDeviceEntityBroker broker = ctx.GetBroker<IDeviceEntityBroker>();
                    DeviceSelectCriteria criteria = new DeviceSelectCriteria();
                    criteria.DeviceTypeEnum.EqualTo(DeviceTypeEnum.PriorsServer);
                    criteria.ServerPartitionKey.EqualTo(partition.Key);
                    IList<Device> list = broker.Find(criteria);
                    foreach (Device theDevice in list)
                    {
                        // Check the settings and log for debug purposes
                        if (!theDevice.Enabled)
                        {
                            Platform.Log(LogLevel.Debug, "Prior Server '{0}' on partition '{1}' is disabled in the device setting",
                                    theDevice.AeTitle, partition.AeTitle);
                            continue;
                        }

                        DicomStudyRootQuery remoteQuery = new DicomStudyRootQuery(partition.AeTitle, theDevice.AeTitle,
                                                                                  theDevice.IpAddress, theDevice.Port);
                        _defaultQueryList.Add(remoteQuery);
                    }

                    // Log the prior servers for debug purpose
                    if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    {
                        if (_defaultQueryList.Count > 0)
                        {
                            StringBuilder log = new StringBuilder();
                            log.Append("Searching for priors on the following servers:");

                            StringBuilder serverList = new StringBuilder();
                            foreach (DicomStudyRootQuery server in _defaultQueryList)
                            {
                                if (serverList.Length > 0)
                                    serverList.Append(",");
                                serverList.AppendFormat("{0}", server);
                            }

                            log.Append(serverList.ToString());
                            Platform.Log(LogLevel.Debug, log.ToString());

                        }
                    }
                }
            }
			StudyItemList results = new StudyItemList();

			DefaultPatientReconciliationStrategy reconciliationStrategy = new DefaultPatientReconciliationStrategy();
			List<string> patientIds = new List<string>();
			foreach (Patient patient in Viewer.StudyTree.Patients)
			{
				if (_cancel)
					break;

				IPatientData reconciled = reconciliationStrategy.ReconcileSearchCriteria(patient);
				if (!patientIds.Contains(reconciled.PatientId))
					patientIds.Add(reconciled.PatientId);
			}

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				foreach (string patientId in patientIds)
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
					identifier.PatientId = patientId;

					IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
					foreach (StudyRootStudyIdentifier study in studies)
					{
						if (_cancel)
							break;

						StudyItem studyItem = ConvertToStudyItem(study);
						if (studyItem != null)
							results.Add(studyItem);
					}
				}
			}

            foreach (IStudyRootQuery query in _defaultQueryList)
            {
                foreach (string patientId in patientIds)
                {

                    try
                    {
                        StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
                        identifier.PatientId = patientId;

                        IList<StudyRootStudyIdentifier> list = query.StudyQuery(identifier);
                        foreach (StudyRootStudyIdentifier i in list)
                        {
                            if (_cancel)
                                break;

                            StudyItem studyItem = ConvertToStudyItem(i);
                            if (studyItem != null)
                                results.Add(studyItem);
                        }
                    }
                    catch (FaultException<DataValidationFault> ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "An error has occurred when searching for prior studies on server '{0}'", query.ToString());

                        // note: ideally we want to continue searching on other servers. But for consistency with the workstation, we throw an exception here
                        throw; 
                    }
                    catch (FaultException<QueryFailedFault> ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "An error has occurred when searching for prior studies on server '{0}'", query.ToString());
                        
                        //note: ideally we want to continue searching on other servers. But for consistency with the workstation, we throw an exception here
                        throw;
                    }
                }
            }
          
			return results;
		}

		public override void Cancel()
		{
			_cancel = true;
		}

		private StudyItem ConvertToStudyItem(StudyRootStudyIdentifier study)
		{
			string studyLoaderName;
			ApplicationEntity applicationEntity;

		    ServerPartition partiton = ServerPartitionMonitor.Instance.GetPartition(study.RetrieveAeTitle);
			if (partiton != null)
			{
                studyLoaderName = WebViewerServices.Default.StudyLoaderName;
                string host = WebViewerServices.Default.ArchiveServerHostname;
                int port = WebViewerServices.Default.ArchiveServerPort;
                int headerPort = WebViewerServices.Default.ArchiveServerHeaderPort;
                int wadoPort = WebViewerServices.Default.ArchiveServerWADOPort;
                applicationEntity = new ApplicationEntity(host, study.RetrieveAeTitle, study.RetrieveAeTitle, port, true, headerPort, wadoPort);

			}
            else
			{
			    Device theDevice = FindServer(study.RetrieveAeTitle);

                if (theDevice != null)
			    {
		            studyLoaderName = "DICOM_REMOTE";

			        applicationEntity = new ApplicationEntity(theDevice.IpAddress, theDevice.AeTitle, theDevice.Description,
			                                                  theDevice.Port,
			                                                  false, 0, 0);
			    }
			    else // (node == null)
			    {
			        Platform.Log(LogLevel.Warn,
			                     String.Format("Unable to find server information '{0}' in order to load study '{1}'",
			                                   study.RetrieveAeTitle, study.StudyInstanceUid));

			        return null;
			    }
			}

		    StudyItem item = new StudyItem(study, applicationEntity, studyLoaderName);
			if (String.IsNullOrEmpty(item.InstanceAvailability))
				item.InstanceAvailability = "ONLINE";

			return item;
		}

		private static Device FindServer(string retrieveAeTitle)
		{
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IDeviceEntityBroker broker = ctx.GetBroker<IDeviceEntityBroker>();
                DeviceSelectCriteria criteria = new DeviceSelectCriteria();
                criteria.AeTitle.EqualTo(retrieveAeTitle);
                IList<Device> list = broker.Find(criteria);
                foreach (Device theDevice in list)
                {
                    return theDevice;
                }
            }

			return null;
		}
	}
}
