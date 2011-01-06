#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Interface for processors of WorkQueue items
    /// </summary>
    public interface IWorkQueueItemProcessor : IDisposable
    {
        #region Properties

        string Name { get;}
        #endregion
        #region Methods

        void Process(Model.WorkQueue item);
        
        #endregion
    }
}