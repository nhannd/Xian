#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Defines the interface of a command used by the <see cref="ServerCommandProcessor"/>
	/// which includes several internal additional commands that must be rolled back.
	/// </summary>
	/// <remarks>
	/// <para>
	/// When an <see cref="IServerCommand"/> also is defined as an IAggregateServerCommand,
	/// it is assumed that the command will execute its sub-commands through the 
	/// <see cref="ServerCommandProcessor.ExecuteSubCommand"/> method.  This method will 
	/// automatically add the commands as they are executed to the <see cref="AggregateCommands"/>
	/// property to ensure proper later rollback.
	/// </para>
	/// <para>
	/// If an error occurs that causes a Rollback, the <see cref="ServerCommandProcessor"/> will 
	/// automatically also rollback the commands associated with the IAggregateServerCommand
	/// by looking at the <see cref="AggregateCommands"/> property.
	/// </para>
	/// </remarks>
	public interface IAggregateServerCommand : IServerCommand
	{
		Stack<IServerCommand> AggregateCommands { get; }
	}
}
