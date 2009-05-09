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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core
{

	/// <summary>
	/// Encapsulates the context of the 'StudyProcess' operation.
	/// </summary>
	public class StudyProcessorContext
	{
		#region Private Members

	    private readonly object _syncLock = new object();
        private readonly StudyStorageLocation _storageLocation;
		private Study _study;
		private ServerRulesEngine _sopProcessedRulesEngine;
		private IReadContext _readContext;

	    #endregion

		#region Constructors

		public StudyProcessorContext(StudyStorageLocation storageLocation)
		{
		    Platform.CheckForNullReference(storageLocation, "storageLocation");
		    _storageLocation = storageLocation;
		}

		#endregion

		#region Public Properties

        /// <summary>
        /// Gets
        /// </summary>
        internal ServerPartition Partition
		{
            get { return _storageLocation.ServerPartition; }
		}

		internal Study Study
		{
            get
            {
                if (_study==null)
                {
                    lock (_syncLock)
                    {
                        _study = _storageLocation.LoadStudy(ExecutionContext.Current.PersistenceContext);    
                    }
                    
                }
                return _study;
            }

		}

        internal StudyStorageLocation StorageLocation
		{
			get { return _storageLocation; }
		}

	    public ServerRulesEngine SopProcessedRulesEngine
		{
			get
			{
                if (_sopProcessedRulesEngine==null)
                {
                    lock (_syncLock)
                    {
                        _sopProcessedRulesEngine =
                            new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Partition.GetKey());
                        _sopProcessedRulesEngine.Load();
                    }
                }
			    return _sopProcessedRulesEngine;
			}
			set
			{
                lock (_syncLock)
                {
                    _sopProcessedRulesEngine = value;
                }
			}
		}

	    public IReadContext ReadContext
		{
			get
			{
				if (_readContext == null)
				{
                    lock (_syncLock)
                    {
                        _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
                    }
				}
				return _readContext;
			}
			set
			{
                lock (_syncLock)
                {
                    _readContext = value;
                }
			}
		}


	    #endregion
	}

	/// <summary>
	/// Processor for Sop Instances being inserted into the database.
	/// </summary>
	public class SopInstanceProcessor
	{
		#region Private Members

		private readonly StudyProcessorContext _context;
		private readonly InstanceStatistics _instanceStats = new InstanceStatistics();
		private string _modality;

	    #endregion

		#region Constructors

		public SopInstanceProcessor(StudyProcessorContext context)
		{
            Platform.CheckForNullReference(context, "context");
		    _context = context;
		}

		#endregion

		#region Public Properties

		public string Modality
		{
			get { return _modality; }
		}

		public InstanceStatistics InstanceStats
		{
			get { return _instanceStats; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Process a specific DICOM file related to a <see cref="WorkQueue"/> request.
		/// </summary>
		/// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
		/// <param name="duplicate"></param>
		/// <param name="file"></param>
        public virtual ProcessingResult ProcessFile(DicomFile file, StudyXml stream, bool duplicate, bool compare)
		{
		    ProcessingResult result = new ProcessingResult();
            result.Status = ProcessingStatus.Failed; // will reset later

            _instanceStats.ProcessTime.Start();

            if (ShouldReconcile(file, duplicate) && compare)
			{
				ScheduleReconcile(file, duplicate);
                result.Status = ProcessingStatus.Reconciled;
			}
			else
			{
				InsertInstance(file, stream, duplicate);
			    result.Status = ProcessingStatus.Success;
			}

			_instanceStats.ProcessTime.End();

			if (_context.SopProcessedRulesEngine.Statistics.LoadTime.IsSet)
				_instanceStats.SopRulesLoadTime.Add(_context.SopProcessedRulesEngine.Statistics.LoadTime);

			if (_context.SopProcessedRulesEngine.Statistics.ExecutionTime.IsSet)
				_instanceStats.SopEngineExecutionTime.Add(_context.SopProcessedRulesEngine.Statistics.ExecutionTime);


			_context.SopProcessedRulesEngine.Statistics.Reset();

            //TODO: Should throw exception if result is failed?
		    return result;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns a value indicating whether the Dicom image must be reconciled.
		/// </summary>
		/// <param name="message">The Dicom message</param>
		/// <returns></returns>
		/// <param name="duplicate"></param>
		private bool ShouldReconcile(DicomMessageBase message, bool duplicate)
		{
			Platform.CheckForNullReference(_context, "_context");
			Platform.CheckForNullReference(message, "message");

			if (_context.Study == null)
			{
				// the study doesn't exist in the database
				return false;
			}
			else
			{
				DifferenceCollection list = StudyHelper.Compare(message, _context.Study, _context.Partition);

				if (list != null && list.Count > 0)
				{
					LogDifferences(message, list);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private static void LogDifferences(DicomMessageBase message, DifferenceCollection list)
		{
			string sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Found {0} issue(s) in SOP {1}\n", list.Count, sopInstanceUid);
			sb.Append(list.ToString());
			Platform.Log(LogLevel.Warn, sb.ToString());
		}

		/// <summary>
		/// Schedules a reconciliation for the specified <see cref="DicomFile"/>
		/// </summary>
		/// <param name="file"></param>
		/// <param name="duplicate"></param>
		private void ScheduleReconcile(DicomFile file, bool duplicate)
		{
			ImageReconciler reconciler;
			reconciler = new ImageReconciler();
			reconciler.ReadContext = _context.ReadContext;
			reconciler.ExistingStudy = _context.Study;
			reconciler.ExistingStudyLocation = _context.StorageLocation;
			reconciler.Partition = _context.Partition;
			reconciler.ReconcileImage(file, duplicate);
		}

		private void InsertInstance(DicomFile file, StudyXml stream, bool duplicate)
		{
			//Platform.CheckForNullReference(_context, "_context");
			//Platform.CheckForNullReference(_context.WorkQueueItem, "_context.WorkQueueItem");
            
			using (ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM file"))
			{
				InsertInstanceCommand insertInstanceCommand = null;
				InsertStudyXmlCommand insertStudyXmlCommand = null;

				String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
				_modality = file.DataSet[DicomTags.Modality].GetString(0, String.Empty);

				try
				{

					// Update the StudyStream object
					insertStudyXmlCommand = new InsertStudyXmlCommand(file, stream, _context.StorageLocation);
					processor.AddCommand(insertStudyXmlCommand);

					// Insert into the database, but only if its not a duplicate so the counts don't get off
					insertInstanceCommand = new InsertInstanceCommand(file, _context.StorageLocation);
					processor.AddCommand(insertInstanceCommand);
					
					// Create a context for applying actions from the rules engine
					ServerActionContext context =
						new ServerActionContext(file, _context.StorageLocation.FilesystemKey, _context.Partition.Key, _context.StorageLocation.Key);
					context.CommandProcessor = processor;

					// Run the rules engine against the object.
					_context.SopProcessedRulesEngine.Execute(context);

					// Do insert into the archival queue.  Note that we re-run this with each object processed
					// so that the scheduled time is pushed back each time.  Note, however, if the study only 
					// has one image, we could incorrectly insert an ArchiveQueue request, since the 
					// study rules haven't been run.  We re-run the command when the study processed
					// rules are run to remove out the archivequeue request again, if it isn't needed.
					context.CommandProcessor.AddCommand(
						new InsertArchiveQueueCommand(_context.Partition.Key, _context.StorageLocation.Key));

					// Do the actual processing
					if (!processor.Execute())
					{
						Platform.Log(LogLevel.Error, "Failure processing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
						Platform.Log(LogLevel.Error, "File that failed processing: {0}", file.Filename);
						throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid);
					}
					else
					{
						Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
					             processor.Description);
					processor.Rollback();
					throw new ApplicationException("Unexpected exception when processing file.", e);
				}
				finally
				{
					if (insertInstanceCommand != null && insertInstanceCommand.Statistics.IsSet)
						_instanceStats.InsertDBTime.Add(insertInstanceCommand.Statistics);
					if (insertStudyXmlCommand != null && insertStudyXmlCommand.Statistics.IsSet)
						_instanceStats.InsertStreamTime.Add(insertStudyXmlCommand.Statistics);
				}
			}
		}
		#endregion
	}

    public class ProcessingResult
    {
        public ProcessingStatus Status;
    }

    public enum ProcessingStatus
    {
        Success,
        Reconciled,
        Failed
    }
}