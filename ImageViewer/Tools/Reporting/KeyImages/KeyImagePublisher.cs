#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Services;
using System;
using ClearCanvas.Common;
using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal class KeyImagePublisher
	{
		private readonly KeyImageInformation _sourceInformation;
		private readonly bool _publishToSourceServer;

		private readonly List<DicomFile> _keyObjectDocuments;
		private readonly Dictionary<Server, List<DicomFile>> _remotePublishingInfo;
		private readonly List<DicomFile> _localPublishingInfo;

		private List<Server> _defaultServers;
		private List<Server> _allServers;
		private List<KeyValuePair<Frame, DicomSoftcopyPresentationState>> _framePresentationStates;
		private Dictionary<string, SeriesInfo> _seriesIndex;

		public KeyImagePublisher(KeyImageInformation information, bool publishToSourceServer)
		{
			_sourceInformation = information;
			_publishToSourceServer = publishToSourceServer;

			_keyObjectDocuments = new List<DicomFile>();
			_remotePublishingInfo = new Dictionary<Server, List<DicomFile>>();
			_localPublishingInfo = new List<DicomFile>();
		}

		private List<KeyValuePair<Frame, DicomSoftcopyPresentationState>> SourceFrames
		{
			get
			{
				if (_framePresentationStates == null)
				{
					_framePresentationStates = new List<KeyValuePair<Frame, DicomSoftcopyPresentationState>>();
					_seriesIndex = new Dictionary<string, SeriesInfo>();
					foreach (IClipboardItem item in _sourceInformation.ClipboardItems)
					{
						IImageSopProvider provider = item.Item as IImageSopProvider;
						if (provider != null)
						{
							string key = provider.ImageSop.StudyInstanceUid;
							if (!_seriesIndex.ContainsKey(key))
							{
								_seriesIndex.Add(key, new SeriesInfo(provider));
							}

							DicomSoftcopyPresentationState presentationState = null;
							if (item.Item is IPresentationImage && DicomSoftcopyPresentationState.IsSupported((IPresentationImage)item.Item))
							{
								presentationState = DicomSoftcopyPresentationState.Create
									((IPresentationImage) item.Item,
									 delegate(DicomSoftcopyPresentationState ps)
									 	{
									 		ps.PresentationSeriesInstanceUid = _seriesIndex[key].PresentationSeriesUid;
									 		ps.PresentationSeriesNumber = _seriesIndex[key].PresentationSeriesNumber;
									 		ps.PresentationSeriesDateTime = _seriesIndex[key].PresentationSeriesDateTime;
									 		ps.PresentationInstanceNumber = _seriesIndex[key].GetNextPresentationInstanceNumber();
									 		ps.SourceAETitle = DicomServerConfigurationHelper.GetOfflineAETitle(false);
									 	});
							}
							_framePresentationStates.Add(new KeyValuePair<Frame, DicomSoftcopyPresentationState>(provider.Frame, presentationState));
						}
					}
				}

				return _framePresentationStates;
			}
		}

		private List<Server> AllServers
		{
			get
			{
				if (_allServers == null)
				{
					_allServers = new List<Server>();

					ServerTree tree = new ServerTree();
					List<IServerTreeNode> allServers = tree.FindChildServers(tree.RootNode.ServerGroupNode);

					foreach (IServerTreeNode node in allServers)
						_allServers.Add((Server)node);
				}

				return _allServers;
			}
		}

		private List<Server> DefaultServers
		{
			get
			{
				if (_defaultServers == null)
					_defaultServers = ImageViewer.Configuration.DefaultServers.GetAll();

				return _defaultServers;
			}	
		}

		private bool IsDefaultServer(Server publishServer)
		{
			return CollectionUtils.Contains(DefaultServers, delegate(Server server)
																{
																	return server.Path == publishServer.Path;
																});
		}

		private void CreateKeyObjectDocuments()
		{
			KeyImageSerializer serializer = new KeyImageSerializer();
			serializer.Description = _sourceInformation.Description;
			serializer.DocumentTitle = _sourceInformation.DocumentTitle;
			serializer.SeriesDescription = _sourceInformation.SeriesDescription;
			serializer.SourceAETitle = DicomServerConfigurationHelper.GetOfflineAETitle(false);

			foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> frameAndPR in SourceFrames)
				serializer.AddImage(frameAndPR.Key, frameAndPR.Value);

			_keyObjectDocuments.AddRange(serializer.Serialize(
				delegate(KeyImageSerializer.KeyObjectDocumentSeries keyObjectDocumentSeries)
					{
						string key = keyObjectDocumentSeries.StudyInstanceUid;
						if (_seriesIndex.ContainsKey(key))
						{
							keyObjectDocumentSeries.SeriesDateTime = _seriesIndex[key].KeyObjectSeriesDateTime;
							keyObjectDocumentSeries.SeriesNumber = _seriesIndex[key].KeyObjectSeriesNumber;
							keyObjectDocumentSeries.SeriesInstanceUid = _seriesIndex[key].KeyObjectSeriesUid;
						}
					}
				));
		}

		private static DicomFile GetKeyObjectDocument(string studyInstanceUid, List<DicomFile> documents)
		{
			return documents.Find(
				delegate(DicomFile file)
				{
					string fileStudyUid = file.DataSet[DicomTags.StudyInstanceUid];
					return fileStudyUid == studyInstanceUid;
				});
		}

		private void AddRemotePublishDocuments(Server publishServer, Frame frame, DicomFile presentationStateDocument)
		{
			if (!_remotePublishingInfo.ContainsKey(publishServer))
				_remotePublishingInfo.Add(publishServer, new List<DicomFile>());

			List<DicomFile> publishDocuments = _remotePublishingInfo[publishServer];
			DicomFile existingDocument = GetKeyObjectDocument(frame.StudyInstanceUid, publishDocuments);

			if (existingDocument == null)
			{
				DicomFile document = GetKeyObjectDocument(frame.StudyInstanceUid, _keyObjectDocuments);
				publishDocuments.Add(document);
			}

			if (presentationStateDocument != null && !publishDocuments.Contains(presentationStateDocument))
				publishDocuments.Add(presentationStateDocument);
		}

		private Server GetServer(ApplicationEntity server)
		{
			return AllServers.Find(delegate(Server defaultServer)
									{ return server.AETitle == defaultServer.AETitle; });
		}

		private void BuildPublishingInfo()
		{
			if (_publishToSourceServer)
			{
				foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> frameAndPR in SourceFrames)
				{
					Frame frame = frameAndPR.Key;
					DicomSoftcopyPresentationState presentationState = frameAndPR.Value;
					DicomFile presentationStateDocument = null;
					if(presentationState != null)
						presentationStateDocument = presentationState.DicomFile;

					foreach (Server defaultServer in DefaultServers)
						AddRemotePublishDocuments(defaultServer, frame, presentationStateDocument);

					ISopDataSource dataSource = frame.ParentImageSop.DataSource;
					ApplicationEntity sourceServer = dataSource.Server as ApplicationEntity;
					if (sourceServer != null)
					{
						Server publishServer = GetServer(sourceServer);
						//if it's a default server, then it was already added above.
						if (publishServer != null && !IsDefaultServer(publishServer))
							AddRemotePublishDocuments(publishServer, frame, presentationStateDocument);
					}
					else if (dataSource.StudyLoaderName == "DICOM_LOCAL")
					{
						DicomFile existingDocument = GetKeyObjectDocument(frame.StudyInstanceUid, _localPublishingInfo);
						if (existingDocument == null)
						{
							DicomFile document = GetKeyObjectDocument(frame.StudyInstanceUid, _keyObjectDocuments);
							_localPublishingInfo.Add(document);
						}

						if (presentationStateDocument != null)
							_localPublishingInfo.Add(presentationStateDocument);
					}
				}
			}
		}

		public void Publish()
		{
			if (_sourceInformation.ClipboardItems.Count == 0)
				return;

			while(!LocalDataStoreActivityMonitor.IsConnected)
			{
				DialogBoxAction result = Application.ActiveDesktopWindow.ShowMessageBox(
					SR.MessageCannotPublishKeyImagesServersNotRunning, MessageBoxActions.OkCancel);

				if (result == DialogBoxAction.Cancel)
					return;
			}

			CreateKeyObjectDocuments();
			BuildPublishingInfo();

			bool remotePublishFailed = false;
			if (PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
			{
				foreach (KeyValuePair<Server, List<DicomFile>> pair in _remotePublishingInfo)
				{
					EventResult result = EventResult.Success;
					AuditedInstances updatedInstances = new AuditedInstances();

					try
					{
						Server server = pair.Key;
						List<DicomFile> documents = pair.Value;
						List<AEInformation> destinationServers = new List<AEInformation>();
						AEInformation destination = new AEInformation();
						destination.AETitle = server.AETitle;
						destination.HostName = server.Host;
						destination.Port = server.Port;
						destinationServers.Add(destination);

						foreach (DicomFile file in documents)
						{
							string studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
							string patientId = file.DataSet[DicomTags.PatientId].ToString();
							string patientsName = file.DataSet[DicomTags.PatientsName].ToString();
							updatedInstances.AddInstance(patientId, patientsName, studyInstanceUid);
						}

						DicomFilePublisher.PublishRemote(documents, destination, true);
					}
					catch (EndpointNotFoundException)
					{
						result = EventResult.MajorFailure;
						remotePublishFailed = true;
						Platform.Log(LogLevel.Error,
						             "Unable to publish key images to default servers; the local dicom server does not appear to be running.");
					}
					catch (Exception e)
					{
						result = EventResult.SeriousFailure;
						remotePublishFailed = true;
						Platform.Log(LogLevel.Error, e,
						             "An error occurred while attempting to publish key images to server {0}.", pair.Key.AETitle);
					}
					finally
					{
						AuditHelper.LogUpdateInstances(new string[] {pair.Key.AETitle}, updatedInstances, EventSource.CurrentUser, result);
					}
				}
			}
			else
			{
				Platform.Log(LogLevel.Info, "Skipping remote key image publishing step; user does not have Export permissions.");
			}

			bool localPublishFailed = true;
			try
			{
				DicomFilePublisher.PublishLocal(_localPublishingInfo, true);
				localPublishFailed = false;
			}
			catch (EndpointNotFoundException)
			{
				Platform.Log(LogLevel.Error,
					"Unable to publish key images locally; the local data store service does not appear to be running.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e,
						"An error occurred while attempting to publish key images locally.");
			}

			if (localPublishFailed && remotePublishFailed)
				Application.ActiveDesktopWindow.ShowMessageBox(SR.MessagePublishingFailed, MessageBoxActions.Ok);
			else if (localPublishFailed)
				Application.ActiveDesktopWindow.ShowMessageBox(SR.MessageLocalPublishingFailed, MessageBoxActions.Ok);
			else if (remotePublishFailed)
				Application.ActiveDesktopWindow.ShowMessageBox(SR.MessageRemotePublishingFailed, MessageBoxActions.Ok);
		}

		private class SeriesInfo
		{
			public readonly string PresentationSeriesUid;
			public readonly int PresentationSeriesNumber;
			public readonly DateTime PresentationSeriesDateTime;
			public readonly string KeyObjectSeriesUid;
			public readonly int KeyObjectSeriesNumber;
			public readonly DateTime KeyObjectSeriesDateTime;

			private int _presentationNextInstanceNumber;

			public SeriesInfo(IImageSopProvider provider)
			{
				this.KeyObjectSeriesUid = DicomUid.GenerateUid().UID;
				this.KeyObjectSeriesNumber = CalculateSeriesNumber(provider.Frame);
				this.KeyObjectSeriesDateTime = DateTime.Now;
				this.PresentationSeriesUid = DicomUid.GenerateUid().UID;
				this.PresentationSeriesNumber = KeyObjectSeriesNumber + 1;
				this.PresentationSeriesDateTime = DateTime.Now;
				_presentationNextInstanceNumber = 1;
			}

			public int GetNextPresentationInstanceNumber()
			{
				return _presentationNextInstanceNumber++;
			}

			private static int CalculateSeriesNumber(Frame frame)
			{
				if (frame.ParentImageSop == null || frame.ParentImageSop.ParentSeries == null || frame.ParentImageSop.ParentSeries.ParentStudy == null)
					return 1;

				int maxValue = 0;
				foreach (Series series in frame.ParentImageSop.ParentSeries.ParentStudy.Series)
				{
					if (series.SeriesNumber > maxValue)
						maxValue = series.SeriesNumber;
				}

				return maxValue + 1;
			}
		}
	}
}
