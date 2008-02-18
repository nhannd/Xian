#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval
{
    /// <summary>
    /// Loads the compressed study header stream.
    /// </summary>
    internal class HeaderLoader
    {
        #region Private Members

        private Settings _settings = Settings.Default;
        private Stream _compressedHeaderStream = null;
        private string _partitionAE;
        private string _studyInstanceUid;
        private string _studyHeaderFilePath;
        private string _compressedStudyHeaderFilePath;
        private string _studyContainerFolder;
        private HeaderLoaderStatistics _statistics = new HeaderLoaderStatistics();

        #endregion Private Members

        #region Protected Properties

        protected string StudyHeaderFilePath
        {
            get
            {
                if (String.IsNullOrEmpty(_studyHeaderFilePath))
                {
                    _studyHeaderFilePath = String.Format("{0}/{1}.xml", StudyContainerFolder, StudyInstanceUid);
                }

                return _studyHeaderFilePath;

            }
            set
            {
                _studyHeaderFilePath = value;
            }

        }

        protected string CompressedStudyHeaderFilePath
        {
            get
            {
                if (String.IsNullOrEmpty(_studyContainerFolder))
                {
                    _compressedStudyHeaderFilePath = String.Format("{0}/{1}.xml.gz", StudyContainerFolder, StudyInstanceUid);
                }

                return _compressedStudyHeaderFilePath;

            }
            set
            {
                _studyHeaderFilePath = value;
            }

        }

        protected string StudyContainerFolder
        {
            get
            {
                if (String.IsNullOrEmpty(_studyContainerFolder))
                {
                    _studyContainerFolder = ResolveStudyContainerFolder(PartitionAE, StudyInstanceUid);
                }

                return _studyContainerFolder;

            }
            set
            {
                _studyContainerFolder = value;
            }

        }

        protected string PartitionAE
        {
            get { return _partitionAE; }
            set { _partitionAE = value; }
        }

        protected string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        #endregion Protected Properties

        #region Private methods


        private void LoadCompressedHeaderStream()
        {
            _statistics.LoadHeaderStream.Start();
            FileInfo fi = new FileInfo(CompressedStudyHeaderFilePath);
            _compressedHeaderStream = new FileStream(CompressedStudyHeaderFilePath, FileMode.Open);
            _statistics.LoadHeaderStream.End();
            _statistics.Size = (ulong) fi.Length;

        }

        private string ResolveStudyContainerFolder(string partitionAETitle, string studyInstanceUid)
        {
            _statistics.FindStudyFolder.Start();

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                IServerPartitionEntityBroker partitionBroker = ctx.GetBroker<IServerPartitionEntityBroker>();
                ServerPartitionSelectCriteria partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(partitionAETitle);
                IList<ServerPartition> partitions = partitionBroker.Find(partitionCriteria);

                if (partitions != null && partitions.Count > 0)
                {
                    ServerPartition partition = partitions[0];

                    IStudyStorageEntityBroker broker = ctx.GetBroker<IStudyStorageEntityBroker>();
                    StudyStorageSelectCriteria storageCriteria = new StudyStorageSelectCriteria();
                    storageCriteria.StudyInstanceUid.EqualTo(studyInstanceUid);
                    storageCriteria.ServerPartitionKey.EqualTo(partition.GetKey());
                    IList<StudyStorage> storages = broker.Find(storageCriteria);

                    if (storages != null && storages.Count > 0)
                    {
                        StudyStorage storage = storages[0];

                        IStorageFilesystemEntityBroker storageFilesystemBroker =
                            ctx.GetBroker<IStorageFilesystemEntityBroker>();
                        StorageFilesystemSelectCriteria storageFilesystemCriteria = new StorageFilesystemSelectCriteria();
                        storageFilesystemCriteria.StudyStorageKey.EqualTo(storage.GetKey());

                        IList<StorageFilesystem> storageFS = storageFilesystemBroker.Find(storageFilesystemCriteria);

                        if (storageFS != null && storageFS.Count > 0)
                        {
                            IFilesystemEntityBroker filesystemBroker = ctx.GetBroker<IFilesystemEntityBroker>();
                            Filesystem fs = filesystemBroker.Load(storageFS[0].FilesystemKey);
                            string folder =
                                String.Format("{0}/{1}/{2}/{3}", fs.FilesystemPath, partition.PartitionFolder, storageFS[0].StudyFolder,
                                              studyInstanceUid);

                            _statistics.FindStudyFolder.End();

                            return folder;
                        }


                    }
                }



            }

            return null;
        }

        private void GenerateCompressedHeader()
        {
            Platform.Log(LogLevel.Info, "GenerateCompressedHeader: compressing header file...");

            _statistics.CompressHeader.Start();
            string tempFilePath = StudyHeaderFilePath + ".tmp";

            if (File.Exists(tempFilePath))
            {
                // either another service instance/thread is using this file to generate the compressed header or
                // the server crashed previously.
                try
                {
                    // try to delete it. 
                    File.Delete(tempFilePath);
                }
                catch (Exception e)
                {
                    // we can't delete the temp file, that mean it is being used for compression by another thread.
                    Platform.Log(LogLevel.Info, "GenerateCompressedHeader: Unable to create temp header file : "  + e.Message);

                    // this thread should fail
                    return;
                }
            }


            File.Copy(StudyHeaderFilePath, tempFilePath);

            FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Open);

            FileStream compressedHeaderStream = new FileStream(CompressedStudyHeaderFilePath, FileMode.OpenOrCreate);

            GZipStream zipStream = new GZipStream(compressedHeaderStream, CompressionMode.Compress, true);

            byte[] buffer = new byte[_settings.ReadBufferSize];
            int size;
            do
            {
                size = tempFileStream.Read(buffer, 0, buffer.Length);
                if (size > 0)
                {
                    zipStream.Write(buffer, 0, size);
                }
            } while (size > 0);

            zipStream.Close();
            tempFileStream.Close();
            File.Delete(tempFilePath);
            compressedHeaderStream.Close();


            _statistics.CompressHeader.End();

            Platform.Log(LogLevel.Info, "GenerateCompressedHeader: compressed header file created");
        }

        #endregion

        #region Public Properties

        public HeaderLoaderStatistics Statistics
        {
            get { return _statistics; }
        }

        public Stream CompressedHeaderStream
        {
            get { return _compressedHeaderStream; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the compressed header stream for the study with the specified study instance uid
        /// </summary>
        /// <param name="partitionAETitle">The server partition where the study is located</param>
        /// <param name="studyInstanceUid">The study instance uid</param>
        /// <returns></returns>
        /// <remarks>
        /// The compressed header is created on the fly if it doesn't exist.
        /// </remarks>
        public Stream Load(string partitionAETitle, string studyInstanceUid)
        {
            PartitionAE = partitionAETitle;
            StudyInstanceUid = studyInstanceUid;

            if (!File.Exists(CompressedStudyHeaderFilePath))
            {
                GenerateCompressedHeader();
            }
            else
            {
                DateTime t1 = File.GetLastWriteTimeUtc(StudyHeaderFilePath);
                DateTime t2 = File.GetLastWriteTimeUtc(CompressedStudyHeaderFilePath);

                if (t1 > t2)
                {
                    // The compressed header file is out of date
                    GenerateCompressedHeader();
                }

            }
            LoadCompressedHeaderStream();
            return CompressedHeaderStream;

        }


        #endregion Public Methods

        
    }
}
