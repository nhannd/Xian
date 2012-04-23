#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    /// <summary>
    /// Plugin for WorkItem processors implementing <see cref="IWorkItemProcessorFactory"/>.
    /// </summary>
    [ExtensionPoint]
    public class WorkItemFactoryExtensionPoint : ExtensionPoint<IWorkItemProcessorFactory>
    {
    }
}
