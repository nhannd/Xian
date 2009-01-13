using System.Drawing;
using System.Drawing.Imaging;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

[MenuAction("compare", "clipboard-contextmenu/MenuCompareImage", "Compare")]
[ButtonAction("compare", "clipboard-toolbar/ToolbarCompareImage", "Compare")]
[Tooltip("compare", "TooltipCompareImage")]
[IconSet("compare", IconScheme.Colour, "Icons.CompareImageToolLarge.png", "Icons.CompareImageToolMedium.png", "Icons.CompareImageToolSmall.png")]
[EnabledStateObserver("compare", "Enabled", "EnabledChanged")]
[ExtensionOf(typeof(ClipboardToolExtensionPoint))]

public class CompareImageTool : ClipboardTool
{
    public CompareImageTool()
    {
    }

    public override void Initialize()
    {
        this.Enabled = this.Context.SelectedClipboardItems.Count > 0;
        base.Initialize();
    }

    public void Compare()
    {
        ImageGraphic firstGraphic = null;
        PixelData firstPixelData = null;
        PresentationImage firstPresentationImage = null;
        Bitmap firstBitmap = null;

        if (Context.SelectedClipboardItems.Count != 2)
        {
            this.Context.DesktopWindow.ShowMessageBox("Tool expects that exactly 2 images are selected.", MessageBoxActions.Ok);
            return;
        }

        foreach (IClipboardItem clipboardItem in this.Context.SelectedClipboardItems)
        {
            if (clipboardItem.Item is IPresentationImage)
            {
                IImageGraphicProvider provider = clipboardItem.Item as IImageGraphicProvider;
                ImageGraphic graphic = provider.ImageGraphic;

                if (firstGraphic == null)
                {
                    firstGraphic = graphic;
                    firstPixelData = graphic.PixelData;
                    firstPresentationImage = clipboardItem.Item as PresentationImage;
                    firstBitmap = firstPresentationImage.DrawToBitmap(firstGraphic.Rows, firstGraphic.Columns);
                }
                else
                {
                    //add checks the second image is the same colour, size, etc... as first
                    //and if not then exit

                    if (firstGraphic.Rows != graphic.Rows || firstGraphic.Columns != graphic.Columns)
                    {
                        this.Context.DesktopWindow.ShowMessageBox("Images are not the same size.", MessageBoxActions.Ok);
                        return;
                    }

                    bool same = true;
                    bool firstPass = true;
                    int[] delta = new int[3] { 0, 0, 0 };
                    int[] cumulativeDeltaSquared = new int[3] { 0, 0, 0 };
                    
                    graphic.PixelData.ForEachPixel(
                        delegate(int i, int x, int y, int pixelIndex)
                        {
                            int value = graphic.PixelData.GetPixel(pixelIndex);
                            value = ProcessModalityLut(graphic, value);

                            int compareValue = firstPixelData.GetPixel(pixelIndex);
                            compareValue = ProcessModalityLut(firstGraphic, compareValue);

                            if (firstPass == true)
                            {
                                delta = FindPixelDelta(graphic, value, compareValue);

                                for (int j=0; j < 3; j++)
                                {
                                    cumulativeDeltaSquared[j] += delta[j] * delta[j];
                                }
                                firstPass = false;
                            }
                            else
                            {
                                int[] diff = FindPixelDelta(graphic, value, compareValue);

                                if (diff[0] != delta[0] || diff[1] != delta[1] || diff[2] != delta[2])
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        cumulativeDeltaSquared[j] += diff[j] * diff[j];
                                    }
                                    same = false;
                                }
                            }
                        });

                    if (same != true)
                    {
                        double[] avgDelta = new double[3] { 0.0, 0.0, 0.0 };

                        for (int j = 0; j < 3; j++)
                        {
                            avgDelta[j] = System.Math.Sqrt(cumulativeDeltaSquared[j] / (graphic.Rows * graphic.Columns));
                        }

                        string message = "Fail - Avg Delta: Greyscale or R: " + avgDelta[0].ToString() + " G: " + avgDelta[1].ToString() + " B: " + avgDelta[2].ToString();
                        this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
                    }
                    else
                    {                        
                        PresentationImage presentation = clipboardItem.Item as PresentationImage;
                        Bitmap bitmap = presentation.DrawToBitmap(graphic.Rows, graphic.Columns);
                        
                        for (int x=0;x < graphic.Rows;x++)
                        {
                            for (int y = 0; y < graphic.Columns; y++)
                            {
                                Color firstPixel = firstBitmap.GetPixel(x, y);
                                Color comparisonPixel = bitmap.GetPixel(x, y);
                                if (firstPixel != comparisonPixel)
                                {
                                    same = false;
                                    break;
                                }                                
                            }
                            if (same == false)
                                break;
                        }
                        if (same != true)
                        {
                            this.Context.DesktopWindow.ShowMessageBox("Fail - bitmaps different", MessageBoxActions.Ok);
                        }
                        else
                        {
                            this.Context.DesktopWindow.ShowMessageBox("Same", MessageBoxActions.Ok);
                        }

                        bitmap.Dispose();                        
                    }
                }
            }
            else if (clipboardItem.Item is IDisplaySet)
            {
                this.Context.DesktopWindow.ShowMessageBox("Not supported on display sets yet.", MessageBoxActions.Ok);
                return;
            }
        }

        firstBitmap.Dispose();
    }

    private int ProcessModalityLut(ImageGraphic graphic, int value)
    {
        if (graphic is GrayscaleImageGraphic)
        {
            GrayscaleImageGraphic greyscaleImage = graphic as GrayscaleImageGraphic;
            if (greyscaleImage.ModalityLut != null)
                return greyscaleImage.ModalityLut[value];
        }

        return value;
    }

    private int[] FindPixelDelta(ImageGraphic graphic, int value, int compareValue)
    {
        if (graphic is GrayscaleImageGraphic)
        {
            return new int[3] { value - compareValue, 0, 0};
        }
        else
        {
            Color first = Color.FromArgb(value);
            Color compare = Color.FromArgb(compareValue);

            return new int[3] { first.R - compare.R, first.G - compare.G, first.B - compare.B };            
        }
    }    
}
