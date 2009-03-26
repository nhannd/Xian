using System;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// Execution context for a command implementing <see cref="IServerCommand"/>
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Gets the temporary directory that can be used by the command.
        /// </summary>
        /// <remarks>
        /// All commmands within the same operation uses the same 
        /// temporary directory. It is created by the owner of the context
        /// which is also responsible for cleaning up this directory.
        /// </remarks>
        String TempDirectory { get; }

        /// <summary>
        /// Gets a unique temporary directory within the <see cref="TempDirectory"/>
        /// </summary>
        /// <returns></returns>
        String GetUniqueTempDir();
    }

    /// <summary>
    /// Defines the interface of a command used by the <see cref="CommandProcessor"/>
    /// </summary>
    public interface IServerCommand
    {
        /// <summary>
        /// Gets and sets the execution context for the command.
        /// </summary>
        IExecutionContext ExecutionContext { set; get; }

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