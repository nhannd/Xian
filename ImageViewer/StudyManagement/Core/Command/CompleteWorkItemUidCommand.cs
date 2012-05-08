#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
    /// <summary>
    /// Complete a specific <see cref="WorkItemUid"/> record in the database.
    /// </summary>
    public class CompleteWorkItemUidCommand : DataAccessCommand
    {
        private WorkItemUid _uid;

        public CompleteWorkItemUidCommand(WorkItemUid uid) : base("Complete WorkItemUid")
        {
            _uid = uid;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            var broker = DataAccessContext.GetWorkItemUidBroker();
            _uid = broker.GetWorkItemUid(_uid.Oid);
            _uid.Complete = true;
        }

        protected override void OnUndo()
        {
            _uid.Complete = false;
        }
    }
}
