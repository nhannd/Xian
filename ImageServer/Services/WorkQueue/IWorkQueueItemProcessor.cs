#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
	/// <summary>
    /// Defines the event handler for <see cref="IWorkQueueItemProcessor.ProcessingBegin"/>.
	/// </summary>
	/// <param name="item"></param>
    public delegate void ProcessingBeginEventListener(Model.WorkQueue item);
    /// <summary>
    /// Defines the event handler for <see cref="IWorkQueueItemProcessor.ProcessingCompleted"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="status"></param>
    public delegate void ProcessingCompletedEventListener(Model.WorkQueue item, ProcessResultEnum status);
    


    /// <summary>
    /// Interface for processors of WorkQueue items
    /// </summary>
    public interface IWorkQueueItemProcessor : IDisposable
    {
        #region Properties
        // A string used to identify the processor
        string ProcessorID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the result of the processing.
        /// </summary>
        ProcessResultEnum ProcessResult
        {
            get;
        }

        /// <summary>
        /// Gets or sets the WorkQueue item being processed.
        /// </summary>
        Model.WorkQueue WorkQueueItem
        {
            get; set;
        }

        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs before the <see cref="WorkQueueItem"/> is processed.
        /// </summary>
        event ProcessingBeginEventListener ProcessingBegin;

        /// <summary>
        /// Occurs after the <see cref="WorkQueueItem"/> has been processed.
        /// </summary>
        /// <remarks>
        /// The status of the processing can be retrieved via <see cref="ProcessResult"/>.
        /// </remarks>
        event ProcessingCompletedEventListener ProcessingCompleted;
        #endregion

        #region Methods

        void Process();
        
        #endregion

    }
}