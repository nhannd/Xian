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
        protected DiskspaceManagerComponent _component;

        public DiskspaceManager() { }

        public DiskspaceManager(DiskspaceManagerComponent component)
        {
            Platform.CheckForNullReference(component, "component");
            _component = component;
        }

        #region IDiskspaceManagerInterface Members

        public void SetComponent(DiskspaceManagerComponent component)
        {
            _component = component;
            OrderedStudyListRequiredEvent += RetrieveStudyHandler;
            DeleteStudyInDBRequiredEvent += DeleteStudyInDBHandler;
        }

        public DMStudyItemList OrderedStudyList
        {
            get { return _component.OrderedStudyList; }
            set { _component.OrderedStudyList = value; }
        }

        public bool IsProcessing
        {
            get { return _component.IsProcessing; }
            set { _component.IsProcessing = value; }
        }

        public event EventHandler DeleteStudyInDBRequiredEvent
        {
            add { _component.DeleteStudyInDBRequiredEvent += value; }
            remove { _component.DeleteStudyInDBRequiredEvent -= value; }
        }

        public event EventHandler OrderedStudyListRequiredEvent
        {
            add { _component.OrderedStudyListRequiredEvent += value; }
            remove { _component.OrderedStudyListRequiredEvent -= value; }
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
            _component.FireDeleteStudyInDBCompleted();
        }

        public void DeleteStudyInDB()
        {
            int deletedNumber = 0;
            foreach (DMStudyItem studyItem in _component.OrderedStudyList)
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
                _component.FireOrderedStudyListReady();
        }

        public bool RetrieveOrderedStudy()
        {
            _component.OrderedStudyList = new DMStudyItemList();

            IList<IStudy> studiesFound = DataAccessLayer.GetIDataStoreReader().GetStudies();

            if (studiesFound == null || studiesFound.Count <= 0)
            {
                //Platform.Log("    There is not any study in DataStore.");
                _component.IsProcessing = false;
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
                _component.OrderedStudyList.Add(studyItem);
            }
            _component.OrderedStudyList.Sort(delegate(DMStudyItem s1, DMStudyItem s2)
            { return s1.StoreTime.CompareTo(s2.StoreTime); });
            
            return true;
        }

    }
}
