using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using ClearCanvas.Dicom;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal class PropertyFilters<T> where T : class
    {
        private readonly DicomAttributeCollection _criteria;
        private readonly IList<IPropertyFilter<T>> _filters;

        public PropertyFilters(DicomAttributeCollection criteria)
        {
            _criteria = criteria;
            _filters = new ReadOnlyCollection<IPropertyFilter<T>>(CreateFilters(criteria));
        }

        public IEnumerable<T> Query(Table<T> table)
        {
            var query = table.AsQueryable();
            foreach (var filter in _filters)
                query = filter.AddToQuery(query);

            var results = query.AsEnumerable();
            return FilterResults(results);
        }

        public IEnumerable<T> FilterResults(IEnumerable<T> results)
        {
            foreach (var filter in _filters)
                results = filter.FilterResults(results);

            return results;
        }

        public List<DicomAttributeCollection> ConvertResults(IEnumerable<T> results)
        {
            var dicomResults = new List<DicomAttributeCollection>();
            foreach (var result in results)
            {
                var dicomResult = new DicomAttributeCollection();
                foreach (var filter in _filters)
                    filter.SetAttributeValue(result, dicomResult);

                dicomResults.Add(dicomResult);
            }
            return dicomResults;
        }

        public static List<IPropertyFilter<T>> CreateFilters(DicomAttributeCollection criteria)
        {
            var filters = new List<IPropertyFilter<T>>();
            var types = typeof (PropertyFilters<T>).Assembly.GetTypes()
                            .Where(t => typeof (IPropertyFilter<T>).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(new[] {typeof (DicomAttributeCollection)});
                if (constructor != null)
                    filters.Add((IPropertyFilter<T>)Activator.CreateInstance(type, new[]{criteria}));
            }

            return filters;
        }
    }
}
