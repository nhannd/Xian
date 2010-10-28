#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy
{
    public class DeleteStudyContext
    {
        private ServerPartition _serverPartition;
        private StudyStorageLocation _storage;
        private Study _study;
        private Model.WorkQueue _item;
        private ServerFilesystemInfo _filesystem;
        private string _userId;
        private string _userName;

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _storage; }
            set { _storage = value; }
        }

        public ServerFilesystemInfo Filesystem
        {
            get { return _filesystem; }
            set { _filesystem = value; }
        }

        public Model.WorkQueue WorkQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
    }

    public interface IDeleteStudyProcessorExtension
    {
        bool Enabled { get; }
        void Initialize(DeleteStudyContext context);
        void OnStudyDeleting();
        void OnStudyDeleted();
    }

    public class DeleteStudyProcessorExtensionPoint:ExtensionPoint<IDeleteStudyProcessorExtension>
    {}
}
