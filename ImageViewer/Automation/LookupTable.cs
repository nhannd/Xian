using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Automation
{
    public interface ILookupTable
    {
        IVoiLut SetWindowLevel(double windowWidth, double windowCentre);
        IVoiLut ApplyPreset(string name);
        IVoiLut GetLookupTableAt(RectangularGrid.Location tileLocation);
    }
}