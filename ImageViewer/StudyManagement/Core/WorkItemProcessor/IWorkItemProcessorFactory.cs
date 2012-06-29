#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
    /// <summary>
    /// Factory for creating <see cref="IWorkItemProcessor"/> instances.
    /// </summary>
    public interface IWorkItemProcessorFactory
    {
        string GetWorkQueueType();

        IWorkItemProcessor GetItemProcessor();
    }
}
