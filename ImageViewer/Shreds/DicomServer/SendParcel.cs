using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public enum ParcelTransferState
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
	
	public interface ISendParcel
    {
        int Include(Uid referencedUid);
        ParcelTransferState GetState();
        int GetToSendObjectCount();
        IEnumerable<string> GetReferencedSopInstanceFileNames();
    }

    /// <summary>
    /// Allows access to the DataStore in a way that's not specific to Study, Series or SopInstance
    /// </summary>
    public class SendParcel : Parcel, ISendParcel
    {
        private Dictionary<string, string> _setTransferSyntaxes;
		private Dictionary<string, string> _setSopClasses;
        private List<ISopInstance> _sopInstances;

        public SendParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
            : base(sourceAE, destinationAE, parcelDescription)
        {
			_setTransferSyntaxes = new Dictionary<string, string>();
			_setSopClasses = new Dictionary<string, string>();
            _sopInstances = new List<ISopInstance>();
        }

        private SendParcel()
            : base()
        {
        }

        public virtual IDataStoreReader DataStoreReader
        {
            get { return DataAccessLayer.GetIDataStoreReader(); }
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
                            return sop.IsIdenticalTo(iteratedSop);
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
            int currentObjectCount = (this.SopInstances as List<ISopInstance>).Count;

            // search for study that matches Uid
            if (!this.DataStoreReader.StudyExists(referencedUid))
            {

                // search for series that matches Uid
                if (!this.DataStoreReader.SeriesExists(referencedUid))
                {
                    // search for SOP instance that matches Uid
                    if (!this.DataStoreReader.SopInstanceExists(referencedUid))
                    {
                        return 0;
                    }
                    else
                    {
                        // get the SopInstance object
                        ISopInstance sop = this.DataStoreReader.GetSopInstance(referencedUid);
                        AddSopInstanceIntoParcel(sop);
                    }
                }
                else // series was found
                {
                    ISeries series = this.DataStoreReader.GetSeries(referencedUid);
                    IEnumerable<ISopInstance> sops = series.GetSopInstances();
                    foreach (ISopInstance sop in sops)
                    {
                        AddSopInstanceIntoParcel(sop);
                    }
                }
            }
            else // study was found
            {
                IStudy study = this.DataStoreReader.GetStudy(referencedUid);
                IEnumerable<ISopInstance> sops = study.GetSopInstances();
                foreach (ISopInstance sop in sops)
                {
                    AddSopInstanceIntoParcel(sop);
                }
            }

            int afterIncludeObjectCount = (this.SopInstances as List<ISopInstance>).Count;
            return afterIncludeObjectCount - currentObjectCount;
        }

        public ParcelTransferState GetState()
        {
            return this.ParcelTransferState;
        }

		//public void StartSend()
		//{
		//    try
		//    {
		//        DicomClient client = new DicomClient(this.SourceAE);
		//        client.SendProgressUpdated += delegate(object source, SendProgressUpdatedEventArgs args)
		//                {
		//                    //WCF TODO: Update Move status to the ActivityService

		//                    this.TotalProgressSteps = args.TotalCount;
		//                    this.CurrentProgressStep = args.CurrentCount;
		//                };

		//        client.Store(this.DestinationAE, this.SopInstanceFilenamesList, this.SopClassesList, this.TransferSyntaxesList);

		//        this.ParcelTransferState = ParcelTransferState.Completed;
		//    }
		//    catch (Exception e)
		//    {
		//        Platform.Log(e, LogLevel.Error);
		//    }
		//}

		//public void StopSend()
		//{
		//    throw new Exception("TODO: The method or operation is not implemented.");
		//}

        public int GetToSendObjectCount()
        {
            return _sopInstances.Count;
        }

        public int SentObjectCount()
        {
            // This may 'not' necessarily be the same as the CurrentProgressStep
            return this.CurrentProgressStep;
        }

        public IEnumerable<string> GetReferencedSopInstanceFileNames()
        {
            return this.SopInstanceFilenamesList;
        }

        #endregion
    }
}
