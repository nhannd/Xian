using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Web.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.EntityHandlers
{
    public interface IQFactorStrategy
    {
        int GetOptimalQFactor(int imageWidth, int imageHeight, IImageSopProvider sop);
    }

    public class QFactorExtensionPoint : ExtensionPoint<IQFactorStrategy> { }

    public enum ImageQualityOption
    {
        Diagnostic,
        DiagnosticDynamic,
        LossyQuality,
        LossyPerformance
    }

    public class ImageQualityManager
    {
        [ThreadStatic]
        private static ImageQualityManager _instance;

        private string _mimeType;
        private int _quality;
        private readonly ImageQualitySettings _imageQualitySettings = new ImageQualitySettings();
        
        private bool _tileHasCapture;
        private bool _tileHasWheelCapture;

        private ImageQualityManager()
        {
            SetQuality();
        }

        public static ImageQualityManager Instance
        {
            get { return _instance ?? (_instance = new ImageQualityManager()); }
        }

        public ImageQualityOption ImageQualityOption
        {
            get { return _imageQualitySettings.ImageQualityOption; }
            set
            {
                if (_imageQualitySettings.ImageQualityOption == value)
                    return;

                _imageQualitySettings.ImageQualityOption = value;
                _imageQualitySettings.Save();

                SetQuality();
                EventsHelper.Fire(ImageQualityOptionChanged, this, EventArgs.Empty);
            }
        }

        public event EventHandler ImageQualityOptionChanged;

        internal void TileHasCapture()
        {
            _tileHasCapture = true;
            SetQuality();
        }

        internal void TileLostCapture()
        {
            _tileHasCapture = false;
            SetQuality();
        }

        internal void TileHasWheelCapture()
        {
            _tileHasWheelCapture = true;
            SetQuality();
        }

        internal void TileLostWheelCapture()
        {
            _tileHasWheelCapture = false;
            SetQuality();
        }

        public bool IsImageQualityLossy
        {
            get { return _mimeType == Image.MimeTypes.Jpeg; }
        }

        public void GetImageQualityParameters(out string mimeType, out int jpegQuality)
        {
            mimeType = _mimeType;
            jpegQuality = _quality;
        }

        private void SetQuality()
        {
            if (_tileHasCapture || _tileHasWheelCapture)
                CaptureQuality();
            else
                NoCaptureQuality();
        }

        private void CaptureQuality()
        {
            switch (ImageQualityOption)
            {
                case ImageQualityOption.Diagnostic:
                    _mimeType = Image.MimeTypes.Png;
                    break;
                case ImageQualityOption.DiagnosticDynamic:
                    _mimeType = Image.MimeTypes.Jpeg;
                    _quality = _imageQualitySettings.JpegQualityLow;
                    break;
                case ImageQualityOption.LossyQuality:
                    _mimeType = Image.MimeTypes.Jpeg;
                    _quality = _imageQualitySettings.JpegQualityHigh;
                    break;
                case ImageQualityOption.LossyPerformance:
                    _mimeType = Image.MimeTypes.Jpeg;
                    _quality = _imageQualitySettings.JpegQualityLow;
                    break;
            }
        }
    
        private void NoCaptureQuality()
        {
            switch (ImageQualityOption)
            {
                case ImageQualityOption.Diagnostic:
                    _mimeType = Image.MimeTypes.Png;
                    break;
                case ImageQualityOption.DiagnosticDynamic:
                    _mimeType = Image.MimeTypes.Png;
                    break;
                case ImageQualityOption.LossyQuality:
                    _mimeType = Image.MimeTypes.Jpeg;
                    _quality = _imageQualitySettings.JpegQualityHigh;
                    break;
                case ImageQualityOption.LossyPerformance:
                    _mimeType = Image.MimeTypes.Jpeg;
                    _quality = _imageQualitySettings.JpegQualityHigh;
                    break;
            }
        }
    }
}
