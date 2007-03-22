using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.DataStore.NHibernateDriver;

using System.Collections;
using System.Reflection;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Expression;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    [ExtensionPoint]
    public class DiskspaceManagerDBAccessExtensionPoint : ExtensionPoint<IDiskspaceManager>
    {
    }

    public class DiskspaceManager : IDiskspaceManager
    {
        protected DiskspaceManagerData _diskspaceManagerData;

        public DiskspaceManager() { }

        public DiskspaceManager(DiskspaceManagerData diskspaceManagerData)
        {
            Platform.CheckForNullReference(diskspaceManagerData, "diskspaceManagerData");
            _diskspaceManagerData = diskspaceManagerData;
        }

        #region IDiskspaceManagerInterface Members

        public void SetComponent(DiskspaceManagerData diskspaceManagerData)
        {
            _diskspaceManagerData = diskspaceManagerData;
            OrderedStudyListRequiredEvent += RetrieveStudyHandler;
            DeleteStudyInDBRequiredEvent += DeleteStudyInDBHandler;
        }

        public DMStudyItemList OrderedStudyList
        {
            get { return _diskspaceManagerData.OrderedStudyList; }
            set { _diskspaceManagerData.OrderedStudyList = value; }
        }

        public bool IsProcessing
        {
            get { return _diskspaceManagerData.IsProcessing; }
            set { _diskspaceManagerData.IsProcessing = value; }
        }

        public event EventHandler DeleteStudyInDBRequiredEvent
        {
            add { _diskspaceManagerData.DeleteStudyInDBRequiredEvent += value; }
            remove { _diskspaceManagerData.DeleteStudyInDBRequiredEvent -= value; }
        }

        public event EventHandler OrderedStudyListRequiredEvent
        {
            add { _diskspaceManagerData.OrderedStudyListRequiredEvent += value; }
            remove { _diskspaceManagerData.OrderedStudyListRequiredEvent -= value; }
        }

        public virtual void DeleteStudyInDBHandler(object sender, EventArgs args)
        {
        }

        public virtual void RetrieveStudyHandler(object sender, EventArgs args)
        {
        }

        #endregion
    }

    [ExtensionOf(typeof(DiskspaceManagerDBAccessExtensionPoint))]
    public class DiskspaceManagerDBAccess : DiskspaceManager
    {
        public DiskspaceManagerDBAccess() 
        {
        }

        public override void DeleteStudyInDBHandler(object sender, EventArgs args)
        {
            DeleteStudyInDB();
            _diskspaceManagerData.FireDeleteStudyInDBCompleted();
        }

        public void DeleteStudyInDB()
        {
            int deletedNumber = 0;
            foreach (DMStudyItem studyItem in _diskspaceManagerData.OrderedStudyList)
            {
                if (!studyItem.Status.Equals(DiskspaceManagerStatus.ExistsOnDrive))
                    continue;
                IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyItem.StudyInstanceUID));
                try
                {
                    DataAccessLayer.GetIDataStoreWriter().RemoveStudy(study);
                    deletedNumber += 1;
                    studyItem.Status = DiskspaceManagerStatus.DeletedFromDatabase;
                    Platform.Log("    Study deleted in DB " + deletedNumber + ") StudyInstanceUid: " + studyItem.StudyInstanceUID);
                }
                catch (Exception e)
                {
                    Platform.Log(e, LogLevel.Error);
                }
            }
            if (deletedNumber > 0)
                Platform.Log("    Total studies deleted in DB: " + deletedNumber);
            return;
        }

        public override void RetrieveStudyHandler(object sender, EventArgs args)
        {
            if(RetrieveOrderedStudy())
                _diskspaceManagerData.FireOrderedStudyListReady();
        }

        public bool RetrieveOrderedStudy()
        {
            _diskspaceManagerData.OrderedStudyList = new DMStudyItemList();

            IList<IStudy> studiesFound = DataAccessLayer.GetIDataStoreReader().GetStudies();

            if (studiesFound == null || studiesFound.Count <= 0)
            {
                //Platform.Log("    There is not any study in DataStore.");
                _diskspaceManagerData.IsProcessing = false;
                return false;
            }

            foreach (Study studyFound in studiesFound)
            {
                DMStudyItem studyItem = new DMStudyItem();
                studyItem.AccessionNumber = studyFound.AccessionNumber;
                studyItem.StudyInstanceUID = studyFound.StudyInstanceUid;
                studyItem.StoreTime = studyFound.StoreTime;
                studyItem.SopItemList = new List<DMSopItem>();
                IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyItem.StudyInstanceUID));
                foreach (ISopInstance sop in study.GetSopInstances())
                {
                    DMSopItem sopItem = new DMSopItem();
                    sopItem.SopInstanceUID = sop.GetSopInstanceUid();
                    sopItem.LocationUri = sop.GetLocationUri().LocalDiskPath;
                    studyItem.SopItemList.Add(sopItem);
                }
                studyItem.Status = DiskspaceManagerStatus.ExistsInDatabase;
                _diskspaceManagerData.OrderedStudyList.Add(studyItem);
            }
            _diskspaceManagerData.OrderedStudyList.Sort(delegate(DMStudyItem s1, DMStudyItem s2)
            { return s1.StoreTime.CompareTo(s2.StoreTime); });
            
            return true;
        }

    }
}
