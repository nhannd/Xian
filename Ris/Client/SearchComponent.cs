#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
