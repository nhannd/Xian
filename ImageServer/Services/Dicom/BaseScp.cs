#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Base class for all DicomScpExtensions for the ImageServer.
    /// </summary>
    public class BaseScp : IDicomScp<DicomScpContext>
    {
        #region Protected Members
        protected IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private ServerPartition _partition;
        private FilesystemMonitor _fsMonitor;
        private FilesystemSelector _selector;
        #endregion

        #region Properties
        protected ServerPartition Partition
        {
            get { return _partition; }
        }

        protected FilesystemMonitor Monitor
        {
            get { return _fsMonitor; }
        }

        protected FilesystemSelector Selector
        {
            get { return _selector; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Helper method to load a <see cref="StudyXml"/> instance for a given study location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            StudyXml theXml = new StudyXml();

            if (File.Exists(streamFile))
            {
                using (Stream fileStream = new FileStream(streamFile, FileMode.Open))
                {
                    XmlDocument theDoc = new XmlDocument();

                    StudyXmlIo.Read(theDoc, fileStream);

                    theXml.SetMemento(theDoc);

                    fileStream.Close();
                }
            }

            return theXml;
        }

        /// <summary>
        /// Retrieves the storage location fromthe database for the specified study.
        /// </summary>
        /// <param name="studyInstanceUid">The Study to check for.</param>
        /// <param name="location">The returned storage location.</param>
        /// <returns>true if a location was found, false otherwise.</returns>
        public bool GetStudyStorageLocation( string studyInstanceUid, out StudyStorageLocation location)
        {
            using (IReadContext read = _store.OpenReadContext())
            {
                IQueryStudyStorageLocation procedure = read.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
                parms.ServerPartitionKey = Partition.GetKey();
                parms.StudyInstanceUid = studyInstanceUid;
                IList<StudyStorageLocation> locationList = procedure.Execute(parms);

                foreach (StudyStorageLocation studyLocation in locationList)
                {
                    if (Monitor.CheckFilesystemReadable(studyLocation.FilesystemKey))
                    {
                        location = studyLocation;
                        return true;
                    }
                }
                location = null;
                return false;
            }
        }

        /// <summary>
        /// Checks for a storage location for the study in the database, and creates a new location
        /// in the database if it doesn't exist.
        /// </summary>
        /// <param name="message">The DICOM message to create the storage location for.</param>
        /// <returns>A <see cref="StudyStorageLocation"/> instance.</returns>
        public StudyStorageLocation GetStudyStorageLocation(DicomMessage message)
        {
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            String studyDate = message.DataSet[DicomTags.StudyDate].GetString(0, Platform.Time.ToString("yyyyMMdd"));

            using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IQueryStudyStorageLocation locQuery = updateContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
                locParms.StudyInstanceUid = studyInstanceUid;
                locParms.ServerPartitionKey = Partition.GetKey();
                IList<StudyStorageLocation> studyLocationList = locQuery.Execute(locParms);

                if (studyLocationList.Count == 0)
                {
                    IInsertStudyStorage locInsert = _store.OpenReadContext().GetBroker<IInsertStudyStorage>();
                    StudyStorageInsertParameters insertParms = new StudyStorageInsertParameters();
                    insertParms.ServerPartitionKey = Partition.GetKey();
                    insertParms.StudyInstanceUid = studyInstanceUid;
                    insertParms.Folder = studyDate;

                    Filesystem filesystem = Selector.SelectFilesystem(message);
                    if (filesystem == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to select location for storing study.");
                        return null;
                    }

                    insertParms.FilesystemKey = filesystem.GetKey();

                    studyLocationList = locInsert.Execute(insertParms);

                    updateContext.Commit();
                }
                else
                {
                    if (!Monitor.CheckFilesystemWriteable(studyLocationList[0].FilesystemKey))
                    {
                        Platform.Log(LogLevel.Warn, "Unable to find writable filesystem for study {0} on Partition {1}",
                                     studyInstanceUid, _partition.Description);
                        return null;
                    }
                }

                //TODO:  Do we need to do something to identify a primary storage location?
                // Also, should the above check for writeable location check the other availab
                return studyLocationList[0];
            }
        }
        #endregion

        #region IDicomScp Members

        public void SetContext(DicomScpContext parms)
        {
            _partition = parms.Partition;
            _fsMonitor = parms.FilesystemMonitor;
            _selector = parms.FilesystemSelector;
        }

        public virtual bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            throw new Exception("The method or operation is not implemented.  The method must be overriden.");
        }

        public virtual IList<SupportedSop> GetSupportedSopClasses()
        {
            throw new Exception("The method or operation is not implemented.  The method must be overriden.");
        }

        #endregion

    }
}