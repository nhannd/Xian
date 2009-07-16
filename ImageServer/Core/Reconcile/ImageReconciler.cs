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
using System.Diagnostics;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Reconcile a Dicom image against a study.
	/// </summary>
	/// <remarks>
	/// When an image is reconciled, it may either entered the Study Integrity Queue for user manual intervention
	/// or, if reconciliation has happened before, a <see cref="WorkQueueTypeEnum.ReconcileStudy"/> work queue will be spawned to update the information in the image before merging
	/// it into the study.
	/// </remarks>
	class ImageReconciler
	{
		#region Private Members
		private IReadContext _readContext;
		private ServerPartition _partition;
		private Study _existingStudy;
		private StudyStorageLocation _existingStudyLocation;
		private string _sopInstanceUid;
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

        internal static bool LookLikeSameNames(string name1, string name2)
        {
            name1 = StringUtilities.EmptyIfNull(name1);
            name2 = StringUtilities.EmptyIfNull(name2);
            string normalizedS1 = DicomNameUtils.Normalize(name1, DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces);
            string normalizedS2 = DicomNameUtils.Normalize(name2, DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces);


            // if both have "^", may need manual reconciliation
            // eg: "John ^ Smith" vs  "John  Smith^^" ==> manual
            //     "John ^ Smith" vs  "John ^ Smith^^" ==> auto
            if (name1.Contains("^") && name2.Contains("^"))
            {
                PersonName n1 = new PersonName(normalizedS1);
                PersonName n2 = new PersonName(normalizedS2);
                if (n1.AreSame(n2, PersonNameComparisonOptions.CaseInsensitive))
                    return true;
                else
                    return false;
            }


            if (normalizedS1.Length != normalizedS2.Length) return false;

            normalizedS1 = normalizedS1.ToUpper();
            normalizedS2 = normalizedS2.ToUpper();

            if (normalizedS1.Equals(normalizedS2))
                return true;

            for (int i = 0; i < normalizedS1.Length; i++)
            {
                // If S1[i] is ^ or space, S2[i] must be either ^ or space to be considered being the same
                // Otherwise, S1[i] must be the same as S2[i].
                if (normalizedS1[i] == '^' || normalizedS1[i] == ' ')
                {
                    if (normalizedS2[i] != '^' && normalizedS2[i] != ' ')
                        return false;
                }
                else
                {
                    if (normalizedS1[i] != normalizedS2[i])
                        return false;

                }
            }
            return true;
        }


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


		private string GetSuggestedTemporaryReconcileFolderPath()
		{
			string path = Path.Combine(ExistingStudyLocation.FilesystemPath, ExistingStudyLocation.PartitionFolder);
			path = Path.Combine(path, "Reconcile");
			path = Path.Combine(path, Guid.NewGuid().ToString());
			return path;
		}


		private bool TryAutoCorrection(DicomMessageBase message)
		{
			Platform.CheckForNullReference(ExistingStudyLocation, "ExistingStudyLocation");
            
			StudyComparer comparer = new StudyComparer();
			DifferenceCollection list = comparer.Compare(message, ExistingStudy, Partition.GetComparisonOptions());

			if (list.Count == 1)
			{
				ComparisionDifference different = list[0];
				if (different.DicomTag.TagValue == DicomTags.PatientsName)
				{
					if (LookLikeSameNames(different.ExpectValue, different.RealValue))
					{
						AutoCorrectPatientsName(StudyReconcileAction.Merge);
						return true;
					}
				}
			}

			return false;
		}

		
		private void AutoCorrectPatientsName(StudyReconcileAction method)
		{
			Platform.Log(LogLevel.Info, "Scheduling auto reconciliation to correct patient name...");
			using (ServerCommandProcessor processor = new ServerCommandProcessor("Schedule ReconcileStudy request"))
			{
				_reconcileContext.StoragePath = GetSuggestedTemporaryReconcileFolderPath();
                
				switch(method)
				{
					case StudyReconcileAction.Merge:
						{
							processor.AddCommand(new InsertMergeToExistingStudyHistoryCommand(_reconcileContext));
							break;
						}
					default:
						{
							throw new NotImplementedException();
						}
				}
                
				InsertReconcileStudyCommand insertReconcileStudyCommand = new InsertReconcileStudyCommand(_reconcileContext);
				MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(_reconcileContext);

				processor.AddCommand(insertReconcileStudyCommand);
				processor.AddCommand(moveFileCommand);
                
				if (processor.Execute() == false)
				{
					throw new ApplicationException(String.Format("Unable to create ReconcileStudy request: {0}", processor.FailureReason), processor.FailureException);
				}
				Platform.Log(ServerPlatform.InstanceLogLevel, "SOP {0} has been scheduled for auto reconciliation.", _sopInstanceUid);
			}
		}

		#endregion

		#region Public Methods
		public void ReconcileImage(DicomFile file)
		{
			Platform.CheckForNullReference(Partition, "Partition");
			Platform.CheckForNullReference(ExistingStudy, "ExistingStudy");
			Platform.CheckForNullReference(ExistingStudyLocation, "ExistingStudyLocation");

			_sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            
			//TODO: optimization: use previously loaded history list if possible
			IList<StudyHistory> historyList = FindReconcileHistories(file);
			_reconcileContext = new ReconcileImageContext();
			_reconcileContext.Partition = _partition;
			_reconcileContext.CurrentStudyLocation = ExistingStudyLocation;
			_reconcileContext.File = file;
			_reconcileContext.CurrentStudy = ExistingStudy;
			_reconcileContext.History = historyList == null || historyList.Count == 0 ? null : historyList[0];
            
			LogDebugInfo(_reconcileContext);

			if (historyList == null || historyList.Count == 0)
			{
				if (ImageServerCommonConfiguration.EnablePatientsNameAutoCorrection 
				    && TryAutoCorrection(file))
				{
					return;
				}
            
				Platform.Log(LogLevel.Debug, "Scheduling new manual reconciliation...");
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
				Platform.Log(LogLevel.Info, "SOP {0} has been scheduled for manual reconciliation.", _sopInstanceUid);
			}
			else
			{
				Platform.Log(LogLevel.Debug, "Scheduling Auto Reconciliation...");

				if (_reconcileContext.History.DestStudyStorageKey != null)
				{
					StudyStorage destStorage = StudyStorage.Load(ReadContext, _reconcileContext.History.DestStudyStorageKey);
					Debug.Assert(destStorage != null);

					_reconcileContext.DestinationStudyLocation = StudyStorageLocation.FindStorageLocations(destStorage)[0];
					Debug.Assert(_reconcileContext.DestinationStudyLocation != null);
					Study destStudy = destStorage.LoadStudy(ReadContext);
					Debug.Assert(destStudy != null);
				}
                
				// Insert 'ReconcileStudy' in the work queue
				_reconcileContext.StoragePath = GetSuggestedTemporaryReconcileFolderPath(); // since we no longer have the record in the Study Integrity Queue
                
				int index = _studyHistoryList.IndexOf(_reconcileContext.History);
				using (ServerCommandProcessor processor = new ServerCommandProcessor("Schedule ReconcileStudy request based on histrory"))
				{
					InsertReconcileStudyCommand insertCommand = new InsertReconcileStudyCommand(_reconcileContext);
                    
					// Note: we need to retrieve the reconcile folder path using InsertReconcileStudyCommand 
					// before we can move the image
					MoveReconcileImageCommand moveFileCommand = new MoveReconcileImageCommand(_reconcileContext);
                    
					processor.AddCommand(insertCommand);
					processor.AddCommand(moveFileCommand);
					processor.AddCommand(new OpValidationCommand(_reconcileContext));
                    
					if (processor.Execute() == false)
					{
                        throw new ApplicationException(String.Format("Unable to create ReconcileStudy request: {0}", processor.FailureReason), processor.FailureException);
					}

					Debug.Assert(insertCommand.ReconcileStudyWorkQueueItem != null);
				}
				Platform.Log(LogLevel.Info, "SOP {0} has been scheduled for auto reconciliation.", _sopInstanceUid);

				_studyHistoryList[index] = _reconcileContext.History;
			}
		}

		private void LogDebugInfo(ReconcileImageContext context)
		{
			if (Platform.IsLogLevelEnabled(LogLevel.Debug))
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Image To Be Reconciled:\n");
				sb.AppendFormat("\tSOP={0}\n", _sopInstanceUid);
				sb.AppendFormat("\tExisting Patient={0}\n", ExistingStudy.PatientsName);
				sb.AppendFormat("\tExisting Study={0}\n", ExistingStudy.StudyInstanceUid);
				sb.AppendFormat("\tReferenced History record to be used: {0}\n",
				                context.History == null ? "N/A" : context.History.Key.ToString());
				sb.AppendFormat("\tCan reconcile automatically? {0}\n", context.History != null ? "Yes" : "No");

				Platform.Log(LogLevel.Debug, sb.ToString());
			}
		}

		#endregion
	}
}