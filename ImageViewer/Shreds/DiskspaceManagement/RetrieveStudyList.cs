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
    public class RetrieveStudyListExtensionPoint : ExtensionPoint<IDiskspaceManagement>
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

        public event EventHandler OrderedStudyListReadyEvent
        {
            add { _component.OrderedStudyListReadyEvent += value; }
            remove { _component.OrderedStudyListReadyEvent -= value; }
        }

        public event EventHandler OrderedStudyListRequiredEvent
        {
            add { _component.OrderedStudyListRequiredEvent += value; }
            remove { _component.OrderedStudyListRequiredEvent -= value; }
        }

        public void FireOrderedStudyListReady()
        {
            _component.FireOrderedStudyListReady();
        }

        public virtual void RetrieveStudyHandler(object sender, EventArgs args)
        {
        }

        #endregion
    }

    [ExtensionOf(typeof(RetrieveStudyListExtensionPoint))]
    public class RetrieveStudyList : DiskspaceManagement
    {
        public RetrieveStudyList() 
        {
        }

        public override void RetrieveStudyHandler(object sender, EventArgs args)
        {
            RetrieveOrderedStudy();
            FireOrderedStudyListReady();
        }

        public void RetrieveOrderedStudy()
        {
            _component.OrderedStudyList = new DMStudyItemList();

            ReadOnlyQueryResultCollection studyResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(new QueryKey());
            
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
                    sopItem.LocationUri = sop.GetLocationUri();
                    studyItem.SopItemList.Add(sopItem);
                }
                studyItem.Status = DiskspaceManagementStatus.ExistsInDatabase;
                _component.OrderedStudyList.Add(studyItem);
            }
            _component.OrderedStudyList.Sort(delegate(DMStudyItem s1, DMStudyItem s2)
            { return s1.CreatedTimeStamp.CompareTo(s2.CreatedTimeStamp); });
            
            return;
        }

        #region Fields


        #endregion

    }
}
