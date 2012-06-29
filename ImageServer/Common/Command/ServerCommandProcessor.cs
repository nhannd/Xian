#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageServer.Common.Command
{
    /// <summary>
	/// This class is used to execute and undo a series of <see cref="ICommand"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The command pattern is used in the ImageServer whenever there are interactions while processing
	/// DICOM files.  The pattern allows undoing of the operations as files are being modified and
	/// data inserted into the database.  
	/// </para>
	/// <para>
	/// If <see cref="ServerDatabaseCommand"/> objects are included among the <see cref="CommandBase"/>
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
	/// <para>
	/// The ServerCommandProcessor also supports executing commands that implement the 
	/// <see cref="IAggregateCommand"/> interface.  It is assumed that these commands 
	/// are an aggregate of several sub-commands.  When a <see cref="CommandProcessor.Rollback"/> occurs
	/// for an <see cref="IAggregateCommand"/> command, the base command is first
	/// rolled back, then the sub-commands for the <see cref="IAggregateCommand"/>
	/// are rolled back.  Note that classes implementing <see cref="IAggregateCommand"/>
	/// should use the <see cref="CommandProcessor.ExecuteSubCommand"/> method to execute sub-commands.
	/// </para>
	/// </remarks>
    public class ServerCommandProcessor : CommandProcessor
	{
		#region Private Members

        private readonly bool _ownsExecContext;
        private readonly string _contextId;

        #endregion

		#region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ServerCommandProcessor"/> using
        /// the current <see cref="ServerExecutionContext"/> for the thread.
        /// </summary>
        /// <param name="contextId"></param>
        /// <param name="description"></param>
        public ServerCommandProcessor(string contextId, string description) : base(description, new ServerExecutionContext(contextId))
        {
            Platform.CheckForEmptyString(contextId, "contextId");
            _contextId = contextId;
            _ownsExecContext = ServerExecutionContext.Current == null;
        }

        /// <summary>
        /// Creates an instance of <see cref="ServerCommandProcessor"/> using
        /// the current <see cref="ServerExecutionContext"/> for the thread.
        /// </summary>
        /// <param name="description"></param>
        public ServerCommandProcessor(string description)
            :this(Guid.NewGuid().ToString(), description)
        { }

	    #endregion


		#region IDisposable Members

		public override void Dispose()
		{
		    base.Dispose();

            if (_ownsExecContext && ProcessorContext!=null)
            {
                ProcessorContext.Dispose();
            }
		}

		#endregion

	}
}