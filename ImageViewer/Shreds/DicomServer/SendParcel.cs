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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisNetwork;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal enum ParcelTransferState
	{
		Unknown = 0,
		Pending,
		InProgress,
		CancelRequested,
		Cancelled,
		PauseRequested,
		Paused,
		Completed,
		Error
	}
	
	internal class SendParcel : Parcel
    {
		private object _statsLock = new object();

		private Dictionary<string, string> _setTransferSyntaxes;
		private Dictionary<string, string> _setSopClasses;
        private List<ISopInstance> _sopInstances;
		private Dictionary<string, string> _sentSopInstanceUids;

        public SendParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
            : base(sourceAE, destinationAE, parcelDescription)
        {
			_setTransferSyntaxes = new Dictionary<string, string>();
			_setSopClasses = new Dictionary<string, string>();
            _sopInstances = new List<ISopInstance>();
			_sentSopInstanceUids = new Dictionary<string, string>();
        }

        private SendParcel()
            : base()
        {
        }

        public IList<string> TransferSyntaxes
        {
            get { return new List<string>(_setTransferSyntaxes.Keys).AsReadOnly(); }
        }

		public IList<string> SopClasses
		{
			get { return new List<string>(_setSopClasses.Keys).AsReadOnly(); }
		}


        public IList<ISopInstance> SopInstances
        {
            get { return _sopInstances.AsReadOnly(); }
        }

		public IList<ISopInstance> SentSopInstances
		{
			get 
			{
				return new List<ISopInstance>(_sopInstances.FindAll
					(
						delegate(ISopInstance sop) { return _sentSopInstanceUids.ContainsKey(sop.GetSopInstanceUid()); })
					
					).AsReadOnly(); 
			}
		}

		public IList<ISopInstance> UnsentSopInstances
		{
			get
			{
				return new List<ISopInstance>(_sopInstances.FindAll
					(
						delegate(ISopInstance sop) { return !_sentSopInstanceUids.ContainsKey(sop.GetSopInstanceUid()); })

					).AsReadOnly();
			}
		}

		private IDictionary<IStudy, IList<ISopInstance>> GetSopInstancesByStudy(IList<ISopInstance> sopInstances)
		{
			IDictionary<IStudy, IList<ISopInstance>> sopInstancesByStudy = new Dictionary<IStudy, IList<ISopInstance>>();

			foreach (ISopInstance sopInstance in sopInstances)
			{
				ISeries parentSeries = sopInstance.GetParentSeries();
				IStudy parentStudy = parentSeries.GetParentStudy();

				if (!sopInstancesByStudy.ContainsKey(parentStudy))
					sopInstancesByStudy[parentStudy] = new List<ISopInstance>();

				sopInstancesByStudy[parentStudy].Add(sopInstance);
			}

			return sopInstancesByStudy;
		}

		public IDictionary<IStudy, IList<ISopInstance>> SopInstancesByStudy
		{
			get
			{
				return GetSopInstancesByStudy(_sopInstances);
			}
		}

		public IDictionary<IStudy, IList<ISopInstance>> SentSopInstancesByStudy
		{
			get
			{
				return GetSopInstancesByStudy(this.SentSopInstances);
			}
		}

		public IDictionary<IStudy, IList<ISopInstance>> UnsentSopInstancesByStudy
		{
			get
			{
				return GetSopInstancesByStudy(this.UnsentSopInstances);
			}
		}

		#region Internal and Private members
        private void AddTransferSyntax(Uid newTransferSyntax)
        {
			if (_setTransferSyntaxes.ContainsKey(newTransferSyntax))
				return;

			_setTransferSyntaxes[newTransferSyntax] = newTransferSyntax;
        }

        private void AddSopClass(Uid newSopClass)
        {
			if (_setSopClasses.ContainsKey(newSopClass))
				return;

			_setSopClasses[newSopClass] = newSopClass;
		}

        private void AddSopInstance(ISopInstance sop)
        {
            ISopInstance foundSop = _sopInstances.Find(
                    delegate(ISopInstance iteratedSop)
                    {
						if (null != iteratedSop)
							return sop.GetSopInstanceUid() == iteratedSop.GetSopInstanceUid();
						else
							return false;
                    }
                );

            if (null == foundSop)
				_sopInstances.Add(sop);
        }

        private void AddSopInstanceIntoParcel(ISopInstance sop)
        {
            Uid transferSyntax = sop.GetTransferSyntaxUid();
            Uid sopClass = sop.GetSopClassUid();
            AddTransferSyntax(transferSyntax);
            AddSopClass(sopClass);
            AddSopInstance(sop);
        }

		private IEnumerable<string> SopInstanceFilenamesList
		{
			get
			{
				List<string> sops = new List<string>();
				foreach (ISopInstance sop in _sopInstances)
				{
					sops.Add(sop.GetLocationUri().LocalDiskPath);
				}
				return sops.AsReadOnly();
			}
		}

        #endregion

        #region ISendParcel Members

        /// <summary>
        /// Allows client to include a particular DICOM entity, whether a Study, Series or Sop Instance
        /// in a parcel for sending to a remote AE
        /// </summary>
        /// <param name="referencedUid">The UID of the entity, whether a Study, Series or Sop Instance.
        /// If the UID refers to a Study or Series, all the associasted Sop Instances of that Study or
        /// Series will be included in the parcel.
        /// </param>
        /// <param name="parcelDescription">Client-defined description of the parcel. This may include
        /// the Patient's Name, Study Description, etc.
        /// </param>
        /// <returns>The number of new Sop Instances that were added.</returns>
        public int Include(Uid referencedUid)
        {
            // store current count of objects
			int currentObjectCount = _sopInstances.Count;

			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				// search for study that matches Uid
				if (!reader.StudyExists(referencedUid))
				{
					// search for series that matches Uid
					if (!reader.SeriesExists(referencedUid))
					{
						// search for SOP instance that matches Uid
						if (!reader.SopInstanceExists(referencedUid))
						{
							return 0;
						}
						else
						{
							// get the SopInstance object
							ISopInstance sop = reader.GetSopInstance(referencedUid);
							AddSopInstanceIntoParcel(sop);
						}
					}
					else // series was found
					{
						ISeries series = reader.GetSeries(referencedUid);
						foreach (ISopInstance sop in series.GetSopInstances())
						{
							AddSopInstanceIntoParcel(sop);
						}
					}
				}
				else // study was found
				{
					IStudy study = reader.GetStudy(referencedUid);
					foreach (ISopInstance sop in study.GetSopInstances())
					{
						AddSopInstanceIntoParcel(sop);
					}
				}
			}

        	int afterIncludeObjectCount = _sopInstances.Count;
            return afterIncludeObjectCount - currentObjectCount;
        }

		public bool IsActive()
		{ 
			return (this.ParcelTransferState == ParcelTransferState.InProgress ||
					this.ParcelTransferState == ParcelTransferState.Paused ||
					this.ParcelTransferState == ParcelTransferState.PauseRequested ||
					this.ParcelTransferState == ParcelTransferState.Pending);

		}

		public ParcelTransferState GetState()
        {
            return this.ParcelTransferState;
        }

		//a tiny hack to allow the thread doing the move update to get a reliable snapshot of the progress on the store thread.
		public void GetSafeStats(out ParcelTransferState state, out int totalSteps, out int currentStep)
		{
			lock (_statsLock)
			{
				state = this.ParcelTransferState;
				totalSteps = this.TotalProgressSteps;
				currentStep = this.CurrentProgressStep;
			}
		}

		public int GetToSendObjectCount()
		{
			return _sopInstances.Count;
		}


		public void Send()
		{
			DicomClient client = new DicomClient(base.SourceAE);

			EventHandler<SendProgressUpdatedEventArgs> updateDelegate = delegate(object source, SendProgressUpdatedEventArgs args)
					{
						lock (_statsLock)
						{
							if (args.TotalCount == args.CurrentCount)
								this.ParcelTransferState = ParcelTransferState.Completed;

							_sentSopInstanceUids[args.SentSopInstanceUid] = args.SentSopInstanceUid;
							this.TotalProgressSteps = args.TotalCount;
							this.CurrentProgressStep = args.CurrentCount;
						}
					};

			client.SendProgressUpdated += updateDelegate;

			using (client)
			{
				try
				{
					client.Store(this.DestinationAE, this.SopInstanceFilenamesList, this.SopClasses, this.TransferSyntaxes);
					this.ParcelTransferState = ParcelTransferState.Completed;
				}
				catch
				{
					this.ParcelTransferState = ParcelTransferState.Error;
					throw;
				}
				finally
				{
					client.SendProgressUpdated -= updateDelegate;
				}
			}
		}

        #endregion
    }
}
