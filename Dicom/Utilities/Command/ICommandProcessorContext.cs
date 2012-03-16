#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Interfacce for the context that the <see cref="CommandProcessor"/> is run within.
    /// </summary>
    /// <remarks>
    /// Note that a <see cref="CommandProcessor"/> should only access a single instance of 
    /// ICommandProcessorContext.  Onces the <see cref="CommandProcessor"/> is disposed, the
    /// <see cref="ICommandProcessorContext"/> will also be disposed.
    /// </remarks>
    public interface ICommandProcessorContext : IDisposable
    {
        /// <summary>
        /// Called by the <see cref="CommandProcessor"/> before an <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="command">The command being executed.</param>
        void PreExecute(ICommand command);

        /// <summary>
        /// Called when the <see cref="CommandProcessor"/> commits its <see cref="ICommand"/>s.
        /// </summary>
        void Commit();

        /// <summary>
        /// Called when the <see cref="CommandProcessor"/> rolls back its <see cref="ICommand"/>
        /// </summary>
        void Rollback();

        /// <summary>
        /// Temporary directory path that can be used be <see cref="ICommand"/> instances to store
        /// temporary files.
        /// </summary>
        String TempDirectory { get; }
    }
}
