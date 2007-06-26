using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [ButtonAction("search", "folderexplorer-folders-toolbar/Search")]
    [ButtonAction("search", "folders-toolbar/Search")]
    [Tooltip("search", "Search for a patient")]
	[IconSet("search", IconScheme.Colour, "Icons.SearchPatientToolSmall.png", "Icons.SearchPatientToolMedium.png", "Icons.SearchPatientToolLarge.png")]
    [ClickHandler("search", "Search")]

    [ExtensionOf(typeof(RegistrationWorkflowFolderToolExtensionPoint))]
    public class PatientSearchTool : Tool<IRegistrationWorkflowFolderToolContext>
    {
        private PatientSearchComponent _searchComponent;

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        public void Search()
        {
            if (_searchComponent == null)
            {
                try
                {
                    _searchComponent = new PatientSearchComponent();
                    _searchComponent.SearchRequested += SearchRequestedEventHandler;

                    ApplicationComponent.LaunchAsShelf(
                        this.Context.DesktopWindow,
                        _searchComponent,
                        SR.TitleSearch,
                        ShelfDisplayHint.DockFloat,
                        delegate(IApplicationComponent c)
                        {
                            _searchComponent.SearchRequested -= SearchRequestedEventHandler;
                            _searchComponent = null;
                        });
                }
                catch (Exception e)
                {
                    // cannot start component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }

        private void SearchRequestedEventHandler(object sender, PatientSearchRequestedEventArgs e)
        {
            this.Context.SearchCriteria = _searchComponent.SearchCriteria;
        }
    }
}
