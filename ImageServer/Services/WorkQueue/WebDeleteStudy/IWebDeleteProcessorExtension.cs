#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
        private readonly string _userName;
        private readonly StudyStorageLocation _storageLocation;

        public WebDeleteProcessorContext(WebDeleteStudyItemProcessor processor, DeletionLevel level, StudyStorageLocation storageLocation, string reason, string userName)
        {
            _processor = processor;
            _storageLocation = storageLocation;
            _userName = userName;
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
    }
    public interface IWebDeleteProcessorExtension
    {
        void OnSeriesDeleting(WebDeleteProcessorContext context, Series series);
        void OnSeriesDeleted(WebDeleteProcessorContext context, Series series);
        void OnCompleted(WebDeleteProcessorContext context, IList<Series> series);
    }

    public class WebDeleteProcessorExtensionPoint:ExtensionPoint<IWebDeleteProcessorExtension>{}
}