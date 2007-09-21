using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface for handling the Search Data
    /// </summary>
    public interface ISearchDataHandler
    {
        SearchData SearchData { set; }
        SearchField SearchFields { get; }
    }

    [Flags]
    public enum SearchField
    {
        None = 0x00,
        Mrn = 0x01,
        Healthcard = 0x02,
        FamilyName = 0x04,
        GivenName = 0x08,
        AccessionNumber = 0x10
    }

    [ExtensionPoint]
    public class SearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(SearchComponentViewExtensionPoint))]
    public class SearchComponent : ApplicationComponent
    {
        private string _mrn;
        private string _healthcard;
        private string _familyName;
        private string _givenName;
        private string _accessionNumber;
        private bool _showActiveOnly;
        private bool _keepOpen;

        private SearchField _searchField;
        private bool _searchEnabled;

        private static SearchComponent _instance;
        private static Shelf _searchComponentShelf;

        private SearchComponent()
        {
        }

        public static Shelf Launch(IDesktopWindow desktopWindow)
        {
            try
            {
                if (_searchComponentShelf != null)
                {
                    _searchComponentShelf.Activate();
                }
                else
                {
                    desktopWindow.Workspaces.ItemActivationChanged += SearchComponent.Instance.Workspaces_ItemActivationChanged;

                    _searchComponentShelf = LaunchAsShelf(
                        desktopWindow,
                        SearchComponent.Instance,
                        SR.TitleSearch,
                        ShelfDisplayHint.DockFloat,
                        delegate
                            {
                                desktopWindow.Workspaces.ItemActivationChanged -= SearchComponent.Instance.Workspaces_ItemActivationChanged;
                                _searchComponentShelf = null;
                            });

                    SearchComponent.Instance.UpdateDisplay();
                }
            }
            catch (Exception e)
            {
                // cannot start component
                ExceptionHandler.Report(e, desktopWindow);
            }

            return _searchComponentShelf;
        }

        public static SearchComponent Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SearchComponent();

                return _instance;
            }
        }

        #region Public Member

        public SearchData SearchData
        {
            get { return BuildSearchData(); }
        }

        public string Mrn
        {
            get { return _mrn; }
            set
            {
                _mrn = value;

                UpdateDisplay();
            }
        }

        public string Healthcard
        {
            get { return _healthcard; }
            set
            {
                _healthcard = value;

                UpdateDisplay();
            }
        }

        public string HealthcardMask
        {
            get { return TextFieldMasks.HealthcardNumberMask; }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set
            {
                _familyName = value;
                UpdateDisplay();
            }
        }

        public string GivenName
        {
            get { return _givenName; }
            set
            {
                _givenName = value;
                UpdateDisplay();
            }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set
            {
                _accessionNumber = value;
                UpdateDisplay();
            }
        }

        public bool ShowActive
        {
            get { return _showActiveOnly; }
            set { _showActiveOnly = value; }
        }


        public bool UseMrn
        {
            get { return (_searchField & SearchField.Mrn) > 0; }
        }

        public bool UseHealthcard
        {
            get { return (_searchField & SearchField.Healthcard) > 0; }
        }

        public bool UseFamilyName
        {
            get { return (_searchField & SearchField.FamilyName) > 0; }
        }

        public bool UseGivenName
        {
            get { return (_searchField & SearchField.GivenName) > 0; }
        }

        public bool UseAccessionNumber
        {
            get { return (_searchField & SearchField.AccessionNumber) > 0; }
        }

        public bool KeepOpen
        {
            get { return _keepOpen; }
            set { _keepOpen = value; }
        }

        public SearchField SearchField
        {
            get { return _searchField; }
            set
            {
                if (_searchField != value)
                {
                    _searchField = value;
                }
            }
        }

        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            protected set
            {
                if (_searchEnabled != value)
                {
                    _searchEnabled = value;
                }
            }
        }

        public void Search()
        {
            if (!this.HasValidationErrors)
            {
                ISearchDataHandler searchHandler = GetActiveWorkspaceSearchHandler();
                if (searchHandler != null)
                    searchHandler.SearchData = this.SearchData;

                // always turn the validation errors off after a successful search
                this.ShowValidation(false);

                if (!_keepOpen)
                {
                    this.Host.Exit();
                }

            }
            else
            {
                this.ShowValidation(true);
            }
        }

        #endregion

        private void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
        {
            UpdateDisplay();
        }

        private ISearchDataHandler GetActiveWorkspaceSearchHandler()
        {
            Workspace activeWorkspace = _searchComponentShelf.DesktopWindow.ActiveWorkspace;
            if (activeWorkspace != null && activeWorkspace.Component is ISearchDataHandler)
                return (ISearchDataHandler) (activeWorkspace.Component);

            return null;
        }

        private void UpdateDisplay()
        {
            // ensure the criteria is specific enough before enabling search
            this.SearchEnabled = _mrn != null || _healthcard != null || _familyName != null || _givenName != null || _accessionNumber != null;

            ISearchDataHandler searchHandler = GetActiveWorkspaceSearchHandler();
            if (searchHandler != null)
            {
                SearchComponent.Instance.SearchField = searchHandler.SearchFields;

                SearchComponent.Instance.NotifyPropertyChanged("UseMrn");
                SearchComponent.Instance.NotifyPropertyChanged("UseHealthcard");
                SearchComponent.Instance.NotifyPropertyChanged("UseFamilyName");
                SearchComponent.Instance.NotifyPropertyChanged("UseGivenName");
                SearchComponent.Instance.NotifyPropertyChanged("UseAccessionNumber");
            }
        }

        private SearchData BuildSearchData()
        {
            SearchData searchData = new SearchData();

            searchData.MrnID = Instance.UseMrn ? _mrn : "";
            searchData.HealthcardID = Instance.UseHealthcard ? _healthcard : "";
            searchData.FamilyName = Instance.UseFamilyName ? _familyName : "";
            searchData.GivenName = Instance.UseGivenName ? _givenName : "";
            searchData.AccessionNumber = Instance.UseAccessionNumber ? _accessionNumber : "";
            searchData.ShowActiveOnly = Instance.UseAccessionNumber ? _showActiveOnly : false;

            return searchData;
        }
    }
}
