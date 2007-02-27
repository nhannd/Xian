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

namespace ClearCanvas.ImageViewer.Shreds
{
    [ExtensionPoint]
    public class StudyDataAccessExtensionPoint : ExtensionPoint<IDiskspaceManagement>
    {
    }

    public class DiskspaceManagement : IDiskspaceManagement
    {
        protected DiskspaceManagementComponent _component;

        public DiskspaceManagement() { }

        public DiskspaceManagement(DiskspaceManagementComponent component)
        {
            Platform.CheckForNullReference(component, "component");
            _component = component;
        }

        #region IDiskspaceManagementInterface Members

        public void SetComponent(DiskspaceManagementComponent component)
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

    [ExtensionOf(typeof(StudyDataAccessExtensionPoint))]
    public class StudyDataAccess : DiskspaceManagement
    {
        public StudyDataAccess() 
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
                if (!studyItem.Status.Equals(DiskspaceManagementStatus.ExistsOnLocalDrive))
                    continue;
                IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyItem.StudyInstanceUID));
                try
                {
                    DataAccessLayer.GetIDataStoreWriter().RemoveStudy(study);
                    deletedNumber += 1;
                    studyItem.Status = DiskspaceManagementStatus.DeletedFromDatabase;
                }
                catch (Exception e)
                {
                    Platform.Log(e, LogLevel.Error);
                }
            }
            Platform.Log("    Studies deleted in DB: " + deletedNumber);
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

            ReadOnlyQueryResultCollection studyResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(new QueryKey());

            if (studyResults == null || studyResults.Count <= 0)
            {
                Platform.Log("    There is not any study in DataStore.");
                _component.IsProcessing = false;
                return false;
            }

            foreach (QueryResult studyResult in studyResults)
            {
                DMStudyItem studyItem = new DMStudyItem();
                studyItem.AccessionNumber = studyResult.AccessionNumber;
                studyItem.StudyInstanceUID = studyResult.StudyInstanceUid;
                studyItem.CreatedTimeStamp = studyResult.CreatedTimeStamp;
                studyItem.SopItemList = new List<DMSopItem>();
                IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyItem.StudyInstanceUID));
                foreach (ISopInstance sop in study.GetSopInstances())
                {
                    DMSopItem sopItem = new DMSopItem();
                    sopItem.SopInstanceUID = sop.GetSopInstanceUid();
                    sopItem.LocationUri = sop.GetLocationUri().LocalDiskPath;
                    studyItem.SopItemList.Add(sopItem);
                }
                studyItem.Status = DiskspaceManagementStatus.ExistsInDatabase;
                _component.OrderedStudyList.Add(studyItem);
            }
            _component.OrderedStudyList.Sort(delegate(DMStudyItem s1, DMStudyItem s2)
            { return s1.CreatedTimeStamp.CompareTo(s2.CreatedTimeStamp); });
            
            return true;
        }

        #region Fields


        #endregion

    }
}
