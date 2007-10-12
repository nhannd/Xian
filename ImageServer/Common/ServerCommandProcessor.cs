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

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// This class is used to execute and undo a series of <see cref="ServerCommand"/> instances.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The command pattern is used in the ImageServer whenever there are interactions with processing a DICOM file and
    /// in some cases when accessing the databsae. The pattern allows undoing of operations if a failure occurs.
    /// </para>
    /// <para>
    /// This class is utilized for 
    /// </para>
    /// </remarks>
    public class ServerCommandProcessor
    {
        #region Private Members
        private string _description;
        private Stack<ServerCommand> _stack = new Stack<ServerCommand>();
        #endregion

        #region Constructors
        public ServerCommandProcessor(string description)
        {
            _description = description;
        }
        #endregion

        #region Public Properties
        public string Description
        {
            get { return _description; }
        }
        #endregion

        #region Public Methods
        public void ExecuteCommand(ServerCommand command)
        {
            _stack.Push(command);
            command.Execute();
        }

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
                    Platform.Log(LogLevel.Error, e, "Unexpected exception rolling back command {0}", command.Name);
                }
            }
        }
        #endregion
    }
}
