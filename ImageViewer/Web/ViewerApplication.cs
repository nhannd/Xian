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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Web.Common.Messages;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Events;
using ClearCanvas.Web.Services;
using ClearCanvas.ImageViewer.Web.Common;
using ClearCanvas.ImageViewer.Web.EntityHandlers;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using Application=ClearCanvas.Desktop.Application;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Web
{
	//TODO (CR May 2010): resource strings.

	[ExtensionOf(typeof(ExceptionTranslatorExtensionPoint))]
	internal class ExceptionTranslator : IExceptionTranslator
	{
		private const string _messageStudyInUse = 
			"The study is being processed by the server and cannot be opened at this time.  Please try again later.";
		
		private const string _messageStudyOffline = 
			"The study cannot be opened because it is offline.\n"  +
			"Please contact your PACS administrator to restore the study.";
		
		private const string _messageStudyNearline = 
			"The study cannot be opened because it is nearline.\n" +
            "Please contact your PACS administrator to restore the study.";

		private const string _messageStudyNotFound = 
			"The study could not be found.";
		
		private const string _messagePatientStudiesNotFound = 
			"No studies could be found for the specified patient.";

		private const string _messageAccessionStudiesNotFound = 
			"Studies matching the specified accession number could not be found.";

		private const string _messageStudyCouldNotBeLoaded =
			"The study could not be loaded.";

		private const string _messageNoImages =
			"No images could be loaded for display.";

		#region IExceptionTranslator Members

		public string Translate(Exception e)
		{
			if (e.GetType().Equals(typeof(InUseLoadStudyException)))
				return _messageStudyInUse;
			if (e.GetType().Equals(typeof(NearlineLoadStudyException)))
				return _messageStudyNearline;
			if (e.GetType().Equals(typeof(OfflineLoadStudyException)))
				return _messageStudyOffline;
			if (e.GetType().Equals(typeof(NotFoundLoadStudyException)))
				return _messageStudyNotFound;
			if (e.GetType().Equals(typeof(PatientStudiesNotFoundException)))
				return _messagePatientStudiesNotFound;
			if (e.GetType().Equals(typeof(AccessionStudiesNotFoundException)))
				return _messageAccessionStudiesNotFound;
			if (e.GetType().Equals(typeof(InvalidRequestException)))
				return e.Message;
			if (e is LoadMultipleStudiesException)
				return ((LoadMultipleStudiesException)e).GetUserMessage();
			if (e.GetType().Equals(typeof(LoadStudyException)))
				return _messageStudyCouldNotBeLoaded;
			if (e.GetType().Equals(typeof(NoVisibleDisplaySetsException)))
				return _messageNoImages;
			//if (e.GetType().Equals(typeof(StudyLoaderNotFoundException)))

			return null;
		}

		#endregion
	}

	internal class PatientStudiesNotFoundException : Exception
	{
		public PatientStudiesNotFoundException(string patientId)
			:base(String.Format("Studies for patient '{0}' could not be found.", patientId))
		{
		}
	}

	internal class AccessionStudiesNotFoundException : Exception
	{
		public AccessionStudiesNotFoundException(string accession)
			: base(String.Format("Studies matching accession '{0}' could not be found.", accession))
		{
		}
	}

	internal class InvalidRequestException : Exception
	{
		public InvalidRequestException()
			: base("The request must contain at least one valid study instance uid, patient id, or accession#.")
		{
		}
	}

	[Application(typeof(StartViewerApplicationRequest))]
	[ExtensionOf(typeof(ApplicationExtensionPoint))]
	public class ViewerApplication : ClearCanvas.Web.Services.Application
	{
        private static readonly object _syncLock = new object();
		private Common.ViewerApplication _app;
		private ImageViewerComponent _viewer;
		private EntityHandler _viewerHandler;

		private static IList<StudyRootStudyIdentifier> FindStudies(StartViewerApplicationRequest request)
		{
			bool invalidRequest = true;
			List<StudyRootStudyIdentifier> results = new List<StudyRootStudyIdentifier>();

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				if (request.StudyInstanceUid != null && request.StudyInstanceUid.Length > 0)
				{
					foreach (string studyUid in request.StudyInstanceUid)
					{
						//TODO (CR May 2010): can actually trigger a query of all studies
						if (!String.IsNullOrEmpty(studyUid))
							invalidRequest = false;

                        //TODO (CR May 2010): if request.AeTitle is set, assign RetrieveAeTitle parameter in 
                        // StudyRootStudyIndentifer to this value.  Update the query code to then only
                        // search this specified partition and remove the loop code below that looks
                        // for matching AeTitles.
						StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier
						                                          {
						                                              StudyInstanceUid = studyUid
						                                          };
					    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);

					    bool found = false;
						foreach (StudyRootStudyIdentifier study in studies)
						{
						    if (!string.IsNullOrEmpty(request.AeTitle) && !study.RetrieveAeTitle.Equals(request.AeTitle)) continue;

						    results.Add(study);
						    found = true;
						}

                        if (!found)
                            throw new NotFoundLoadStudyException(studyUid);
                    }
				}
				if (request.PatientId != null && request.PatientId.Length > 0)
				{
					foreach (string patientId in request.PatientId)
					{
						if (!String.IsNullOrEmpty(patientId))
							invalidRequest = false;

						StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier
						                                          {
						                                              PatientId = patientId
						                                          };

					    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
					    bool found = false;
						foreach (StudyRootStudyIdentifier study in studies)
						{
						    if (!string.IsNullOrEmpty(request.AeTitle) && !study.RetrieveAeTitle.Equals(request.AeTitle)) continue;

						    results.Add(study);
						    found = true;
						}

                        if (!found)
                            throw new PatientStudiesNotFoundException(patientId);
                    }
				}
				if (request.AccessionNumber != null && request.AccessionNumber.Length > 0)
				{
					foreach (string accession in request.AccessionNumber)
					{
						if (!String.IsNullOrEmpty(accession))
							invalidRequest = false;

						StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier
						                                          {
						                                              AccessionNumber = accession
						                                          };

					    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);

					    bool found = false;
						foreach (StudyRootStudyIdentifier study in studies)
						{
						    if (!string.IsNullOrEmpty(request.AeTitle) && !study.RetrieveAeTitle.Equals(request.AeTitle)) continue;

						    results.Add(study);
						    found = true;
						}

                        if (!found)
                            throw new AccessionStudiesNotFoundException(accession);
                    }
				}
			}

			if (invalidRequest)
				throw new InvalidRequestException();

			return results;
		}

		private static bool AnySopsLoaded(IImageViewer imageViewer)
		{
			foreach (Patient patient in imageViewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					foreach (Series series in study.Series)
					{
						foreach (Sop sop in series.Sops)
						{
							return true;
						}
					}
				}
			}

			return false;
		}


	    protected override EventSet OnGetPendingOutboundEvent(int wait)
	    {
            if (_context == null)
            {
                string reason = string.Format("Application context no longer exists");
                throw new Exception(reason);
            }

            return _context.GetPendingOutboundEvent(wait);
	    }

	    protected override ProcessMessagesResult OnProcessMessageEnd(MessageSet messageSet, bool messageWasProcessed)
	    {
            if (!messageWasProcessed)
            {
                return new ProcessMessagesResult { EventSet = null, Pending = true };
            }
            
            bool hasMouseMoveMsg = false;
            foreach (Message m in messageSet.Messages)
            {
                if (m is MouseMoveMessage)
                {
                    hasMouseMoveMsg = true;
                    break;
                }
            }
            EventSet ev = GetPendingOutboundEvent(hasMouseMoveMsg ? 100 : 0);

            return new ProcessMessagesResult { EventSet = ev, Pending = false };
	    }

	    protected override void OnStart(StartApplicationRequest request)
		{
			lock (_syncLock)
			{
                Platform.Log(LogLevel.Info, "Viewer Application is starting...");
                if (Application.Instance == null)
					Platform.StartApp();
			}

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                Platform.Log(LogLevel.Debug, "Finding studies...");
			var startRequest = (StartViewerApplicationRequest)request;
			IList<StudyRootStudyIdentifier> studies = FindStudies(startRequest);

			List<LoadStudyArgs> loadArgs = CollectionUtils.Map(studies, (StudyRootStudyIdentifier identifier) => CreateLoadStudyArgs(identifier));

		    DesktopWindowCreationArgs args = new DesktopWindowCreationArgs("", Identifier.ToString());
            WebDesktopWindow window = new WebDesktopWindow(args, Application.Instance);
            window.Open();

			_viewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);

			try
			{
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Loading study...");
                _viewer.LoadStudies(loadArgs);
			}
			catch (Exception e)
			{
				if (!AnySopsLoaded(_viewer)) //end the app.
                    throw;

				//Show an error and continue.
				ExceptionHandler.Report(e, window);
			}

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                Platform.Log(LogLevel.Info, "Launching viewer...");
			
			ImageViewerComponent.Launch(_viewer, window, "");

			_viewerHandler = EntityHandler.Create<ViewerEntityHandler>();
			_viewerHandler.SetModelObject(_viewer);
		    _app = new Common.ViewerApplication
		               {
		                   Identifier = Identifier,
		                   Viewer = (Viewer) _viewerHandler.GetEntity(),

                           VersionString = String.Format("{0} [{1}]", 
                                ProductInformation.GetNameAndVersion(false, true),
                                String.Format("{0}.{1}.{2}.{3}", 
                                    ProductInformation.Version.Major, ProductInformation.Version.Minor, 
                                    ProductInformation.Version.Build, ProductInformation.Version.Revision))
			           };
            

            // Push the ViewerApplication object to the client
            Event @event = new PropertyChangedEvent
            {
                PropertyName = "Application",
                Value = _app,
                Identifier = Guid.NewGuid(),
                SenderId = request.Identifier
            };

            ApplicationContext.Current.FireEvent(@event);
		}

	    private LoadStudyArgs CreateLoadStudyArgs(StudyRootStudyIdentifier identifier)
	    {

            // TODO: Need to think about this more. What's the best way to swap different loader?
            // Do we need to support loading studies from multiple servers? 

	        if (WebViewerServices.Default.StudyLoaderName.Equals("CC_STREAMING"))
	        {
	            string host = WebViewerServices.Default.ArchiveServerHostname;
	            int port = WebViewerServices.Default.ArchiveServerPort;

	            int headerPort = WebViewerServices.Default.ArchiveServerHeaderPort;
	            int wadoPort = WebViewerServices.Default.ArchiveServerWADOPort;

	            var serverAe = new StudyManagement.ApplicationEntity(host,
	                                                                 identifier.RetrieveAeTitle,
	                                                                 identifier.RetrieveAeTitle, port, true,
	                                                                 headerPort, wadoPort);

	            return new LoadStudyArgs(identifier.StudyInstanceUid, serverAe, WebViewerServices.Default.StudyLoaderName);
	        }
	        else
	        {
	            throw new NotSupportedException("Only streaming study loader is supported at this time");
	        }
	    }

	    protected override void OnStop()
		{
			if (_viewerHandler != null)
			{
				_viewerHandler.Dispose();
				_viewerHandler = null;
			}

			//TODO (CR May 2010): WebDesktopWindow shouldn't hang around, but we can check.
			if (_viewer != null)
			{
				_viewer.Stop();
				_viewer.Dispose();
				_viewer = null;
			}
		}

		protected override ClearCanvas.Web.Common.Application GetContractObject()
		{
			return _app;
		}
	}

    [ExtensionOf(typeof(SettingsStoreExtensionPoint))]
    public class StandardSettingsProvider : ISettingsStore
    {
        public bool SupportsImport
        {
            get { return false; }
        }

        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            return new List<SettingsGroupDescriptor>();
        }

        public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group)
        {
            return null;
        }

        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            return new List<SettingsPropertyDescriptor>();
        }

        public void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            throw new NotSupportedException();
        }

        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            return new Dictionary<string, string>();
        }

        public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
        {

        }

        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            //throw new NotSupportedException();
        }
    }
}
