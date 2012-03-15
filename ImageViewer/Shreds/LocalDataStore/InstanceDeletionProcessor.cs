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
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private delegate void PublishDelegate();

		internal class InstanceDeletionProcessor
		{
			private LocalDataStoreService _parent;

			private Thread _thread;
			private volatile bool _stop;

			private object _syncLock = new object();
			private List<string> _lowPriorityItems;
			private List<string> _highPriorityItems;

			public InstanceDeletionProcessor(LocalDataStoreService parent)
			{
				_parent = parent;
				_parent.StartEvent += new EventHandler(OnStart);
				_parent.StopEvent += new EventHandler(OnStop);
			}

			private bool QueueEmpty
			{
				get
				{
					return (_lowPriorityItems.Count == 0 && _highPriorityItems.Count == 0);
				}
			}

			private void OnStart(object sender, EventArgs e)
			{
				_lowPriorityItems = new List<string>();
				_highPriorityItems = new List<string>();

				_stop = false;
				_thread = new Thread(new ThreadStart(RunThread));
				_thread.Priority = ThreadPriority.BelowNormal;
				_thread.IsBackground = true;
				_thread.Start();
			}

			private void OnStop(object sender, EventArgs e)
			{
				_stop = true;
				lock (_syncLock)
				{
					if (this.QueueEmpty)
						Monitor.Pulse(_syncLock);
				}

				_thread.Join();
				_thread = null;
			}

			private void GetStatistics(IEnumerable<ISopInstance> sopInstances, out int numberExist, out long totalSpaceUsed)
			{
				numberExist = 0;
				totalSpaceUsed = 0;
				foreach (ISopInstance sopInstance in sopInstances)
				{
					FileInfo info;
					try
					{
						info = new FileInfo(sopInstance.GetLocationUri().LocalDiskPath);
						if (info.Exists)
						{
							++numberExist;
							totalSpaceUsed += info.Length;
						}
					}
					catch(Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}
				}
			}

			private string GetNextStudy()
			{
				lock (_syncLock)
				{
					if (this.QueueEmpty)
					{
						//the queue is empty, let's go back to importing.  NOTE: On startup, 
						// this does no harm since it's already in the importing state.
						_parent.ReserveState(ServiceState.Importing);
						_parent.ActivateState();

						//block until something has been added to the queue or 'stop' has been signalled.
						if (!_stop)
							Monitor.Wait(_syncLock);

						if (_stop)
							return null;

						//a request has come in and the state is already 'Deleting', let's activate the state.
						_parent.ActivateState();
					}

					string studyUid = null;

					if (_highPriorityItems.Count > 0)
					{
						studyUid = _highPriorityItems[0];
						_highPriorityItems.RemoveAt(0);
					}
					else if (_lowPriorityItems.Count > 0)
					{
						studyUid = _lowPriorityItems[0];
						_lowPriorityItems.RemoveAt(0);
					}

					return studyUid;
				}
			}

			private void RunThread()
			{
				do
				{
					string instanceUid = GetNextStudy();

					if (_stop)
						break;

					if (String.IsNullOrEmpty(instanceUid))
						continue;

					long totalUsedSpaceBefore = 0;
					int numberRelatedSopInstancesExistBefore = 0;

					long totalUsedSpaceAfter = 0;
					int numberRelatedSopInstancesExistAfter = 0;

					long actualFreedSpace = 0;

					string errorMessage = null;

					PublishDelegate publish = delegate()
					{
						DeletedInstanceInformation information = new DeletedInstanceInformation();
						information.InstanceLevel = InstanceLevel.Study;
						information.InstanceUid = instanceUid;
						information.ErrorMessage = errorMessage;
						information.TotalFreedSpace = actualFreedSpace;

						LocalDataStoreActivityPublisher.Instance.InstanceDeleted(information.Clone());
					};

					try
					{
						using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
						{
							IStudy study = reader.GetStudy(instanceUid);
							if (study == null)
							{
								errorMessage = String.Format(SR.ExceptionCannotDeleteStudyDoesNotExist, instanceUid);
								Platform.Log(LogLevel.Error, errorMessage);
								publish();
								continue;
							}

							List<ISopInstance> sopInstances;
							try
							{
								sopInstances = new List<ISopInstance>(study.GetSopInstances());
							}
							catch(Exception e)
							{
								sopInstances = new List<ISopInstance>(); //empty list.
								string message = String.Format("Failed to retrieve sop instances for study ({0}).", study.StudyInstanceUid);
								Platform.Log(LogLevel.Error, e, message);
							}
							
							GetStatistics(sopInstances, out numberRelatedSopInstancesExistBefore, out totalUsedSpaceBefore);

							try
							{
								FileRemover remover = new FileRemover(Instance.StorageDirectory);
								remover.DeleteFilesInStudy(study);

								using (IDataStoreStudyRemover studyRemover = DataAccessLayer.GetIDataStoreStudyRemover())
								{
									studyRemover.RemoveStudy(study.StudyInstanceUid);
								}
								
								remover.CleanupEmptyDirectories();
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Error, e);
								errorMessage = String.Format(SR.ExceptionFailedToDeleteStudy, instanceUid);
							}

							GetStatistics(sopInstances, out numberRelatedSopInstancesExistAfter, out totalUsedSpaceAfter);
							actualFreedSpace = totalUsedSpaceBefore - totalUsedSpaceAfter;
						}

						Platform.Log(LogLevel.Info,
						             String.Format(
						             	"Study Deletion Info - StudyInstanceUid: {0}, Space Freed (bytes): {1}, Total Number of Related Sop Instances: {2}, Number of Related Sop Instances Deleted: {3}",
						             	instanceUid, actualFreedSpace, numberRelatedSopInstancesExistBefore,
						             	numberRelatedSopInstancesExistBefore - numberRelatedSopInstancesExistAfter));

						publish();
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
						errorMessage = String.Format(SR.ExceptionFailedToDeleteStudy, instanceUid);
						publish();
					}
				}
				while (!_stop);
			}

			public void DeleteInstances(DeleteInstancesRequest request)
			{
				if (request.InstanceLevel != InstanceLevel.Study)
					throw new NotSupportedException(SR.ExceptionOnlyStudyLevelDeletionCurrentlySupported);

				lock (_syncLock)
				{
					bool pulse = this.QueueEmpty;
					if (pulse)
					{
						// We don't want to actually stop the running imports from this thread because the 
						// WCF connection could time out.  Instead, let's just reserve the 'Deleting' state and activate it
						// on the processing thread.  This will throw an exception if we cannot reserve the state.
						_parent.ReserveState(ServiceState.Deleting);
					}

					List<string> addList = request.DeletePriority == DeletePriority.Low ? _lowPriorityItems : _highPriorityItems;
					foreach (string instanceUid in request.InstanceUids)
					{
						if (!String.IsNullOrEmpty(instanceUid))
							addList.Add(instanceUid);
					}

					//the queue was empty and now it isn't, so release the wait.
					if (pulse && !this.QueueEmpty)
						Monitor.Pulse(_syncLock);
				}
			}
		}
	}
}
