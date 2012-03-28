#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public interface IWorkItemActivityCallback
    {
        [OperationContract(IsOneWay = true)]
        void WorkItemChanged(WorkItemData workItemData);
    }

    public abstract class WorkItemActivityCallback : IWorkItemActivityCallback
    {
        private class NilCallback : WorkItemActivityCallback
        {
            public override void WorkItemChanged(WorkItemData workItemData)
            {
            }
        }

        public static readonly IWorkItemActivityCallback Nil = new NilCallback();

        #region IWorkItemActivityCallback Members

        public abstract void WorkItemChanged(WorkItemData workItemData);

        #endregion
    }
}
