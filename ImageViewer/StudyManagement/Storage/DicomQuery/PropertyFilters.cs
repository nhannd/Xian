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
        private readonly DicomAttributeCollection _inputCriteria;
        private readonly IList<IPropertyFilter<T>> _filters;

        protected PropertyFilters(DicomAttributeCollection inputCriteria, IEnumerable<IPropertyFilter<T>> filters)
        {
            _inputCriteria = inputCriteria;
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
        public StudyPropertyFilters(DicomAttributeCollection inputCriteria)
            : base(inputCriteria, CreateFilters(inputCriteria).Where(f => f.InputCriterion != null))
        {
        }

        private static IPropertyFilter<Study>[] CreateFilters(DicomAttributeCollection inputCriteria)
        {
            return new IPropertyFilter<Study>[]
                       {
                           //TODO (Marmot): Fill this in.
                           new PatientIdFilter(inputCriteria),
                           new PatientsNameFilter(inputCriteria),
                           new PatientsBirthDateFilter(inputCriteria),
                           new PatientsSexFilter(inputCriteria),

                           new StudyInstanceUidFilter(inputCriteria),
                           new StudyDateFilter(inputCriteria),
                           new StudyIdFilter(inputCriteria),
                           new AccessionNumberFilter(inputCriteria),
                           new StudyDescriptionFilter(inputCriteria),
                           
                           new ReferringPhysiciansNameFilter(inputCriteria),
                           
                           new NumberOfStudyRelatedInstancesFilter(inputCriteria),
                           new NumberOfStudyRelatedSeriesFilter(inputCriteria)
                       };
        }
    }

    internal class SeriesPropertyFilters : PropertyFilters<Series>
    {
        public SeriesPropertyFilters(DicomAttributeCollection inputCriteria)
            : base(inputCriteria, CreateFilters(inputCriteria).Where(f => f.InputCriterion != null))
        {
        }

        private static IPropertyFilter<Series>[] CreateFilters(DicomAttributeCollection inputCriteria)
        {
            return new IPropertyFilter<Series>[]
                                          {
                                              //TODO (Marmot): Fill this in.
                                          };
        }
    }

    internal class SopInstancePropertyFilters : PropertyFilters<SopInstance>
    {
        public SopInstancePropertyFilters(DicomAttributeCollection inputCriteria)
            : base(inputCriteria, CreateFilters(inputCriteria).Where(f => f.InputCriterion != null))
        {
        }

        private static IPropertyFilter<SopInstance>[] CreateFilters(DicomAttributeCollection inputCriteria)
        {
            return new IPropertyFilter<SopInstance>[]
                                          {
                                              //TODO (Marmot): Fill this in.
                                          };
        }
    }
}
