using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    public class SearchResultsFolder : Folder
    {
        private PatientProfileTableData _searchResults;
        private PatientProfileSearchCriteria _searchCriteria;
        private IAdtService _adtService;

        public SearchResultsFolder()
            :base("Search Results")
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _searchResults = new PatientProfileTableData(_adtService);
        }

        public PatientProfileSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                DoSearch();
                NotifyItemsChanged();
            }
        }

        public override ClearCanvas.Desktop.ITableData Items
        {
            get
            {
                return _searchResults;
            }
        }

        private void DoSearch()
        {
            _searchResults.Clear();
            if (_searchCriteria != null)
            {
                _searchResults.AddRange(_adtService.ListPatientProfiles(_searchCriteria));
            }
        }

    }
}
