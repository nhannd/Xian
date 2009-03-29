using System;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// Defines the interface of a command used by the <see cref="CommandProcessor"/>
    /// </summary>
    public interface IServerCommand
    {
        /// <summary>
        /// Gets and sets the execution context for the command.
        /// </summary>
        ExecutionContext ExecutionContext { set; get; }

        /// <summary>
        /// Gets and sets a value describing what the command is doing.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets a value describing if the ServerCommand requires a rollback of the operation its included in if it fails during execution.
        /// </summary>
        bool RequiresRollback
        {
            get;
            set;
        }

        /// <summary>
        /// Execute the ServerCommand.
        /// </summary>
        void Execute();

        /// <summary>
        /// Undo the operation done by <see cref="Execute"/>.
        /// </summary>
        void Undo();
    }


}