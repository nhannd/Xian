using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [ButtonAction("search", "folderexplorer-folders-toolbar/Search")]
    [ButtonAction("search", "folders-toolbar/Search")]
    [Tooltip("search", "Search for a patient")]
    [IconSet("search", IconScheme.Colour, "Icons.SearchToolSmall.png", "Icons.SearchToolMedium.png", "Icons.SearchToolLarge.png")]
    [ClickHandler("search", "Search")]

    [ExtensionOf(typeof(FolderToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowFolderToolExtensionPoint))]
    public class PatientSearchTool : ToolBase
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
            if (this.ContextBase is IFolderToolContext)
            {
                ((IFolderToolContext)this.ContextBase).Folders.Add(_folder = new PatientSearchResultsFolder());
            }
            else if (this.ContextBase is IRegistrationWorkflowFolderToolContext)
            {

            }
        }

        public void Search()
        {
            if (this.ContextBase is IFolderToolContext)
            {
                IFolderToolContext context = (IFolderToolContext)this.ContextBase;

                if (_searchComponent == null)
                {
                    _searchComponent = new PatientSearchComponent();
                    _searchComponent.SearchRequested += SearchRequestedEventHandler;

                    // _folder need the searchComponent's Desktop Window to show exception
                    _folder.SearchComponent = _searchComponent;

                    ApplicationComponent.LaunchAsShelf(
                        context.DesktopWindow,
                        _searchComponent,
                        SR.TitleSearch,
                        ShelfDisplayHint.DockFloat,
                        delegate(IApplicationComponent c)
                        {
                            _searchComponent.SearchRequested -= SearchRequestedEventHandler;
                            _searchComponent = null;
                        });
                }
            }
            else if (this.ContextBase is IRegistrationWorkflowFolderToolContext)
            {
                Platform.ShowMessageBox("Do search");
            }

        }

        private void SearchRequestedEventHandler(object sender, PatientSearchRequestedEventArgs e)
        {
            if (this.ContextBase is IFolderToolContext)
            {
                IFolderToolContext context = (IFolderToolContext)this.ContextBase;
                _folder.SearchCriteria = _searchComponent.SearchCriteria;
                context.SelectedFolder = _folder;
            }
            else if (this.ContextBase is IRegistrationWorkflowFolderToolContext)
            {

            }
        }
    }
}
