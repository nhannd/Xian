using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class SearchController : BaseController
    {
        #region Private Members
        private readonly SearchAdaptor _adaptor = new SearchAdaptor();
        private readonly SeriesSearchAdaptor _seriesAdaptor = new SeriesSearchAdaptor();
        #endregion

        #region Public Methods
        public IList<Study> GetStudies(StudySelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<Series> GetSeries(Study study)
        {
            SeriesSelectCriteria criteria = new SeriesSelectCriteria();

            criteria.StudyKey.EqualTo(study.GetKey());

            return _seriesAdaptor.Get(criteria);
        }
        #endregion

    }
}
