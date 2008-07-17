#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client
{
    public class SearchParams
    {
        public SearchParams(string textSearch, bool showActiveOnly)
        {
            this.TextSearch = textSearch;
            this.ShowActiveOnly = showActiveOnly;
        }

        public string TextSearch;
        public bool ShowActiveOnly;
    }

    /// <summary>
    /// Defines an interface for handling the Search Data
    /// </summary>
    public interface ISearchDataHandler
    {
        SearchParams SearchParams { set; }
    }

    [ExtensionPoint]
    public class SearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(SearchComponentViewExtensionPoint))]
    public class SearchComponent : ApplicationComponent
    {
        private string _searchField;
        private bool _showActiveOnly;
        private bool _keepOpen;

        private static SearchComponent _instance;
        private static Shelf _searchComponentShelf;

        private SearchComponent()
        {
            // True by default
            _showActiveOnly = true;
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
                    desktopWindow.Workspaces.ItemActivationChanged += Workspaces_ItemActivationChanged;

                    _searchComponentShelf = LaunchAsShelf(
                        desktopWindow,
                        Instance,
                        SR.TitleSearch,
                        ShelfDisplayHint.DockFloat);

                    _searchComponentShelf.Closed += delegate
                            {
                                desktopWindow.Workspaces.ItemActivationChanged -= Workspaces_ItemActivationChanged;
                                _searchComponentShelf = null;
                            };

                    UpdateDisplay();
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

        public string SearchField
        {
            get { return _searchField; }
            set
            {
                _searchField = value;
                UpdateDisplay();
            }
        }

        public bool ShowActiveOnly
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
            get { return !string.IsNullOrEmpty(_searchField); }
        }

        public void Search()
        {
            if (!this.HasValidationErrors)
            {
                ISearchDataHandler searchHandler = GetActiveWorkspaceSearchHandler();
                if (searchHandler != null)
                    searchHandler.SearchParams = new SearchParams(_searchField, _showActiveOnly);

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

        private static void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
        {
            UpdateDisplay();
        }

        private static ISearchDataHandler GetActiveWorkspaceSearchHandler()
        {
            Workspace activeWorkspace = _searchComponentShelf.DesktopWindow.ActiveWorkspace;
            if (activeWorkspace != null && activeWorkspace.Component is ISearchDataHandler)
                return (ISearchDataHandler) (activeWorkspace.Component);

            return null;
        }

        private static void UpdateDisplay()
        {
            ISearchDataHandler searchHandler = GetActiveWorkspaceSearchHandler();
            if (searchHandler != null)
            {
                // Use this to update the SearchComponentControl when a workspace that implement ISearchDataHandler is active
                // Do something here
            }
        }
    }
}
