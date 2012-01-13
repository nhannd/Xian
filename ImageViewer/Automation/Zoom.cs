using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Automation
{
    public interface IZoom
    {
        void SetZoom(double value);
        double GetZoomAt(RectangularGrid.Location tileLocation);
    }
}