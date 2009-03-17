using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates;
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
		//TODO: change to use something like KeyObjectContentItem instead of KVP?
		private List<KeyValuePair<Frame,DicomSoftcopyPresentationState>> _sourceFrames;

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
				if (_sourceFrames == null)
				{
					_sourceFrames = new List<KeyValuePair<Frame, DicomSoftcopyPresentationState>>();
					foreach (IClipboardItem item in _sourceInformation.ClipboardItems)
					{
						IImageSopProvider provider = item.Item as IImageSopProvider;
						if (provider != null)
						{
							DicomSoftcopyPresentationState presentationState = null;
							if (item.Item is IPresentationImage && DicomSoftcopyPresentationState.IsSupported((IPresentationImage)item.Item))
								presentationState = DicomSoftcopyPresentationState.Create((IPresentationImage) item.Item);
							_sourceFrames.Add(new KeyValuePair<Frame, DicomSoftcopyPresentationState>(provider.Frame, presentationState));
						}
					}
				}

				return _sourceFrames;
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
					_defaultServers = Services.Configuration.DefaultServers.GetAll();

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

			foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> frameAndPR in SourceFrames)
				serializer.Frames.Add(frameAndPR);

			_keyObjectDocuments.AddRange(serializer.Serialize());
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

		private void AddRemotePublishDocuments(Server publishServer, Frame frame, DicomFile dcfPresentationState)
		{
			if (!_remotePublishingInfo.ContainsKey(publishServer))
				_remotePublishingInfo.Add(publishServer, new List<DicomFile>());

			List<DicomFile> publishDocuments = _remotePublishingInfo[publishServer];
			DicomFile existingDocument = GetKeyObjectDocument(frame.StudyInstanceUID, publishDocuments);

			if (existingDocument == null)
			{
				DicomFile document = GetKeyObjectDocument(frame.StudyInstanceUID, _keyObjectDocuments);
				publishDocuments.Add(document);
			}

			if (dcfPresentationState != null && !publishDocuments.Contains(dcfPresentationState))
				publishDocuments.Add(dcfPresentationState);
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
					DicomFile dcfPresentationState = null;
					if(frameAndPR.Value!=null)
						dcfPresentationState = frameAndPR.Value.DicomFile;

					foreach (Server defaultServer in DefaultServers)
						AddRemotePublishDocuments(defaultServer, frame, dcfPresentationState);

					ISopDataSource dataSource = frame.ParentImageSop.DataSource;
					ApplicationEntity sourceServer = dataSource.Server as ApplicationEntity;
					if (sourceServer != null)
					{
						Server publishServer = GetServer(sourceServer);
						//if it's a default server, then it was already added above.
						if (publishServer != null && !IsDefaultServer(publishServer))
							AddRemotePublishDocuments(publishServer, frame, dcfPresentationState);
					}
					else if (dataSource.StudyLoaderName == "DICOM_LOCAL")
					{
						DicomFile existingDocument = GetKeyObjectDocument(frame.StudyInstanceUID, _localPublishingInfo);
						if (existingDocument == null)
						{
							DicomFile document = GetKeyObjectDocument(frame.StudyInstanceUID, _keyObjectDocuments);
							_localPublishingInfo.Add(document);
						}

						if (dcfPresentationState != null)
							_localPublishingInfo.Add(dcfPresentationState);
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
			if (PermissionsHelper.IsInRole(ImageViewer.Common.AuthorityTokens.Workflow.Study.Export))
			{
				foreach (KeyValuePair<Server, List<DicomFile>> pair in _remotePublishingInfo)
				{
					try
					{
						List<AEInformation> destinationServers = new List<AEInformation>();
						AEInformation destination = new AEInformation();
						destination.AETitle = pair.Key.AETitle;
						destination.HostName = pair.Key.Host;
						destination.Port = pair.Key.Port;
						destinationServers.Add(destination);

						DicomFilePublisher.PublishRemote(pair.Value, destination, true);
					}
					catch (EndpointNotFoundException)
					{
						remotePublishFailed = true;
						Platform.Log(LogLevel.Error,
						             "Unable to publish key images to default servers; the local dicom server does not appear to be running.");
					}
					catch (Exception e)
					{
						remotePublishFailed = true;
						Platform.Log(LogLevel.Error, e,
						             "An error occurred while attempting to publish key images to server {0}.", pair.Key.AETitle);
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
	}
}
