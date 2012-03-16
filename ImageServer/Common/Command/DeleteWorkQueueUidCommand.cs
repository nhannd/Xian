#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common.Command
{
    /// <summary>
    /// Command to delete a work queue uid.
    /// </summary>
    public class DeleteWorkQueueUidCommand : ServerDatabaseCommand
    {
        #region Private Fields
        private readonly WorkQueueUid _uid;
        #endregion

        public DeleteWorkQueueUidCommand(WorkQueueUid uid)
            : base("Delete WorkQueue Uid Entry")
        {
            Platform.CheckForNullReference(uid, "uid");
            _uid = uid;
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
            delete.Delete(_uid.GetKey());
        }
    }
}
