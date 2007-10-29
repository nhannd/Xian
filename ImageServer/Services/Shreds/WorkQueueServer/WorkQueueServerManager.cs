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

using ClearCanvas.ImageServer.Services.WorkQueue;

namespace ClearCanvas.ImageServer.Services.Shreds.WorkQueueServer
{
    /// <summary>
    /// Shreds namespace manager of processing threads for the WorkQueue.
    /// </summary>
    public class WorkQueueServerManager
    {
        #region Private Members
        private static WorkQueueServerManager _instance;
        private WorkQueueProcessor _theProcessor;
        #endregion

        #region Constructors
        /// <summary>
        /// **** For internal use only***
        /// </summary>
        private WorkQueueServerManager()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// Singleton instance of the class.
        /// </summary>
        public static WorkQueueServerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WorkQueueServerManager();

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        #endregion

        #region Public Methods

        public void Start()
        {
            if (_theProcessor == null)
            {
                _theProcessor = new WorkQueueProcessor("WorkQueue Processor", ImageServerServicesShredSettings.Default.WorkQueueThreadCount); // 5 threads for processor
                _theProcessor.Start();
            }
        }

        public void Stop()
        {
            if (_theProcessor != null)
            {
                _theProcessor.Stop();
                _theProcessor = null;
            }
        }
        #endregion
    }
}