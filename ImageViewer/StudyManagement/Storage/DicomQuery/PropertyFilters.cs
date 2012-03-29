using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal abstract class PropertyFilters<T> where T : class
    {
        private readonly DicomAttributeCollection _criteria;
        private readonly IList<IPropertyFilter<T>> _filters;

        protected PropertyFilters(DicomAttributeCollection criteria, IEnumerable<IPropertyFilter<T>> filters)
        {
            _criteria = criteria;
            _filters = new ReadOnlyCollection<IPropertyFilter<T>>(filters.ToList());
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
    }

    internal class StudyPropertyFilters : PropertyFilters<Study>
    {
        public StudyPropertyFilters(DicomAttributeCollection criteria)
            : base(criteria, CreateFilters(criteria).Where(f => !f.IsNoOp))
        {
        }

        private static IPropertyFilter<Study>[] CreateFilters(DicomAttributeCollection criteria)
        {
            return new IPropertyFilter<Study>[]
                       {
                           //TODO (Marmot): Fill this in.
                           new PatientIdFilter(criteria),
                           new PatientsNameFilter(criteria),
                           new PatientsBirthDateFilter(criteria),
                           new PatientsSexFilter(criteria),

                           new StudyInstanceUidFilter(criteria),
                           new StudyDateFilter(criteria),
                           new StudyIdFilter(criteria),
                           new AccessionNumberFilter(criteria),
                           new StudyDescriptionFilter(criteria),
                           
                           new ReferringPhysiciansNameFilter(criteria),
                           
                           new NumberOfStudyRelatedInstancesFilter(criteria),
                           new NumberOfStudyRelatedSeriesFilter(criteria)
                       };
        }
    }

    internal class SeriesPropertyFilters : PropertyFilters<Series>
    {
        public SeriesPropertyFilters(DicomAttributeCollection criteria)
            : base(criteria, CreateFilters(criteria).Where(f => !f.IsNoOp))
        {
        }

        private static IPropertyFilter<Series>[] CreateFilters(DicomAttributeCollection criteria)
        {
            return new IPropertyFilter<Series>[]
                                          {
                                              //TODO (Marmot): Fill this in.
                                          };
        }
    }

    internal class SopInstancePropertyFilters : PropertyFilters<SopInstance>
    {
        public SopInstancePropertyFilters(DicomAttributeCollection criteria)
            : base(criteria, CreateFilters(criteria).Where(f => !f.IsNoOp))
        {
        }

        private static IPropertyFilter<SopInstance>[] CreateFilters(DicomAttributeCollection criteria)
        {
            return new IPropertyFilter<SopInstance>[]
                                          {
                                              //TODO (Marmot): Fill this in.
                                          };
        }
    }
}
