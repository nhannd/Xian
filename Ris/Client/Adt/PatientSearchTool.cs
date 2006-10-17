using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [ButtonAction("search", "folders-toolbar/Search")]
    [Tooltip("search", "Search for a patient")]
    [IconSet("search", IconScheme.Colour, "Icons.SearchToolSmall.png", "Icons.SearchToolMedium.png", "Icons.SearchToolLarge.png")]
    [ClickHandler("search", "Search")]

    [ExtensionOf(typeof(FolderToolExtensionPoint))]
    public class PatientSearchTool : Tool<IFolderToolContext>
    {
        private PatientSearchComponent _searchComponent;
        private PatientSearchResultsFolder _folder;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PatientSearchTool()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
            this.Context.Folders.Add(_folder = new PatientSearchResultsFolder());
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
