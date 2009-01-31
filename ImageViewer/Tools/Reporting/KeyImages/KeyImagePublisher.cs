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
					_defaultServers = Services.Configuration.DefaultServers.GetServers();

				return _defaultServers;
			}	
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

		private Server GetServer(ApplicationEntity server)
		{
			return AllServers.Find(delegate(Server defaultServer)
									{ return server.AETitle == defaultServer.AETitle; });
		}

		private void BuildPublishingInfo()
		{
			foreach (Server server in DefaultServers)
			{
				if (!_remotePublishingInfo.ContainsKey(server))
				{
					_remotePublishingInfo[server] = new List<DicomFile>();
					_remotePublishingInfo[server].AddRange(_keyObjectDocuments);
				}
			}

			if (_publishToSourceServer)
			{
				foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> frameAndPR in SourceFrames)
				{
					Frame frame = frameAndPR.Key;
					DicomFile dcfPresentationState = null;
					if(frameAndPR.Value!=null)
						dcfPresentationState = frameAndPR.Value.AsDicomFile();

					ISopDataSource dataSource = frame.ParentImageSop.DataSource;
					ApplicationEntity sourceServer = dataSource.Server as ApplicationEntity;
					if (sourceServer != null)
					{
						Server publishServer = GetServer(sourceServer);
						if (publishServer != null)
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
							
							if(dcfPresentationState != null)
								publishDocuments.Add(dcfPresentationState);
						}
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
			CreateKeyObjectDocuments();
			BuildPublishingInfo();

			foreach (KeyValuePair<Server, List<DicomFile>> pair in _remotePublishingInfo)
			{
				List<AEInformation> destinationServers = new List<AEInformation>();
				AEInformation destination = new AEInformation();
				destination.AETitle = pair.Key.AETitle;
				destination.HostName = pair.Key.Host;
				destination.Port = pair.Key.Port;
				destinationServers.Add(destination);

				try
				{
					DicomFilePublisher.PublishRemote(pair.Value, destination);
				}
				catch (EndpointNotFoundException)
				{
					Platform.Log(LogLevel.Error, 
						"Unable to publish key images to default servers; the local dicom server does not appear to be running.");
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, 
						"An error occurred while attempting to publish key images to server {0}.", pair.Key.AETitle);
				}
			}

			try
			{
				DicomFilePublisher.PublishLocal(_localPublishingInfo);
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
		}
	}
}
