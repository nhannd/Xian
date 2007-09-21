using System;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class HomePageContainer : SplitComponentContainer, ISearchDataHandler
    {
        private readonly FolderExplorerComponent _folderExplorerComponent;

        public HomePageContainer(FolderExplorerComponent folderExplorerComponent, IApplicationComponent previewComponent)
            : base(Desktop.SplitOrientation.Vertical)
        {
            _folderExplorerComponent = folderExplorerComponent;

            this.Pane1 = new SplitPane("Folders", folderExplorerComponent, 1.0f);
            this.Pane2 = new SplitPane("Preview", previewComponent, 1.0f);
        }

        #region ISearchHandler implementation

        public SearchData SearchData
        {
            set { _folderExplorerComponent.SearchData = value; }
        }

        public SearchField SearchFields
        {
            get { return _folderExplorerComponent.SearchFields; }
        }

        #endregion
    }
}
