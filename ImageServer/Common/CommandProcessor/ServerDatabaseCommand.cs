#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// <see cref="ServerCommand"/> derived class for implementing commands that interact with the database.
	/// </summary>
	public abstract class ServerDatabaseCommand : ServerCommand
	{
		#region Private Members
		private IUpdateContext _updateContext;
		#endregion

		#region Properties
		public IUpdateContext UpdateContext
		{
			get { return _updateContext; }
			set { _updateContext = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor for a ServerDatabaseCommand.
		/// </summary>
		/// <param name="description">A description of the command</param>
		/// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
		public ServerDatabaseCommand(string description, bool requiresRollback)
			: base(description, requiresRollback)
		{
		}
		#endregion

		/// <summary>
		/// Execute the ServerDatabaseCommand with the specified <see cref="IUpdateContext"/>.
		/// </summary>
		protected abstract void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext);

		/// <summary>
		/// Execute the <see cref="ServerCommand"/> 
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			if (UpdateContext != null)
			{
				OnExecute(theProcessor, UpdateContext);
				UpdateContext = null;
			}
			else
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					OnExecute(theProcessor, updateContext);
					updateContext.Commit();
				}
			}
		}

		/// <summary>
		/// Undo of database command, note that this is not called because the transaction is rolled back instead.
		/// </summary>
		protected override void OnUndo()
		{

		}
	}


    /// <summary>
    /// <see cref="ServerCommand"/> derived class for implementing commands interact with the database
    /// and are called in a specific context.
    /// </summary>
    public abstract class ServerDatabaseCommand<TContext> : ServerDatabaseCommand
        where TContext : class
    {
        #region Private Fields
        private readonly TContext _context;
        #endregion

        /// <summary>
        /// Constructor for a ServerDatabaseCommand.
        /// </summary>
        /// <param name="description">A description of the command</param>
        /// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
        /// <param name="context"></param>
        public ServerDatabaseCommand(string description, bool requiresRollback, TContext context)
            : base(description, requiresRollback)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the context for this command.
        /// </summary>
        protected TContext Context
        {
            get { return _context; }
        }
    }

    /// <summary>
    /// <see cref="ServerCommand"/> derived class for implementing commands interact with the database
    /// and are called in a specific context and required specific input.
    /// </summary>
    /// <remarks>
    /// <para>
    /// </para>
    /// </remarks>
    public abstract class ServerDatabaseCommand<TContext, TParameters> : ServerDatabaseCommand<TContext>
        where TContext : class
        where TParameters: class
    {
        #region Private Fields
        private readonly TParameters _parameters;
        #endregion

        /// <summary>
        /// Constructor for a ServerDatabaseCommand.
        /// </summary>
        /// <param name="description">A description of the command</param>
        /// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
        /// <param name="context">The context for the command</param>
        /// <param name="parameters">The paramaters for the command</param>
        public ServerDatabaseCommand(string description, bool requiresRollback, TContext context, TParameters parameters)
            : base(description, requiresRollback, context)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Gets the parameter object that was used to construct this command.
        /// </summary>
        protected TParameters Parameters
        {
            get { return _parameters; }
        }
    }
}