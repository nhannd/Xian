#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Web.EntityHandlers
{
    [DropDownAction("quality", "global-toolbars/ToolbarImageQuality/ToolbarImageQuality", "QualityOptions")]
    [IconSet("quality", "ImageQualityToolSmall.png", "ImageQualityToolMedium.png", "ImageQualityToolLarge.png")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    internal class ImageQualityTool : ImageViewerTool
    {
        private SimpleActionModel _qualityDropDown;
        private Dictionary<ImageQualityOption, IClickAction> _qualityActions;

        private DelayedEventPublisher _delayRender;
        private ITile _tileWithCapture;
        private ITile _tileWithWheelCapture;
        private readonly Dictionary<ITile, ITile> _tilesToRefresh;
        private bool _refreshing = false;

        public ImageQualityTool()
        {
            _tilesToRefresh = new Dictionary<ITile, ITile>();
        }

        private ImageQualityOption ImageQualityOption
        {
            get { return ImageQualityManager.Instance.ImageQualityOption; }
            set { ImageQualityManager.Instance.ImageQualityOption = value; }
        }

        protected override void Dispose(bool disposing)
        {
            ImageViewer.EventBroker.MouseCaptureChanged -= TileMouseCaptureChanged;
            ImageViewer.EventBroker.MouseWheelCaptureChanged -= OnMouseWheelCaptureChanged;
            ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;
            
            ImageQualityManager.Instance.ImageQualityOptionChanged -= OnImageQualityOptionChanged;

            base.Dispose(disposing);
            _delayRender.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();
            _delayRender = new DelayedEventPublisher((s,e) => RefreshTiles());

            ImageQualityManager.Instance.ImageQualityOptionChanged += OnImageQualityOptionChanged;
            OnImageQualityOptionChanged(null, null);

            ImageViewer.EventBroker.MouseCaptureChanged += TileMouseCaptureChanged;
            ImageViewer.EventBroker.MouseWheelCaptureChanged += OnMouseWheelCaptureChanged;
            ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
        }

        private void OnImageQualityOptionChanged(object sender, EventArgs eventArgs)
        {
            ImageQualityOption? old = null;

            if (_qualityActions != null)
            {
                var oldAction = _qualityActions.First(k => k.Value.Checked);
                old = oldAction.Key;
                ((ClickAction)oldAction.Value).Checked = false;
                ((ClickAction)_qualityActions[ImageQualityOption]).Checked = true;
            }

            if (old.HasValue)
            {
                bool draw;
                switch (ImageQualityOption)
                {
                    //Only draw if the change will result in an image quality change.
                    case ImageQualityOption.Diagnostic:
                        draw = old != ImageQualityOption.DiagnosticDynamic;
                        break;
                    case ImageQualityOption.DiagnosticDynamic:
                        draw = old != ImageQualityOption.Diagnostic;
                        break;
                    case ImageQualityOption.LossyQuality:
                        draw = old != ImageQualityOption.LossyPerformance;
                        break;
                    case ImageQualityOption.LossyPerformance:
                        draw = old != ImageQualityOption.LossyQuality;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }

                if (draw)
                {
                    _delayRender.Cancel();
                    ImageViewer.PhysicalWorkspace.Draw();
                }
            }
        }

        private void OnMouseWheelCaptureChanged(object sender, MouseWheelCaptureChangedEventArgs args)
        {
            _tileWithWheelCapture = args.Gained ? args.Tile : null;
            if (_tileWithWheelCapture != null)
            {
                _tilesToRefresh[_tileWithWheelCapture] = _tileWithWheelCapture;
                ImageQualityManager.Instance.TileHasWheelCapture();
            }
            else
            {
                ImageQualityManager.Instance.TileLostWheelCapture();
            }
        }

        private void TileMouseCaptureChanged(object sender, MouseCaptureChangedEventArgs args)
        {
            _tileWithCapture = args.Gained ? args.Tile : null;
            if (_tileWithCapture != null)
            {
                _tilesToRefresh[_tileWithCapture] = _tileWithCapture;
                ImageQualityManager.Instance.TileHasCapture();
            }
            else
            {
                ImageQualityManager.Instance.TileLostCapture();
            }
        }

        private void OnImageDrawing(object sender, ImageDrawingEventArgs imageDrawingEventArgs)
        {
            if (_refreshing || (_tileWithCapture == null && _tileWithWheelCapture == null))
                return;

            _tilesToRefresh[imageDrawingEventArgs.PresentationImage.Tile] = imageDrawingEventArgs.PresentationImage.Tile;
            _delayRender.Publish(this, EventArgs.Empty);
        }

        private void RefreshTiles()
        {
            _refreshing = true;

            foreach (var tileToRefresh in _tilesToRefresh.Keys)
            {
                var theTileToRefresh = tileToRefresh;
                var tileInViewer = (from imageBox in ImageViewer.PhysicalWorkspace.ImageBoxes
                                    from tile in imageBox.Tiles
                                    where tile == theTileToRefresh
                                    select tile).FirstOrDefault();

                if (tileInViewer != null)
                    tileInViewer.Draw();
            }

            _tilesToRefresh.Clear();
            _refreshing = false;
        }

        public ActionModelNode QualityOptions
        {
            get
            {
                if (_qualityDropDown == null)
                {
                    _qualityDropDown = new SimpleActionModel(new ActionResourceResolver(this));

                    _qualityActions = new Dictionary<ImageQualityOption, IClickAction>();
                    _qualityActions[ImageQualityOption.Diagnostic] = 
                        _qualityDropDown.AddAction("diagnostic", SR.ImageQualityMenuDiagnostic, null, null,
                            () => ImageQualityOption = ImageQualityOption.Diagnostic);
                    _qualityActions[ImageQualityOption.DiagnosticDynamic] = 
                        _qualityDropDown.AddAction("diagnostic-dynamic", SR.ImageQualityMenuDynamic, null, null,
                            () => ImageQualityOption = ImageQualityOption.DiagnosticDynamic);
                    _qualityActions[ImageQualityOption.LossyQuality] = 
                        _qualityDropDown.AddAction("lossy-quality", SR.ImageQualityMenuLossyQuality, null, null,
                            () => ImageQualityOption = ImageQualityOption.LossyQuality);
                    _qualityActions[ImageQualityOption.LossyPerformance] = 
                        _qualityDropDown.AddAction("diagnostic", SR.ImageQualityMenuLossyPerformance, null, null,
                            () => ImageQualityOption = ImageQualityOption.LossyPerformance);
                    
                }

                ((ClickAction)_qualityActions[ImageQualityOption]).Checked = true;
                return _qualityDropDown;
            }
        }
    }
}
