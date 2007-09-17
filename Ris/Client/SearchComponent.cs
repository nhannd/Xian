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
        private bool _searchEnabled;
        private event EventHandler _searchEnabledChanged;

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
                    _searchComponentShelf = LaunchAsShelf(
                        desktopWindow,
                        SearchComponent.Instance,
                        SR.TitleSearch,
                        ShelfDisplayHint.DockFloat,
                        delegate
                            {
                                _searchComponentShelf = null;
                            });
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

        public bool KeepOpen
        {
            get { return _keepOpen; }
            set { _keepOpen = value; }
        }

        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            protected set
            {
                if (_searchEnabled != value)
                {
                    _searchEnabled = value;
                    EventsHelper.Fire(_searchEnabledChanged, this, new EventArgs());
                }
            }
        }

        public event EventHandler SearchEnabledChanged
        {
            add { _searchEnabledChanged += value; }
            remove { _searchEnabledChanged -= value; }
        }

        public void Search()
        {
            if (!this.HasValidationErrors)
            {
                // Pass the SearchData to the component hosted inthe current active workspace, 
                // if the component implements the ISearchDataHandler interface
                Workspace activeWorkspace = _searchComponentShelf.DesktopWindow.ActiveWorkspace;
                if (activeWorkspace != null && activeWorkspace.Component is ISearchDataHandler)
                    ((ISearchDataHandler) (activeWorkspace.Component)).SearchData = this.SearchData;

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

        private void UpdateDisplay()
        {
            // ensure the criteria is specific enough before enabling search
            this.SearchEnabled = _mrn != null || _healthcard != null || _familyName != null || _givenName != null || _accessionNumber != null;
        }

        private SearchData BuildSearchData()
        {
            SearchData searchData = new SearchData();

            searchData.MrnID = _mrn;
            searchData.HealthcardID = _healthcard;
            searchData.FamilyName = _familyName;
            searchData.GivenName = _givenName;
            searchData.AccessionNumber = _accessionNumber;
            searchData.ShowActiveOnly = _showActiveOnly;

            return searchData;
        }
    }
}
