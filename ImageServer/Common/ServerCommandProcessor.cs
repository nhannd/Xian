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
            command.Execute();
            _stack.Push(command);
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
