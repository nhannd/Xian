using System;
using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Automation
{
    public enum DisplaySetType
    {
        None = 0,
        Unknown,
        Series,
        SingleImage,
        AllImages,
        MixedMultiframeMultiframeImage,
        MixedMultiframeSingleImages,
        MREcho
    }

    public class DisplaySetInfo
    {
        public DisplaySetType DisplaySetType { get; set; }
        public string DisplaySetUid { get; set; }
        public string DisplaySetName { get; set; }
        public string StudyInstanceUid { get; set; }
        //public string AccessionNumber { get; set; }
        public string SeriesInstanceUid { get; set; }
        public string Modality { get; set; }
        public int? SeriesNumber { get; set; }
        public string SeriesDescription { get; set; }
        public int? InstanceNumber { get; set; }
        public int? FrameNumber { get; set; }
        public int? EchoNumber { get; set; }

    }

    public interface IDisplaySetLayout
    {
        DisplaySetInfo AssignDisplaySet(DisplaySetInfo displaySetInfo);
        DisplaySetInfo GetDisplaySetAt(RectangularGrid.Location imageBoxLocation);
    }
}