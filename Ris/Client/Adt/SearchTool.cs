using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [ButtonAction("search", "folders-toolbar/SearchTool")]
    [Tooltip("search", "Find items")]
    [IconSet("search", IconScheme.Colour, "Icons.SearchToolSmall.png", "Icons.SearchToolMedium.png", "Icons.SearchToolLarge.png")]
    [ClickHandler("search", "Search")]

    [ExtensionOf(typeof(FolderToolExtensionPoint))]
    public class SearchTool : Tool<IFolderToolContext>
    {
        private PatientSearchComponent _searchComponent;
        private SearchResultsFolder _folder;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SearchTool()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
            this.Context.Folders.Add(_folder = new SearchResultsFolder());
        }

        public void Search()
        {
            if (_searchComponent == null)
            {
                _searchComponent = new PatientSearchComponent();
                _searchComponent.SearchRequested += SearchRequestedEventHandler;

                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    _searchComponent,
                    "Search",
                    ShelfDisplayHint.DockFloat,
                    delegate(IApplicationComponent c)
                    {
                        _searchComponent.SearchRequested -= SearchRequestedEventHandler;
                        _searchComponent = null;
                    });
            }

        }

        private void SearchRequestedEventHandler(object sender, PatientSearchRequestedEventArgs e)
        {
            _folder.SearchCriteria = _searchComponent.SearchCriteria;
            this.Context.SelectedFolder = _folder;
        }
    }
}
