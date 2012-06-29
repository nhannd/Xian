#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Defines the interface of a command used by the <see cref="CommandProcessor"/>
    /// which includes several internal additional commands that must be rolled back.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an <see cref="ICommand"/> also is defined as an IAggregateServerCommand,
    /// it is assumed that the command will execute its sub-commands through the 
    /// <see cref="CommandProcessor.ExecuteSubCommand"/> method.  This method will 
    /// automatically add the commands as they are executed to the <see cref="AggregateCommands"/>
    /// property to ensure proper later rollback.
    /// </para>
    /// <para>
    /// If an error occurs that causes a Rollback, the <see cref="CommandProcessor"/> will 
    /// automatically also rollback the commands associated with the IAggregateServerCommand
    /// by looking at the <see cref="AggregateCommands"/> property.
    /// </para>
    /// </remarks>
    public interface IAggregateCommand : ICommand
    {
        Stack<ICommand> AggregateCommands { get; }
    }
}
