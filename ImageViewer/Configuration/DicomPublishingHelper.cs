﻿#region License

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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using System.Linq;
using ClearCanvas.ImageViewer.Common.StudyManagement;

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
        public IDicomServiceNode SourceServer { get; set; }

		/// <summary>
		/// Gets or sets the remote server AE from which the study originated.
		/// </summary>
		/// <remarks>
		/// In most cases, this is the server identified by the <see cref="DicomTags.SourceApplicationEntityTitle"/> attribute in the file's metadata.
		/// </remarks>
		public IDicomServiceNode OriginServer { get; set; }

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
		/// Files are published to one or more of the servers identified by the <see cref="SourceServer"/> and <see cref="OriginServer"/> properties,
		/// and the <see cref="DefaultServers"/> depending on the current user configuration.
		/// </remarks>
		/// <returns>True if the files were published to all relevant destinations successfully; False if publishing to one or more destinations failed.</returns>
		public bool Publish()
		{
		    // TODO (CR Mar 2012): Method's getting a bit long now.

			var filesByStudyUid = Files.GroupBy(f => f.DataSet[DicomTags.StudyInstanceUid].ToString());
			var hasErrors = false;

            // generate a list of files to be published
            var localFiles = new List<DicomFile>();
            foreach (var studyFiles in filesByStudyUid)
            {
                try
                {
                    // files are only published to the local store if it possesses the parent study
                    if (StudyExistsOnLocal(studyFiles.Key))
                        localFiles.AddRange(studyFiles);
                }
                catch (Exception e)
                {
                    //Consider failure to query an error because we don't know whether or not the study exists.
                    Platform.Log(LogLevel.Error, e, "Unable to determine if study '{0}' exists locally.", studyFiles.Key);
                    hasErrors = true;
                }
            }

            // publish local files now
            if (localFiles.Count > 0 && !PublishFilesToLocal(localFiles))
                hasErrors = true;

			// check that user has permissions to publish to remote servers
			if (PermissionsHelper.IsInRole(AuthorityTokens.Publishing))
			{
				var servers = new List<IDicomServiceNode>();

				// if configured to publish to the original server, resolve the origin and add to list of destinations
				if (DicomPublishingSettings.Default.SendLocalToStudySourceAE)
				{
                    if (OriginServer != null && !OriginServer.IsLocal && !ContainsServer(servers, OriginServer))
                        servers.Add(OriginServer);
				}

				// resolve the source server and add to list of destinations
                if (SourceServer != null && !SourceServer.IsLocal && !ContainsServer(servers, SourceServer))
                    servers.Add(SourceServer);

				// enumerate the list of destination servers
				foreach (var server in servers)
				{
					// generate a list of files to be published
					var remoteFiles = new List<DicomFile>();
					foreach (var studyFiles in filesByStudyUid)
					{
                        try
                        {
                            // files are only published to servers which possess the parent study
                            if (StudyExistsOnRemote(server, studyFiles.Key))
                                remoteFiles.AddRange(studyFiles);
                        }
                        catch (Exception e)
                        {
                            //Consider failure to query an error because we don't know whether or not the study exists.
                            Platform.Log(LogLevel.Error, e, "Unable to determine if study '{0}' exists on server '{1}'.", studyFiles.Key, server.AETitle);
                            hasErrors = true;
                        }
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

			_hasErrors = hasErrors;
			return !hasErrors;
		}

        private static bool ContainsServer(IEnumerable<IDicomServiceNode> servers, IDicomServiceNode server)
		{
			return server != null && servers.Any(s => s.Name == server.Name);
		}

		private static bool StudyExistsOnLocal(string studyInstanceUid)
		{
		    using (var bridge = new StudyStoreBridge())
		        return bridge.QueryByStudyInstanceUid(studyInstanceUid).Count > 0;
		}

        private static bool StudyExistsOnRemote(IDicomServiceNode server, string studyInstanceUid)
		{
            IList<StudyRootStudyIdentifier> result = null;
            server.GetService<IStudyRootQuery>(s => result = 
                s.StudyQuery(new StudyRootStudyIdentifier {StudyInstanceUid = studyInstanceUid}));
			return result.Count > 0;
		}

		private static bool PublishFilesToLocal(ICollection<DicomFile> files)
		{
			try
			{
			    Platform.GetService((IPublishFiles service) => service.PublishLocal(files));
				return true;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occurred while attempting to publish files locally.");
			}
			return false;
		}

        private static bool PublishFilesToRemote(IDicomServiceNode destination, ICollection<DicomFile> files)
		{
			try
			{
                Platform.GetService((IPublishFiles service) => service.PublishRemote(files,destination));
				return true;
			}
            catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occurred while attempting to publish files to server {0}.", destination.AETitle);
			}
			return false;
		}
	}
}