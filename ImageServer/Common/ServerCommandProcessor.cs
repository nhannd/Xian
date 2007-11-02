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
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// This class is used to execute and undo a series of <see cref="ServerCommand"/> instances.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The command pattern is used in the ImageServer whenever there are interactions while processing
    /// DICOM files.  The pattern allows undoing of the operations as files are being modified and
    /// data inserted into the database.  
    /// </para>
    /// </remarks>
    public class ServerCommandProcessor
    {
        #region Private Members
        private readonly string _description;
        private readonly Stack<ServerCommand> _stack = new Stack<ServerCommand>();
        private readonly Queue<ServerCommand> _queue = new Queue<ServerCommand>();
        private string _failureReason;
        #endregion

        #region Constructors
        public ServerCommandProcessor(string description)
        {
            _description = description;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Description for the processor.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Reason for a failure, if it occurs.
        /// </summary>
        public string FailureReason
        {
            get { return _failureReason; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a command to the processor.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void AddCommand(ServerCommand command)
        {
            _queue.Enqueue(command);
        }
        
        /// <summary>
        /// Execute the commands passed to the processor.
        /// </summary>
        /// <returns>false on failure, true on success</returns>
        public bool Execute()
        {
            while (_queue.Count > 0)
            {
                ServerCommand command = _queue.Dequeue();

                _stack.Push(command);
                try
                {
                    command.Execute();
                } 
                catch (Exception e)
                {
                    if (command.RequiresRollback)
                    {
                        _failureReason = e.Message;
                        Platform.Log(LogLevel.Error, e, "Unexpeceted error when executing command: {0}", command.Description);
                        Rollback();
                        return false;
                    }
                    else
                    {
                        Platform.Log(LogLevel.Warn, e,
                                     "Unexpected exception on command {0} that doesn't require rollback", command.Description);
                        _stack.Pop(); // Pop it off the stack, since it failed.
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Rollback the commands that have been executed already.
        /// </summary>
        public void Rollback()
        {
            while (_stack.Count > 0)
            {
                ServerCommand command = _stack.Pop();
                try
                {
                    command.Undo();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception rolling back command {0}", command.Description);
                }
            }
        }
        #endregion
    }
}
