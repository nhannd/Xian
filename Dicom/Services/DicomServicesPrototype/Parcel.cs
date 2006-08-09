using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// Allows access to the DataStore in a way that's not specific to Study, Series or SopInstance
    /// </summary>
    public class Parcel : IParcel
    {
        public Parcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE) : this()
        {
            _sourceAE = sourceAE;
            _destinationAE = destinationAE;
        }

        protected Parcel()
        {
            _transferSyntaxes = new List<string>();
            _sopClasses = new List<string>();
            _sopInstances = new List<ISopInstance>();
        }

        public virtual IDataStore DataStore
        {
            get
            {
                return DataAbstractionLayer.GetIDataStore();
            }
        }

        protected virtual long ParcelOid
        {
            get { return _parcelOid; }
            set { _parcelOid = value; }
        }

        protected virtual ApplicationEntity DestinationAE
        {
            get { return _destinationAE; }
            set { _destinationAE = value; }
        }

        protected virtual ApplicationEntity SourceAE
        {
            get { return _sourceAE; }
            set { _sourceAE = value; }
        }

        internal virtual IList TransferSyntaxes
        {
            get { return _transferSyntaxes; }
        }

        internal virtual IList SopClasses
        {
            get { return _sopClasses; }
        }

        internal virtual IList SopInstances
        {
            get { return _sopInstances; }
        }

        #region IParcel Members

        /// <summary>
        /// Allows client to include a particular DICOM entity, whether a Study, Series or Sop Instance
        /// in a parcel for sending to a remote AE
        /// </summary>
        /// <param name="referencedUid">The UID of the entity, whether a Study, Series or Sop Instance.
        /// If the UID refers to a Study or Series, all the associasted Sop Instances of that Study or
        /// Series will be included in the parcel.
        /// </param>
        /// <returns>The number of new Sop Instances that were added.</returns>
        public int Include(Uid referencedUid)
        {
            // store current count of objects
            int currentObjectCount = (this.SopInstances as List<ISopInstance>).Count;

            // search for study that matches Uid
            if (!this.DataStore.StudyExists(referencedUid))
            {

                // search for series that matches Uid
                if (!this.DataStore.SeriesExists(referencedUid))
                {
                    // search for SOP instance that matches Uid
                    if (!this.DataStore.SopInstanceExists(referencedUid))
                    {
                        return 0;
                    }
                    else
                    {
                        // get the SopInstance object
                        ISopInstance sop = this.DataStore.GetSopInstance(referencedUid);
                        AddSopInstanceIntoParcel(sop);
                    }
                }
                else // series was found
                {
                    ISeries series = this.DataStore.GetSeries(referencedUid);
                    IEnumerable<ISopInstance> sops = series.GetSopInstances();
                    foreach (ISopInstance sop in sops)
                    {
                        AddSopInstanceIntoParcel(sop);
                    }
                }
            }
            else // study was found
            {
                IStudy study = this.DataStore.GetStudy(referencedUid);
                IEnumerable<ISopInstance> sops = study.GetSopInstances();
                foreach (ISopInstance sop in sops)
                {
                    AddSopInstanceIntoParcel(sop);
                }
            }

            int afterIncludeObjectCount = (this.SopInstances as List<ISopInstance>).Count;
            return afterIncludeObjectCount - currentObjectCount;
        }

        #endregion

        #region Internal and Private members
        private void AddTransferSyntax(Uid newTransferSyntax)
        {
            string foundMatch = Parcel.Find((this.TransferSyntaxes as List<string>), newTransferSyntax);

            if (null == foundMatch)
            {
                (this.TransferSyntaxes as List<string>).Add(newTransferSyntax);
            }
        }

        private void AddSopClass(Uid newSopClass)
        {
            string foundMatch = Parcel.Find((this.SopClasses as List<string>), newSopClass);

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

        private ApplicationEntity _destinationAE;
        private ApplicationEntity _sourceAE;
        private IList _transferSyntaxes;
        private IList _sopClasses;
        private IList _sopInstances;
        private long _parcelOid;
        #endregion
    }
}
