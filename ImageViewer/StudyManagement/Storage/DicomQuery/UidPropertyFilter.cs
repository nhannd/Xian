using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal abstract class UidPropertyFilter<TDatabaseObject, TStoreEntry> : DicomPropertyFilter<TDatabaseObject, TStoreEntry>
        where TDatabaseObject : class
        where TStoreEntry : StoreEntry
    {
        private string[] _criterionValues;

        protected UidPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
            : base(path, criteria)
        {
            Platform.CheckTrue(path.ValueRepresentation.Name == "UI", "Path is not VR=UI");
            if (Criterion != null)
                Platform.CheckTrue(Criterion.Tag.VR.Name == "UI", "Criteria is not VR=UI");
        }

        protected string[] CriterionValues
        {
            get
            {
                if (_criterionValues != null)
                    return _criterionValues;

                _criterionValues = DicomStringHelper.GetStringArray(Criterion.ToString()) ?? new string[0];
                return _criterionValues;
            }    
        }

        protected virtual IQueryable<TDatabaseObject> AddUidToQuery(IQueryable<TDatabaseObject> query, string uid)
        {
            throw new NotImplementedException("AddUidToQuery must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddUidsToQuery(IQueryable<TDatabaseObject> query, string[] uids)
        {
            throw new NotImplementedException("AddUidsToQuery must be implemented.");
        }

        protected override IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (CriterionValues.Length == 0)
                return base.AddToQuery(query);

            if (CriterionValues.Length == 1)
                return AddUidToQuery(query, CriterionValues[0]);

            return AddUidsToQuery(query, CriterionValues);
        }
    }
}
