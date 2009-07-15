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

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    /// <summary>
    /// Loads the compressed study header stream.
    /// </summary>
    internal class HeaderLoader
    {
        private readonly HeaderLoaderStatistics _statistics = new HeaderLoaderStatistics();
        private readonly string _partitionAE;
        private readonly string _studyInstanceUid;
        private StudyStorageLocation _studyLocation;
    	private string _faultDescription;

        #region Constructor

        public HeaderLoader(HeaderStreamingContext context)
        {
            _studyInstanceUid = context.Parameters.StudyInstanceUID;
            _partitionAE = context.Parameters.ServerAETitle;
			_statistics.FindStudyFolder.Start();
            string sessionId = context.CallerAE;
            
            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(_partitionAE);

            StudyStorageLoader storageLoader = new StudyStorageLoader(sessionId);
            storageLoader.CacheEnabled = ImageStreamingServerSettings.Default.EnableCache;
            storageLoader.CacheRetentionTime = ImageStreamingServerSettings.Default.CacheRetentionWindow;
            StudyLocation = storageLoader.Find(_studyInstanceUid, partition);

            if (StudyLocation != null && StudyLocation.QueueStudyStateEnum != QueueStudyStateEnum.Idle)
            {
                Platform.Log(LogLevel.Warn, "Accessing to study {0} while its current state is {1}",
                             StudyLocation.StudyInstanceUid,  StudyLocation.QueueStudyStateEnum);
            }

            _statistics.FindStudyFolder.End();
        }

        #endregion

        #region Public Properties

        public HeaderLoaderStatistics Statistics
        {
            get { return _statistics; }
        }

        public bool StudyExists
        {
            get { return StudyLocation != null; }
        }

        public StudyStorageLocation StudyLocation
        {
            get { return _studyLocation; }
            set { _studyLocation = value; }
		}

    	public string FaultDescription
    	{
			get { return _faultDescription; }
			set { _faultDescription = value; }    		
    	}

		#endregion

		#region Public Methods
		/// <summary>
		/// Loads the compressed header stream for the study with the specified study instance uid
		/// </summary>
		/// <returns>
		/// The compressed study header stream or null if the study doesn't exist.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public Stream Load()
		{
			if (!StudyExists)
				return null;

			_statistics.LoadHeaderStream.Start();
			String studyPath = StudyLocation.GetStudyPath();
			if (!Directory.Exists(studyPath))
			{
				// the study exist in the database but not on the filesystem.

				// TODO: If the study is migrated to another tier and the study folder is removed, 
				// we may want to do something here instead of throwing exception.
				_statistics.LoadHeaderStream.End();
				throw new ApplicationException(String.Format("Study Folder {0} doesn't exist", studyPath));
			}

			String compressedHeaderFile = Path.Combine(studyPath, _studyInstanceUid + ".xml.gz");
            Stream headerStream = null;
			Platform.Log(LogLevel.Debug, "Study Header Path={0}", compressedHeaderFile);
			try
			{
			    headerStream = FileStreamOpener.OpenForRead(compressedHeaderFile, FileMode.Open, 30000 /* try for 30 seconds */);
			}
            catch(FileNotFoundException)
            {
                throw;
            }
            catch(IOException ex)
            {
                // treated as sharing violation
                throw new StudyAccessException("Study header is not accessible at this time.", StudyLocation.QueueStudyStateEnum, ex);
            }
			_statistics.LoadHeaderStream.End();
            _statistics.Size = (ulong)headerStream.Length;
            return headerStream;
		}

    	#endregion Public Methods
    }
}