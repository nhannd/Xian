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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private class SentFileProcessor
		{
			private LocalDataStoreService _parent;
			private SimpleBlockingThreadPool _sentFileInformationProcessor;
			private List<SendProgressItem> _sendProgressItems;

			public SentFileProcessor(LocalDataStoreService parent)
			{
				_sentFileInformationProcessor = new SimpleBlockingThreadPool(1);
				_sendProgressItems = new List<SendProgressItem>();

				_parent = parent;
				_parent.StartEvent += new EventHandler(OnStart);
				_parent.StopEvent += new EventHandler(OnStop);
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);
			}

			private void OnStart(object sender, EventArgs e)
			{
				_sentFileInformationProcessor.Start();
			}
			private void OnStop(object sender, EventArgs e)
			{
				_sentFileInformationProcessor.Stop();
			}

			private void OnRepublish(object sender, EventArgs e)
			{
				//use the same thread for everything, then no synclock is required on the progress items.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							foreach (SendProgressItem item in _sendProgressItems)
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item.Clone());
						}
					);
			}

			private void OnCancel(object sender, ItemEventArgs<CancelProgressItemInformation> e)
			{
				CancelProgressItemInformation information = e.Item;

				if (information.ProgressItemIdentifiers == null)
					return;

				if (information.CancellationFlags != CancellationFlags.Clear)
					return;

				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							foreach (Guid identifier in information.ProgressItemIdentifiers)
							{
								SendProgressItem item = _sendProgressItems.Find(delegate(SendProgressItem test) { return test.Identifier.Equals(identifier); });
								if (item != null)
								{
									_sendProgressItems.Remove(item);
									item.Removed = true;
									LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item.Clone());
								}
							}
						}
					);
			}

			private StudyInformation GetStudyInformation(StoreScuSentFileInformation information)
			{
				//If the study information is already specified, then don't parse the file.
				if (information.StudyInformation != null)
					return information.StudyInformation;

				DicomFile dicomFile = new DicomFile(information.FileName);
				try
				{
					dicomFile.Load(DicomTags.PixelData, DicomReadOptions.Default);
				}
				catch (Exception e)
				{
					throw new Exception(String.Format(SR.FormatUnableToParseFile, information.FileName), e);
				}

				StudyInformationFieldExchanger studyInformation = new StudyInformationFieldExchanger();
				dicomFile.DataSet.LoadDicomFields(studyInformation);
				return studyInformation;
			}

			private SendProgressItem GetProgressItem(SendOperationReference reference, string toAETitle, StudyInformation studyInformation)
			{
				//no synclock required, since only the single thread pool thread is accessing the items.
				SendProgressItem progressItem = _sendProgressItems.Find(delegate(SendProgressItem testItem)
					{
						if (reference == null)
						{
							return testItem.SendOperationReference == null && testItem.ToAETitle == toAETitle &&
							testItem.StudyInformation.StudyInstanceUid == studyInformation.StudyInstanceUid;
						}
						else
						{
							return testItem.SendOperationReference == reference;
						}
					});

				if (progressItem == null)
				{
					progressItem = new SendProgressItem();
					progressItem.SendOperationReference = reference;

					if (reference != null)
					{
						progressItem.Identifier = reference.Identifier;
						progressItem.IsBackground = reference.IsBackground;
					}
					else
					{
						 progressItem.Identifier = Guid.NewGuid();
					}

					progressItem.StartTime = Platform.Time;
					progressItem.LastActive = progressItem.StartTime;
					progressItem.ToAETitle = toAETitle;
					progressItem.StudyInformation = studyInformation;
					progressItem.NumberOfFilesExported = 0;
					progressItem.AllowedCancellationOperations = CancellationFlags.Clear;

					_sendProgressItems.Add(progressItem);
				}

				return progressItem;
			}

			public void ProcessSentFileInformation(StoreScuSentFileInformation sentFileInformation)
			{
				//always leave the main thread free to accept incoming requests.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							try
							{
								StudyInformation studyInformation = GetStudyInformation(sentFileInformation);
								SendProgressItem progressItem = GetProgressItem(sentFileInformation.SendOperationReference, sentFileInformation.ToAETitle, studyInformation);
								progressItem.SendOperationReference = sentFileInformation.SendOperationReference;
								progressItem.StudyInformation = studyInformation;

								if (progressItem.StatusMessage == SR.MessagePending)
									progressItem.StatusMessage = "";

								progressItem.LastActive = Platform.Time;
								++progressItem.NumberOfFilesExported;
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Error, e); //this only happens if we can't parse the file, so not much we can do.
							}
						}
					);
			}

			internal void SendStarted(SendStudyInformation information)
			{
				//always leave the main thread free to accept incoming requests.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							SendProgressItem progressItem = GetProgressItem(information.SendOperationReference, information.ToAETitle, information.StudyInformation);
							progressItem.StudyInformation = information.StudyInformation;
							progressItem.LastActive = Platform.Time; 
							progressItem.StatusMessage = SR.MessagePending;
							progressItem.SendOperationReference = information.SendOperationReference;
							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
						}
					);
			}
			
			public void SendError(SendErrorInformation errorInformation)
			{
				//always leave the main thread free to accept incoming requests.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							SendProgressItem progressItem = GetProgressItem(errorInformation.SendOperationReference, errorInformation.ToAETitle, errorInformation.StudyInformation);
							progressItem.StudyInformation = errorInformation.StudyInformation;
							progressItem.LastActive = Platform.Time;
							progressItem.StatusMessage = errorInformation.ErrorMessage;
							progressItem.SendOperationReference = errorInformation.SendOperationReference;
							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
						}
					);
			}
		}
	}
}