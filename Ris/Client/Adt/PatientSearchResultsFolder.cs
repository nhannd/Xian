using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientSearchResultsFolder : Folder
    {
        private PatientProfileTable _searchResults;
        private PatientProfileSearchCriteria _searchCriteria;
        private IAdtService _adtService;

        public PatientSearchResultsFolder()
            :base("Search Results")
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _searchResults = new PatientProfileTable();
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

        public override ITable Items
        {
            get
            {
                return _searchResults;
            }
        }

        private void DoSearch()
        {
            _searchResults.Items.Clear();
            if (_searchCriteria != null)
            {
                try
                {
                    _searchResults.Items.AddRange(_adtService.ListPatientProfiles(_searchCriteria));
                }
                catch (Exception e)
                {
                    //TODO we need a more formalized means of handling query service layer exceptions
                    Platform.Log(e);
                    Platform.ShowMessageBox("An error occured while trying to execute the query");
                }
            }
        }

    }
}
