using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Automation
{
    public interface IWorkspaceLayout
    {
        void SelectImageBoxAt(RectangularGrid.Location imageBoxLocation);
        void SelectTileAt(RectangularGrid.Location tileLocation);

        void SetLayout(RectangularGrid layout);
        void SetSelectedImageBoxLayout(RectangularGrid layout);

        IImageBox GetSelectedImageBox(out RectangularGrid.Location imageBoxLocation);
        ITile GetSelectedTile(out RectangularGrid.Location tileLocation);

        RectangularGrid GetLayout();
        RectangularGrid GetImageBoxLayoutAt(RectangularGrid.Location imageBoxLocation);

        IImageBox GetImageBoxAt(RectangularGrid.Location imageBoxLocation);
        ITile GetTileAt(RectangularGrid.Location tileLocation);
    }
}
