#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{   
    /// <summary>
    /// Interface for processors of WorkItem items
    /// </summary>
    public interface IWorkItemProcessor : IDisposable
    {
        #region Properties

        string Name { get;}

        #endregion

        #region Methods

        bool Initialize(WorkItemStatusProxy proxy);

        bool CanStart(WorkItemStatusProxy proxy, out string reason);

        void Process(WorkItemStatusProxy proxy);

        void Cancel();

        void Stop();

        void Delete(WorkItemStatusProxy proxy);

        #endregion
    }
}
