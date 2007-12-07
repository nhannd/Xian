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
        #endregion

        #region Public Methods
        public IList<Study> GetStudies(StudySelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }
        #endregion
    }
}
