using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// The AggregateCommand is a placeholder <see cref="ICommand"/> that will execute a series of subcommands when its executed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The AggregateCommand can be used as a placeholder in the <see cref="CommandProcessor"/> where a number of commands can be queued up
    /// for execution in the place of the AggregateCommand.  This is useful if a number of commands need to be executed before a database 
    /// related command is executed.
    /// </para>
    /// </remarks>
    public class AggregateCommand : CommandBase,IAggregateCommand
    {
        #region Private Members
        private readonly Stack<ICommand> _aggregateStack = new Stack<ICommand>();
        private readonly Queue<ICommand> _subCommands = new Queue<ICommand>();
        #endregion
        
 
        /// <summary>
        /// Constructor.
        /// </summary>
        public AggregateCommand()
            : base("Aggregate Command", true)
        {}

        public void AddSubCommand(ICommand command)
        {
            _subCommands.Enqueue(command);
        }

        public Stack<ICommand> AggregateCommands
        {
            get { return _aggregateStack; }
        }
        
        protected override void OnExecute(CommandProcessor theProcessor)
        {
            while (_subCommands.Count > 0)
            {
                if (!theProcessor.ExecuteSubCommand(this, _subCommands.Dequeue()))
                    throw new ApplicationException(theProcessor.FailureReason);
            }
        }

        protected override void OnUndo()
        {
        }
    }
}
