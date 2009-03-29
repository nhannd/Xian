#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
	/// This class is used to execute and undo a series of <see cref="IServerCommand"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The command pattern is used in the ImageServer whenever there are interactions while processing
	/// DICOM files.  The pattern allows undoing of the operations as files are being modified and
	/// data inserted into the database.  
	/// </para>
	/// <para>
	/// If <see cref="ServerDatabaseCommand"/> objects are included among the <see cref="ServerCommand"/>
	/// instances, an <see cref="IUpdateContext"/> will be opened for the database, and will be used
	/// to execute each of the <see cref="ServerDatabaseCommand"/> instances.  If a failure occurs
	/// when executing the commands, the <see cref="IUpdateContext"/> will be rolled back.  If no
	/// failures occur, the context will be committed.
	/// </para>
	/// <para>
	/// When implementing <see cref="ServerDatabaseCommand"/> instances, it is recommended to group these
	/// together at the end of the list of commands, so that they are executed in sequence, and there
	/// are not any long running non-database related commands executing.  Having long running 
	/// non-database related commands being executed between database commands will cause a delay
	/// in committing transactions, and could cause database deadlocks and problems.
	/// </para>
	/// </remarks>
	public class ServerCommandProcessor : IDisposable
	{
		#region Private Members
		private string _description;
        private readonly Stack<IServerCommand> _stack = new Stack<IServerCommand>();
        private readonly Queue<IServerCommand> _queue = new Queue<IServerCommand>();
        private readonly List<IServerCommand> _list = new List<IServerCommand>(); 
        private string _failureReason;
		private IUpdateContext _updateContext = null;
		private Exception _exception;
        private ExecutionContext _executionContext;
	    private bool _ownsContext;

	    #endregion

		#region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ServerCommandProcessor"/> using
        /// the current <see cref="ExecutionContext"/> for the thread.
        /// </summary>
        /// <param name="description"></param>
	    public ServerCommandProcessor(string description)
            : this(description, CommandProcessor.ExecutionContext.Current)
		{
            
		}

        /// <summary>
        /// Creates an instance of <see cref="ServerCommandProcessor"/> using
        /// the specified <see cref="ExecutionContext"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="context"></param>
        public ServerCommandProcessor(string description, ExecutionContext context)
	    {
            _description = description;
	        ExecutionContext = context;
	    }

	    #endregion

		#region Public Properties
		/// <summary>
		/// Description for the processor.
		/// </summary>
		public string Description
		{
			get { return _description; }
            set { _description = value; }
		}

		/// <summary>
		/// Reason for a failure, if it occurs.
		/// </summary>
		public string FailureReason
		{
			get { return _failureReason; }
		}

		public Exception FailureException
		{
			get { return _exception; }
			protected set { _exception = value; }
		}

		/// <summary>
		/// Number of commands stored in the processor queue.
		/// </summary>
		public int CommandCount
		{
			get { return _queue.Count; }
		}

		public IUpdateContext UpdateContext
		{
			get { return _updateContext; }
			set { _updateContext = value; }
		}

        public ExecutionContext ExecutionContext
	    {
	        get { return _executionContext; }
	        set { _executionContext = value; }
	    }

	    #endregion

		#region Public Methods
		/// <summary>
		/// Add a command to the processor.
		/// </summary>
		/// <param name="command">The command to add.</param>
		public void AddCommand(IServerCommand command)
		{
		    _queue.Enqueue(command);
		    _list.Add(command);
		}
        
		/// <summary>
		/// Execute the commands passed to the processor.
		/// </summary>
		/// <returns>false on failure, true on success</returns>
		public bool Execute()
		{
            if (ExecutionContext==null)
            {
                // No execution context assigned, create one
                ExecutionContext = new ExecutionContext();
                _ownsContext = true;
            }
			
			while (_queue.Count > 0)
			{
                IServerCommand command = _queue.Dequeue();
                command.ExecutionContext = ExecutionContext;
                ServerDatabaseCommand dbCommand = command as ServerDatabaseCommand;

				_stack.Push(command);
				try
				{
					if (dbCommand != null)
					{
						if (UpdateContext == null)
							UpdateContext =
								PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
						dbCommand.UpdateContext = UpdateContext;
					}

					command.Execute();
				}
				catch (Exception e)
				{
					if (command.RequiresRollback || dbCommand != null)
					{
						_failureReason = String.Format("{0}: {1}", e.GetType().Name, e.Message);
						Platform.Log(LogLevel.Error, e, "Unexpected error when executing command: {0}", command.Description);
						Rollback();
						FailureException = e;
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

			if (UpdateContext != null)
			{
				UpdateContext.Commit();
				UpdateContext.Dispose();
				UpdateContext = null;
			}

			return true;
		}


	    /// <summary>
		/// Rollback the commands that have been executed already.
		/// </summary>
		public void Rollback()
		{
			if (UpdateContext != null)
			{
				UpdateContext.Dispose(); // Rollback the db
				UpdateContext = null;
			}

			while (_stack.Count > 0)
			{
			    IServerCommand command = _stack.Pop();
				
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

		#region IDisposable Members

		public void Dispose()
		{
            // note: Each dispose operation is INTENTIONALLY wrapped within a try/catch block 
            // to ensure everything is cleaned up.
            try
            {
                if (UpdateContext != null)
                {
                    Rollback();
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex);
            }

        
            foreach (ServerCommand command in _list)
            {
                if (command is IDisposable)
                {
                    try
                    {
                        (command as IDisposable).Dispose();
                    }
                    catch(Exception ex)
		            {
        			    Platform.Log(LogLevel.Error, ex);
		            }
                }
            }

            if (_ownsContext && ExecutionContext!=null)
            {
                ExecutionContext.Dispose();
            }
		}

		#endregion
	}
}