#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Web.EntityHandlers
{
    internal class DefaultQFactorStrategy : IQFactorStrategy
    {
        private readonly Dictionary<int, int> _8bitImageSizeToQMap = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _12bitImageSizeToQMap = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _16bitImageSizeToQMap = new Dictionary<int, int>();
	    
        public DefaultQFactorStrategy()
        {
            InitOptimalQFactors();
        }

        private void InitOptimalQFactors()
        {
            // TODO: It's probably better to adjust this dynamically
            // based on: connection speed, bitdepth and range, 
            // zoom level (can be more aggressive at 5x than at 1x?), 
            // action: zoom, stack, pan
            _8bitImageSizeToQMap.Add(300 * 300, 70);
            _8bitImageSizeToQMap.Add(400 * 400, 65);
            _8bitImageSizeToQMap.Add(600 * 600, 60);
            _8bitImageSizeToQMap.Add(800 * 800, 55);
            _8bitImageSizeToQMap.Add(900 * 900, 50);

            _12bitImageSizeToQMap.Add(300 * 300, 70);
            _12bitImageSizeToQMap.Add(400 * 400, 60);
            _12bitImageSizeToQMap.Add(600 * 600, 50);
            _12bitImageSizeToQMap.Add(800 * 800, 40);
            _12bitImageSizeToQMap.Add(900 * 900, 30);

            _16bitImageSizeToQMap.Add(300 * 300, 50);
            _16bitImageSizeToQMap.Add(400 * 400, 50);
            _16bitImageSizeToQMap.Add(600 * 600, 45);
            _16bitImageSizeToQMap.Add(800 * 800, 40);
            _16bitImageSizeToQMap.Add(900 * 900, 30);
        }

        public int GetOptimalQFactor(int imageWidth, int imageHeight, IImageSopProvider sop)
        {
            // TODO: It's probably better to adjust this dynamically
            // based on: connection speed, bitdepth and pixel value range, 
            // zoom level (can be more aggressive at 5x than at 1x or otherwise), 
            // action: zoom, stack, pan

            // We don't need to change the quality if the previous image
            // already < 32K
            //if (_prevImageSize >0 && _prevImageSize < 1024*30)
            //{
            //    return _quality;
            //}

            //float zoomLevel = 1.0f;
            //if (sop is ISpatialTransformProvider)
            //    zoomLevel = (sop as ISpatialTransformProvider).SpatialTransform.Scale;

            int lowestQuality = 50;
            int highBit = sop.Frame.HighBit;

            if (highBit <= 8)
            {
                foreach (int k in _8bitImageSizeToQMap.Keys)
                {
                    if (k > imageWidth * imageHeight)
                        return _8bitImageSizeToQMap[k];

                    lowestQuality = _8bitImageSizeToQMap[k];
                }

                return lowestQuality;
            }
            if (highBit <= 12)
            {
                foreach (int k in _12bitImageSizeToQMap.Keys)
                {
                    if (k > imageWidth * imageHeight)
                        return _12bitImageSizeToQMap[k];
                    lowestQuality = _12bitImageSizeToQMap[k];
                }

                return lowestQuality;
            }
            foreach (int k in _16bitImageSizeToQMap.Keys)
            {
                if (k > imageWidth * imageHeight)
                    return _16bitImageSizeToQMap[k];

                lowestQuality = _16bitImageSizeToQMap[k];
            }

            return lowestQuality;
        }
    }
}