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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Helper class for publishing DICOM files to update existing studies using the configured publishing settings.
	/// </summary>
	/// <remarks>
	/// This class identifies potential destination servers depending on the current configuration
	/// and source/origin servers identified by calling code. Instances are only published to servers
	/// that possess the parent study.
	/// </remarks>
	public class DicomPublishingHelper
	{
		private readonly IList<DicomFile> _files = new List<DicomFile>();
		private bool _hasErrors = false;

		/// <summary>
		/// Gets the list of files to be published.
		/// </summary>
		public IList<DicomFile> Files
		{
			get { return _files; }
		}

		/// <summary>
		/// Gets or sets the source server AE from which the study was opened.
		/// </summary>
		/// <remarks>
		/// If the subject study is streamed from an ImageServer, the source AE would be the streaming server.
		/// If the subject study is on a remote PACS server, the source AE would be the remote server.
		/// </remarks>
		public string SourceServerAE { get; set; }

		/// <summary>
		/// Gets or sets the remote server AE from which the study originated.
		/// </summary>
		/// <remarks>
		/// In most cases, this is the value of the <see cref="DicomTags.SourceApplicationEntityTitle"/> attribute in the file's metadata.
		/// </remarks>
		public string OriginServerAE { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the previous call to <see cref="Publish"/> files resulted in one or more errors.
		/// </summary>
		public bool HasErrors
		{
			get { return _hasErrors; }
		}

		/// <summary>
		/// Publishes the specified files.
		/// </summary>
		/// <remarks>
		/// Files are published to one or more of the servers identified by the <see cref="SourceServerAE"/> and <see cref="OriginServerAE"/> properties,
		/// and the <see cref="DefaultServers"/> depending on the current user configuration.
		/// </remarks>
		/// <returns>True if the files were published to all relevant destinations successfully; False if publishing to one or more destinations failed.</returns>
		public bool Publish()
		{
			var studyFiles = GetFilesByStudy(Files);
			var hasErrors = false;

			// check that user has permissions to publish to remote servers
			if (PermissionsHelper.IsInRole(AuthorityTokens.Publishing))
			{
				var servers = new List<Server>();

				// if configured to publish to default servers, add those to list of destinations
				if (PublishingSettings.Default.PublishToDefaultServers)
				{
					foreach (var defaultServer in DefaultServers.GetAll())
					{
						if (defaultServer != null && !ContainsServer(servers, defaultServer))
							servers.Add(defaultServer);
					}
				}

				// if configured to publish to the original server, resolve the origin and add to list of destinations
				if (PublishingSettings.Default.PublishLocalToSourceAE)
				{
					var originServer = ResolveRemoteServer(OriginServerAE);
					if (originServer != null && !ContainsServer(servers, originServer))
						servers.Add(originServer);
				}

				// resolve the source server and add to list of destinations
				var sourceServer = ResolveRemoteServer(SourceServerAE);
				if (sourceServer != null && !ContainsServer(servers, sourceServer))
					servers.Add(sourceServer);

				// enumerate the list of destination servers
				foreach (var server in servers)
				{
					// generate a list of files to be published
					var remoteFiles = new List<DicomFile>();
					foreach (var study in studyFiles)
					{
						// files are only published to servers which possess the parent study
						if (StudyExistsOnRemote(server, study.Key))
							remoteFiles.AddRange(study.Value);
					}

					// publish remote files now
					if (remoteFiles.Count > 0 && !PublishFilesToRemote(server, remoteFiles))
						hasErrors = true;
				}
			}
			else
			{
				Platform.Log(LogLevel.Debug, "Skipping remote publishing step; user does not have Publishing permissions.");
			}

			// generate a list of files to be published
			var localFiles = new List<DicomFile>();
			foreach (var study in studyFiles)
			{
				// files are only published to the local store if it possesses the parent study
				if (StudyExistsOnLocal(study.Key))
					localFiles.AddRange(study.Value);
			}

			// publish local files now
			if (localFiles.Count > 0 && !PublishFilesToLocal(localFiles))
				hasErrors = true;

			_hasErrors = hasErrors;
			return !hasErrors;
		}

		private static bool ContainsServer(IEnumerable<Server> servers, Server server)
		{
			return server != null && CollectionUtils.Contains(servers, s => s.Path == server.Path);
		}

		private static Server ResolveRemoteServer(string aetitle)
		{
			if (string.IsNullOrEmpty(aetitle))
				return null;

			Services.ServerTree.ServerTree tree = new Services.ServerTree.ServerTree();
			foreach (var node in tree.FindChildServers(tree.RootNode.ServerGroupNode))
			{
				var server = node as Server;
				if (server != null && !server.IsLocalDataStore && server.AETitle == aetitle) // yes, AETitle is case sensitive
					return server;
			}
			return null;
		}

		private static IDictionary<string, IList<DicomFile>> GetFilesByStudy(IEnumerable<DicomFile> files)
		{
			var studies = new Dictionary<string, IList<DicomFile>>();
			foreach (var file in files)
			{
				if (file == null)
					continue;

				var studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
				if (!studies.ContainsKey(studyInstanceUid))
					studies.Add(studyInstanceUid, new List<DicomFile>());

				var studyFiles = studies[studyInstanceUid];
				if (!studyFiles.Contains(file))
					studyFiles.Add(file);
			}
			return studies;
		}

		private static bool StudyExistsOnLocal(string studyInstanceUid)
		{
			try
			{
				var srq = (IStudyRootQuery) new LocalStudyRootQueryExtensionPoint().CreateExtension();
				var result = srq.StudyQuery(new StudyRootStudyIdentifier {StudyInstanceUid = studyInstanceUid});
				return result.Count > 0;
			}
			catch (Exception)
			{
			    /// TODO (CR Nov 2011): This could be a valid error condition that the user should be notified of.
				return false;
			}
		}

		private static bool StudyExistsOnRemote(Server server, string studyInstanceUid)
		{
			try
			{
				var srq = new DicomStudyRootQuery(Services.ServerTree.ServerTree.GetClientAETitle(), server.AETitle, server.Host, server.Port);
				var result = srq.StudyQuery(new StudyRootStudyIdentifier {StudyInstanceUid = studyInstanceUid});
				return result.Count > 0;
			}
			catch (Exception)
			{
                /// TODO (CR Nov 2011): This could be a valid error condition that the user should be notified of.
				return false;
			}
		}

		private static bool PublishFilesToLocal(ICollection<DicomFile> files)
		{
			try
			{
				DicomFilePublisher.PublishLocal(files, true);
				return true;
			}
			catch (DicomFilePublishingException ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occurred while attempting to publish files locally.");
			}
			return false;
		}

		private static bool PublishFilesToRemote(Server server, ICollection<DicomFile> files)
		{
			try
			{
				var destination = new AEInformation();
				destination.AETitle = server.AETitle;
				destination.HostName = server.Host;
				destination.Port = server.Port;

				DicomFilePublisher.PublishRemote(files, destination, true);
				return true;
			}
			catch (DicomFilePublishingException ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occurred while attempting to publish files to server {0}.", server.AETitle);
			}
			return false;
		}
	}
}