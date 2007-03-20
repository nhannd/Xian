using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientSearchResultsFolder : Folder
    {
        private PatientProfileTable _searchResults;
        private PatientProfileSearchCriteria _searchCriteria;
        private IAdtService _adtService;
        private PatientSearchComponent _searchComponent;

        public PatientSearchResultsFolder()
            :base(SR.TitleFolderSearchResults)
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

        public PatientSearchComponent SearchComponent
        {
            get { return _searchComponent; }
            set { _searchComponent = value; }
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
                    if (_searchComponent != null)
                    {
                        ExceptionHandler.Report(e, SR.ExceptionFailedToExecuteQuery, _searchComponent.DesktopWindow);
                    }
                    else
                    {
                        Platform.Log(e);
                        Platform.ShowMessageBox(SR.ExceptionFailedToExecuteQuery);
                    }
                }
            }
        }

    }
}
