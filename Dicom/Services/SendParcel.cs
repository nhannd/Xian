using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;
using NHibernate.Collection;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// Allows access to the DataStore in a way that's not specific to Study, Series or SopInstance
    /// </summary>
    public class SendParcel : Parcel, ISendParcel
    {
        public delegate void InitializeAssociatedCollectionCallback(object domainObject, PersistentCollection associatedCollection);
        public InitializeAssociatedCollectionCallback InitializeAssociatedCollection;

        public SendParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
            : base(sourceAE, destinationAE, parcelDescription)
        {
            _transferSyntaxes = new List<string>();
            _sopClasses = new List<string>();
            _internalSopInstances = new List<ISopInstance>();
        }

        private SendParcel() : base()
        {
        }

        public virtual IDataStoreReader DataStoreReader
        {
            get { return DataAccessLayer.GetIDataStoreReader(); }
        }

        public virtual IDicomSender DicomSender
        {
            get 
            {
                if (null == _dicomSender)
                    _dicomSender = DicomServicesLayer.GetIDicomSender();

                return _dicomSender; 
            }
        }

        public virtual ISendQueue SendQueue
        {
            get { return DicomServicesLayer.GetISendQueue(); }
        }

        public bool IsSendCompleted
        {
            get { return this.ParcelTransferState == ParcelTransferState.Completed; }
        }

        protected internal virtual IList TransferSyntaxes
        {
            get { return _transferSyntaxes; }
        }

        protected internal virtual IList SopClasses
        {
            get { return _sopClasses; }
        }


        protected internal virtual IList InternalSopInstances
        {
            get { return _internalSopInstances; }
        }

        public virtual IList SopInstances
        {
            get 
            {
                if (null != InitializeAssociatedCollection)
                    InitializeAssociatedCollection(this, _internalSopInstances as PersistentCollection);

                return _internalSopInstances; 
            }
        }

        #region Internal and Private members
        private void AddTransferSyntax(Uid newTransferSyntax)
        {
            string foundMatch = SendParcel.Find((this.TransferSyntaxes as List<string>), newTransferSyntax);

            if (null == foundMatch)
            {
                (this.TransferSyntaxes as List<string>).Add(newTransferSyntax);
            }
        }

        private void AddSopClass(Uid newSopClass)
        {
            string foundMatch = SendParcel.Find((this.SopClasses as List<string>), newSopClass);

            if (null == foundMatch)
            {
                (this.SopClasses as List<string>).Add(newSopClass);
            }
        }

        private void AddSopInstance(ISopInstance sop)
        {
            ISopInstance foundSop = (this.SopInstances as List<ISopInstance>).Find(
                    delegate(ISopInstance iteratedSop)
                    {
                        if (null != iteratedSop)
                            return sop.IsIdenticalTo(iteratedSop);
                        else
                            return false;
                    }
                );

            if (null == foundSop)
                (this.SopInstances as List<ISopInstance>).Add(sop);
        }

        private void AddSopInstanceIntoParcel(ISopInstance sop)
        {
            Uid transferSyntax = sop.GetTransferSyntaxUid();
            Uid sopClass = sop.GetSopClassUid();
            AddTransferSyntax(transferSyntax);
            AddSopClass(sopClass);
            AddSopInstance(sop);
        }

        private static string Find(List<string> listOfStrings, string stringToFind)
        {
            return listOfStrings.Find(
                delegate(string nextString)
                {
                    return nextString == stringToFind;
                }
            );
        }

        private IEnumerable<string> SopInstanceFilenamesList
        {
            get
            {
                List<string> sops = new List<string>();
                foreach (ISopInstance sop in _internalSopInstances)
                {
                    sops.Add(sop.GetLocationUri().LocalDiskPath);
                }
                return sops.AsReadOnly();
            }
        }

        private IEnumerable<string> SopClassesList
        {
            get
            {
                List<string> sopClasses = new List<string>();
                foreach (string sopClass in _sopClasses)
                {
                    sopClasses.Add(sopClass);
                }

                return sopClasses.AsReadOnly();
            }
        }

        private IEnumerable<string> TransferSyntaxesList
        {
            get
            {
                List<string> transferSyntaxes = new List<string>();
                foreach (string transferSyntax in _transferSyntaxes)
                {
                    transferSyntaxes.Add(transferSyntax);
                }

                return transferSyntaxes.AsReadOnly();
            }
        }

        private IList _transferSyntaxes;
        private IList _sopClasses;
        private IList _internalSopInstances;
        private IDicomSender _dicomSender;

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

        public void StartSend()
        {
            this.DicomSender.SetSourceApplicationEntity(this.SourceAE);
            this.DicomSender.SetDestinationApplicationEntity(this.DestinationAE);

            this.ParcelTransferState = ParcelTransferState.InProgress;
            this.SendQueue.UpdateParcel(this);
            try
            {
                this.DicomSender.Send(this.SopInstanceFilenamesList, 
                    this.SopClassesList, 
                    this.TransferSyntaxesList, 
                    delegate(object source, SendProgressUpdatedEventArgs args)
                    {
                        this.TotalProgressSteps = args.TotalCount;
                        this.CurrentProgressStep = args.CurrentCount;

                        this.SendQueue.UpdateParcel(this);
                    }
                    );

                this.ParcelTransferState = ParcelTransferState.Completed;
            }
            catch
            {
                // TODO: do some logging here
                this.ParcelTransferState = ParcelTransferState.Error;
            }

            this.SendQueue.UpdateParcel(this);        
        }

        public void StopSend()
        {
            throw new Exception("TODO: The method or operation is not implemented.");
        }

        public int GetToSendObjectCount()
        {
            return this.SopInstances.Count;
        }

        public int SentObjectCount()
        {
            return 0;
        }

        public IEnumerable<string> GetReferencedSopInstanceFileNames()
        {
            return this.SopInstanceFilenamesList;
        }

        #endregion
    }
}
