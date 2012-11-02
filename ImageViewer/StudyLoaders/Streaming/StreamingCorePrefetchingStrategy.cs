#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
    internal class StreamingCorePrefetchingStrategy : ICorePrefetchingStrategy
    {
        #region ICorePrefetchingStrategy Members

        public bool CanRetrieveFrame(Frame frame)
        {
            return frame.ParentImageSop.DataSource is StreamingSopDataSource && !frame.Info.Cached;
        }

        public void RetrieveFrame(Frame frame)
        {
            if (frame.Info.Cached)
                return;

            try
            {
                var dataSource = (IStreamingSopDataSource) frame.ParentImageSop.DataSource;
                dataSource.GetFrameData(frame.Info).RetrievePixelData();
            }
            catch (OutOfMemoryException)
            {
                Platform.Log(LogLevel.Error, "Out of memory trying to retrieve pixel data.");
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Error retrieving frame pixel data.");
            }
        }

        public bool CanDecompressFrame(Frame frame)
        {
            throw new NotImplementedException();
        }

        public void DecompressFrame(Frame frame)
        {
            throw new NotImplementedException();
        }

        #endregion

    
  
    }
}