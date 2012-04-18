using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using ClearCanvas.Dicom;
using System;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal class PropertyFilters<TDatabaseObject, TStoreEntry>
        where TDatabaseObject : class
        where TStoreEntry : StoreEntry, new()
    {
        private readonly DicomAttributeCollection _criteria;
        private IList<IPropertyFilter<TDatabaseObject, TStoreEntry>> _filters;

        public PropertyFilters(DicomAttributeCollection criteria)
        {
            _criteria = criteria;
        }

        private IEnumerable<IPropertyFilter<TDatabaseObject, TStoreEntry>> Filters
        {
            get { return _filters ?? (_filters = CreateFilters(_criteria)); }
        }

        protected virtual List<IPropertyFilter<TDatabaseObject, TStoreEntry>> CreateFilters(
            DicomAttributeCollection criteria)
        {
            var filters = new List<IPropertyFilter<TDatabaseObject, TStoreEntry>>();
            var types = typeof (PropertyFilters<TDatabaseObject, TStoreEntry>).Assembly.GetTypes()
                .Where(t => typeof (IPropertyFilter<TDatabaseObject, TStoreEntry>).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(new[] {typeof (DicomAttributeCollection)});
                if (constructor != null)
                    filters.Add(
                        (IPropertyFilter<TDatabaseObject, TStoreEntry>)
                        Activator.CreateInstance(type, new object[] {criteria}));
            }

            return filters;
        }

        protected virtual IQueryable<TDatabaseObject> Query(IQueryable<TDatabaseObject> initialQuery)
        {
            return Filters.Aggregate(initialQuery, (current, filter) => filter.AddToQuery(current));
        }

        public IEnumerable<TDatabaseObject> Query(Table<TDatabaseObject> table)
        {
            var query = Query(table.AsQueryable());
            var results = query.AsEnumerable(); //TODO (CR) - what is the reason for this line?
            return FilterResults(results);
        }

        public IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> items)
        {
            return Filters.Aggregate(items, (current, filter) => filter.FilterResults(current));
        }

        public List<DicomAttributeCollection> ConvertResultsToDataSets(IEnumerable<TDatabaseObject> results)
        {
            var dicomResults = new List<DicomAttributeCollection>();
            foreach (var result in results)
            {
                var dicomResult = new DicomAttributeCollection();
                foreach (var filter in Filters)
                    filter.SetAttributeValue(result, dicomResult);

                dicomResults.Add(dicomResult);
            }

            return dicomResults;
        }
    }

    internal class StudyPropertyFilters : PropertyFilters<Study, StudyEntry>
    {
        public StudyPropertyFilters(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }

        protected override List<IPropertyFilter<Study, StudyEntry>> CreateFilters(DicomAttributeCollection criteria)
        {
            var filters = base.CreateFilters(criteria);
            var modalitiesInStudyPath = new DicomTagPath(DicomTags.ModalitiesInStudy);
            var dicomFilters = filters.OfType<DicomPropertyFilter<Study, StudyEntry>>().ToList();
            var modalitiesInStudyIndex = dicomFilters.FindIndex(f => f.Path.Equals(modalitiesInStudyPath));
            var modalitiesInStudyFilter = filters[modalitiesInStudyIndex];

            //Because of the potentially complex joins of the same initial query over and over, move this one to the front.
            filters.RemoveAt(modalitiesInStudyIndex);
            filters.Insert(0, modalitiesInStudyFilter);
            return filters;
        }

        protected override IQueryable<Study> Query(IQueryable<Study> initialQuery)
        {
            var query = base.Query(initialQuery);

            //We don't want to return anything that is scheduled to be deleted.
            query = query.Where(s => !s.Deleted);

            return query;
        }
    }

    internal class SeriesPropertyFilters : PropertyFilters<Series, SeriesEntry>
    {
        public SeriesPropertyFilters(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstancePropertyFilters : PropertyFilters<SopInstance, ImageEntry>
    {
        public SopInstancePropertyFilters(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

}