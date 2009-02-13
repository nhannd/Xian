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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    /// <summary>
    /// Base class with common routines for processors of <see cref="Model.ServiceLock"/> entries.
    /// </summary>
    public class BaseServiceLockItemProcessor : IDisposable
	{
		#region Private Members
		private IReadContext _readContext;
		private bool _cancelPending = false;
		private readonly object _syncRoot = new object();
		#endregion

		#region Protected Properties
		protected IReadContext ReadContext
        {
            get { return _readContext; }
        }
		protected bool CancelPending
		{
			get { lock (_syncRoot) return _cancelPending; }
		}
        #endregion

        #region Contructors
        public BaseServiceLockItemProcessor()
        {
            _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
        }
        #endregion

        #region Protected Methods
		/// <summary>
		/// Get a list of candidates from the <see cref="FilesystemQueue"/>.
		/// </summary>
		/// <param name="item">The ServiceLock item.</param>
		/// <param name="scheduledTime">The scheduled time to query against</param>
		/// <param name="type">The type of FilesystemQueue entry.</param>
		/// <param name="statusCheck">If true, check for specific status value WorkQueue entries already existing, otherwise check for any WorkQueue entry.</param>
		/// <returns>The list of queue entries.</returns>
        protected IList<FilesystemQueue> GetFilesystemQueueCandidates(Model.ServiceLock item, DateTime scheduledTime, FilesystemQueueTypeEnum type, bool statusCheck)
        {
            IFilesystemQueueEntityBroker broker = ReadContext.GetBroker<IFilesystemQueueEntityBroker>();
            FilesystemQueueSelectCriteria fsQueueCriteria = new FilesystemQueueSelectCriteria();

            fsQueueCriteria.FilesystemKey.EqualTo(item.FilesystemKey);
            fsQueueCriteria.ScheduledTime.LessThanOrEqualTo(scheduledTime);
            fsQueueCriteria.FilesystemQueueTypeEnum.EqualTo(type);

			// Do the select based on the QueueStudyState (used to be based on a link to the WorkQueue table)
			StudyStorageSelectCriteria studyStorageSearchCriteria = new StudyStorageSelectCriteria();
			studyStorageSearchCriteria.QueueStudyStateEnum.EqualTo(QueueStudyStateEnum.Idle);
			fsQueueCriteria.StudyStorage.Exists(studyStorageSearchCriteria);

            fsQueueCriteria.ScheduledTime.SortAsc(0);

            IList<FilesystemQueue> list = broker.Find(fsQueueCriteria, 0, ServiceLockSettings.Default.FilesystemQueueResultCount);

            return list;
        }

        /// <summary>
        /// Load the storage location for a Study and partition.
        /// </summary>
        protected static StudyStorageLocation LoadStorageLocation(ServerEntityKey serverPartitionKey, String studyInstanceUid)
        {
            StudyStorageLocation storageLocation;
        	if (!FilesystemMonitor.Instance.GetStudyStorageLocation(serverPartitionKey, studyInstanceUid, out storageLocation))
            {
                string error = String.Format("Unable to find storage location for study {0} on partition {1}",studyInstanceUid, serverPartitionKey);
                Platform.Log(LogLevel.Error, error);
                throw new ApplicationException(error);
            }
            return storageLocation;
        }

		/// <summary>
		/// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
		/// </summary>
		/// <param name="location">The location a study is stored.</param>
		/// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
		protected virtual StudyXml LoadStudyXml(StudyStorageLocation location)
		{
			StudyXml theXml = new StudyXml();

			String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
			if (File.Exists(streamFile))
			{
				using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
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
		/// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
		/// </summary>
		/// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
		/// <param name="context"></param>
		/// <param name="workQueueKey"></param>
		protected static void InsertWorkQueueUidFromStudyXml(StudyXml studyXml, IUpdateContext context, ServerEntityKey workQueueKey)
		{
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					WorkQueueUidUpdateColumns updateColumns = new WorkQueueUidUpdateColumns();
					updateColumns.Duplicate = false;
					updateColumns.Failed = false;
					updateColumns.SeriesInstanceUid = seriesXml.SeriesInstanceUid;
					updateColumns.SopInstanceUid = instanceXml.SopInstanceUid;
					updateColumns.WorkQueueKey = workQueueKey;

					IWorkQueueUidEntityBroker broker = context.GetBroker<IWorkQueueUidEntityBroker>();

					broker.Insert(updateColumns);
				}
			}
		}

		/// <summary>
		/// Estimate the folder size for a study
		/// </summary>
		/// <remarks>
		/// This routine loads the StudyXml file and traverses through each series
		/// for the study.  It then looks at the size of the first image in the series,
		/// and assumes the series size is equal to the first image size times the number
		/// of images within the series.  If the file sizes vary within the series, this
		/// algorithm will fall down a bit.
		/// </remarks>
		/// <param name="location">The StudyStorageLocation object for the study.</param>
		/// <returns></returns>
		protected float EstimateFolderSizeFromStudyXml(StudyStorageLocation location)
		{
			float folderSize = 0.0f;
			string studyFolder = location.GetStudyPath();

			string file = Path.Combine(studyFolder, location.StudyInstanceUid + ".xml");
			if (File.Exists(file))
			{
				FileInfo finfo = new FileInfo(file);
				folderSize += finfo.Length;
			}
			file = Path.Combine(studyFolder, location.StudyInstanceUid + ".xml.gz");
			if (File.Exists(file))
			{
				FileInfo finfo = new FileInfo(file);
				folderSize += finfo.Length;
			}

			StudyXml study = LoadStudyXml(location);
			foreach (SeriesXml series in study)
			{
				string seriesFolder = Path.Combine(studyFolder, series.SeriesInstanceUid);

				foreach (InstanceXml instance in series)
				{
					if (instance.FileSize != 0)
					{
						folderSize += instance.FileSize;
					}
					else
					{
						file = Path.Combine(seriesFolder, String.Format("{0}.dcm", instance.SopInstanceUid));
						if (File.Exists(file))
						{
							FileInfo finfo = new FileInfo(file);
							folderSize += finfo.Length;
						}
					}
				}
			}

			return folderSize;
		}

        #endregion

		#region Public Methods
		public void Cancel()
		{
			lock (_syncRoot)
				_cancelPending = true;
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Calculate the folder size based on recursing through all files
		/// within the folder and adding up their sizes.
		/// </summary>
		/// <param name="folder">The path of the folder to calculate the size for.</param>
		/// <returns></returns>
        protected static float CalculateFolderSize(string folder)
        {
            float folderSize = 0.0f;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }

                        foreach (string dir in Directory.GetDirectories(folder))
                            folderSize += CalculateFolderSize(dir);
                    }
                    catch (NotSupportedException e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unable to calculate folder size");
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Platform.Log(LogLevel.Error, e, "Unable to calculate folder size");
            }
            return folderSize;
        }

        /// <summary>
        /// Set a <see cref="ServiceLock"/> entry to pending.
        /// </summary>
        /// <param name="item">The <see cref="ServiceLock"/> entry to set.</param>
        /// <param name="scheduledTime"></param>
        /// <param name="enabled">Bool telling if the ServiceLock entry should be enabled after unlock.</param>
        protected static void UnlockServiceLock(Model.ServiceLock item, bool enabled, DateTime scheduledTime)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // Update the WorkQueue item status and times.
                IUpdateServiceLock update = updateContext.GetBroker<IUpdateServiceLock>();

                ServiceLockUpdateParameters parms = new ServiceLockUpdateParameters();
                parms.ServiceLockKey = item.GetKey();
                parms.Lock = false;
                parms.ScheduledTime = scheduledTime;
                parms.ProcessorId = item.ProcessorId;
                parms.Enabled = enabled;

                if (false == update.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update StudyLock GUID Status: {0}",
                                 item.GetKey().ToString());
                }

                updateContext.Commit();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public void Dispose()
        {
            if (_readContext != null)
            {
                _readContext.Dispose();
                _readContext = null;
            }
        }
        #endregion
    }
}
