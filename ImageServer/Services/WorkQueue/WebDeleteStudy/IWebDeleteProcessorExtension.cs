#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    public class WebDeleteProcessorContext
    {
        private readonly WebDeleteStudyItemProcessor _processor;
        private readonly DeletionLevel _level;
        private readonly string _reason;
        private readonly string _userId;
        private readonly string _userName;
        private readonly StudyStorageLocation _storageLocation;

        public WebDeleteProcessorContext(WebDeleteStudyItemProcessor processor, DeletionLevel level, StudyStorageLocation storageLocation, string reason, string userId, string userName)
        {
            _processor = processor;
            _storageLocation = storageLocation;
            _userName = userName;
            _userId = userId;
            _reason = reason;
            _level = level;
        }

        public WebDeleteStudyItemProcessor Processor
        {
            get { return _processor; }
        }

        public DeletionLevel Level
        {
            get { return _level; }
        }

        public string Reason
        {
            get { return _reason; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
        }

        public string UserId
        {
            get { return _userId; }
        }
    }
    public interface IWebDeleteProcessorExtension
    {
        void OnSeriesDeleting(WebDeleteProcessorContext context, Series series);
        void OnSeriesDeleted(WebDeleteProcessorContext context, Series series);
        void OnCompleted(WebDeleteProcessorContext context, IList<Series> series);
    }

    public class WebDeleteProcessorExtensionPoint:ExtensionPoint<IWebDeleteProcessorExtension>{}
}