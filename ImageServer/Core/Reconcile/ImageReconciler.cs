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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	class ImageReconciler
	{
		#region Private Members
		private IReadContext _readContext;
		private ServerPartition _partition;
		private Study _existingStudy;
		private StudyStorageLocation _existingStudyLocation;
		private ReconcileImageContext _reconcileContext;
        private List<StudyHistory> _studyHistoryList;

		#endregion

		#region Public Properties

		/// <summary>
		/// The server partition where the study is located.
		/// </summary>
		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}

		/// <summary>
		/// The existing <see cref="StudyStorageLocation"/> of the study.
		/// </summary>
		public StudyStorageLocation ExistingStudyLocation
		{
			get { return _existingStudyLocation; }
			set { _existingStudyLocation = value; }
		}

		/// <summary>
		/// The study against which the image will be reconciled.
		/// </summary>
		public Study ExistingStudy
		{
			get { return _existingStudy; }
			set { _existingStudy = value; }
		}

		public IReadContext ReadContext
		{
			get { return _readContext; }
			set { _readContext = value; }
		}

		#endregion

		#region Private Methods

	    private IList<StudyHistory> FindReconcileHistories(DicomFile file)
		{
			ImageSetDescriptor fileDesc = new ImageSetDescriptor(file.DataSet);

			_studyHistoryList = new List<StudyHistory>(ServerHelper.FindStudyHistories( ExistingStudyLocation.StudyStorage));

			IList<StudyHistory> reconcileHistories = _studyHistoryList.FindAll(
				delegate(StudyHistory item)
					{
						if (item.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled)
						{
							ImageSetDescriptor desc = XmlUtils.Deserialize<ImageSetDescriptor>(item.StudyData.DocumentElement);
							return desc.Equals(fileDesc);
						}
						else
							return false;
                       
					});

			if (reconcileHistories==null || reconcileHistories.Count==0)
			{
				// no history found in cache... reload the list and search again one more time
                _studyHistoryList = new List<StudyHistory>(ServerHelper.FindStudyHistories(ExistingStudyLocation.StudyStorage));

				reconcileHistories = _studyHistoryList.FindAll(
					delegate(StudyHistory item)
						{
							if (item.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled)
							{
								ImageSetDescriptor desc = XmlUtils.Deserialize<ImageSetDescriptor>(item.StudyData.DocumentElement);
								return desc.Equals(fileDesc);
							}
							else
								return false;
						});

			}

			return reconcileHistories;
		}

		#endregion

		#region Public Methods
		public void ScheduleReconcile(DicomFile file)
		{
			Platform.CheckForNullReference(Partition, "Partition");
			Platform.CheckForNullReference(ExistingStudy, "ExistingStudy");
			Platform.CheckForNullReference(ExistingStudyLocation, "ExistingStudyLocation");

			//TODO: optimization: use previously loaded history list if possible
			IList<StudyHistory> historyList = FindReconcileHistories(file);
			_reconcileContext = new ReconcileImageContext();
			_reconcileContext.Partition = _partition;
			_reconcileContext.CurrentStudyLocation = ExistingStudyLocation;
			_reconcileContext.File = file;
			_reconcileContext.CurrentStudy = ExistingStudy;
			_reconcileContext.History = historyList == null || historyList.Count == 0 ? null : historyList[0];
            
			
            Platform.Log(LogLevel.Info, "Scheduling new manual reconciliation for SOP {0}", file.MediaStorageSopInstanceUid);
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Schedule new reconciliation"))
            {
                InsertSIQReconcileStudyCommand updateStudyCommand = new InsertSIQReconcileStudyCommand(_reconcileContext);
                processor.AddCommand(updateStudyCommand);
                MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(_reconcileContext);
                processor.AddCommand(moveFileCommand);

                processor.AddCommand(new OpValidationCommand(_reconcileContext));
                if (processor.Execute() == false)
                {
                    throw new ApplicationException(String.Format("Unable to schedule image reconcilation : {0}", processor.FailureReason), processor.FailureException);
                }

            }
		}

		#endregion
	}
}