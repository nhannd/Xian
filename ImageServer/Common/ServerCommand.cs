using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Abstract class representing a command within the Command pattern.
    /// </summary>
    /// <remarks>
    /// <para>The Command pattern is used throughout the ImageServer when doing
    /// file and database operations to allow undoing of the operations.  This
    /// abstract class is used as the interface for the command.</para>
    /// </remarks>
    public abstract class ServerCommand
    {
        #region Private Members
        private string _name;
        #endregion

        #region Constructor
        public ServerCommand(string name)
        {
            _name = name;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion

        #region Abstract Methods
        public abstract void Execute();
        public abstract void Undo();
        #endregion
    }
}
