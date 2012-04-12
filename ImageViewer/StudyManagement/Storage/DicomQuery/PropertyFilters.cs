using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using ClearCanvas.Dicom;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal class PropertyFilters<T> where T : class
    {
        private readonly DicomAttributeCollection _criteria;
        private IList<IPropertyFilter<T>> _filters;

        public PropertyFilters(DicomAttributeCollection criteria)
        {
            _criteria = criteria;
        }

        private IEnumerable<IPropertyFilter<T>> Filters
        {
            get { return _filters ?? (_filters = CreateFilters(_criteria)); }
        }

        protected virtual List<IPropertyFilter<T>> CreateFilters(DicomAttributeCollection criteria)
        {
            var filters = new List<IPropertyFilter<T>>();
            var types = typeof(PropertyFilters<T>).Assembly.GetTypes()
                            .Where(t => typeof(IPropertyFilter<T>).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(new[] { typeof(DicomAttributeCollection) });
                if (constructor != null)
                    filters.Add((IPropertyFilter<T>)Activator.CreateInstance(type, new object[] { criteria }));
            }

            return filters;
        }

        protected virtual IQueryable<T> Query(IQueryable<T> initialQuery)
        {
            return Filters.Aggregate(initialQuery, (current, filter) => filter.AddToQuery(current));
        }

        public IEnumerable<T> Query(Table<T> table)
        {
            var query = Query(table.AsQueryable());
            var results = query.AsEnumerable();
            return FilterResults(results);
        }

        public IEnumerable<T> FilterResults(IEnumerable<T> items)
        {
            return Filters.Aggregate(items, (current, filter) => filter.FilterResults(current));
        }

        public List<DicomAttributeCollection> ConvertResults(IEnumerable<T> results)
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
}
