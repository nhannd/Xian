#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for providing a custom factory for creating instances of <see cref="DesktopWindow"/>.
    /// </summary>
    /// <remarks>
    /// Provide an extension to this point if you have subclassed <see cref="DesktopWindow"/> and you want to be 
    /// able to provide instances of your subclass to the framework when it requests creation of a new desktop window.
    /// </remarks>
    [ExtensionPoint]
	public sealed class DesktopWindowFactoryExtensionPoint : ExtensionPoint<IDesktopWindowFactory>
    {
    }

    /// <summary>
    /// Defines the interface to extensions of <see cref="DesktopWindowFactoryExtensionPoint"/>.
    /// </summary>
    public interface IDesktopWindowFactory
    {
        /// <summary>
        /// Creates a new desktop window for the specified arguments.
        /// </summary>
        /// <param name="args">Arguments that control the creation of the desktop window.</param>
        /// <param name="application">The application with which the window is associated.</param>
        /// <returns>A new desktop window instance.</returns>
        DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Application application);
    }

    /// <summary>
    /// Default desktop window factory used when no extensions are provided.
    /// </summary>
    internal class DefaultDesktopWindowFactory : IDesktopWindowFactory
    {
        #region IDesktopWindowFactory Members

        public DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Application application)
        {
            return new DesktopWindow(args, application);
        }

        #endregion
    }

    /// <summary>
    /// Defines an extension point for providing a custom factory for creating instances of <see cref="Workspace"/>.
    /// </summary>
    /// <remarks>
    /// Provide an extension to this point if you have subclassed <see cref="Workspace"/> and you want to be 
    /// able to provide instances of your subclass to the framework when it requests creation of a new workspace.
    /// </remarks>
    [ExtensionPoint]
	public sealed class WorkspaceFactoryExtensionPoint : ExtensionPoint<IWorkspaceFactory>
    {
    }

    /// <summary>
    /// Defines the interface to extensions of <see cref="WorkspaceFactoryExtensionPoint"/>.
    /// </summary>
    public interface IWorkspaceFactory
    {
        /// <summary>
        /// Creates a new workspace for the specified arguments.
        /// </summary>
        /// <param name="args">Arguments that control the creation of the workspace.</param>
        /// <param name="window">The desktop window with which the workspace is associated.</param>
        /// <returns>A new workspace instance.</returns>
        Workspace CreateWorkspace(WorkspaceCreationArgs args, DesktopWindow window);
    }

    /// <summary>
    /// Default workspace factory used when no extensions are provided.
    /// </summary>
    internal class DefaultWorkspaceFactory : IWorkspaceFactory
    {
        #region IWorkspaceFactory Members

        public Workspace CreateWorkspace(WorkspaceCreationArgs args, DesktopWindow window)
        {
            return new Workspace(args, window);
        }

        #endregion
    }

    /// <summary>
    /// Defines an extension point for providing a custom factory for creating instances of <see cref="Shelf"/>.
    /// </summary>
    /// <remarks>
    /// Provide an extension to this point if you have subclassed <see cref="Shelf"/> and you want to be 
    /// able to provide instances of your subclass to the framework when it requests creation of a new shelf.
    /// </remarks>
    [ExtensionPoint]
	public sealed class ShelfFactoryExtensionPoint : ExtensionPoint<IShelfFactory>
    {
    }

    /// <summary>
    /// Defines the interface to extensions of <see cref="ShelfFactoryExtensionPoint"/>.
    /// </summary>
    public interface IShelfFactory
    {
        /// <summary>
        /// Creates a new shelf for the specified arguments.
        /// </summary>
        /// <param name="args">Arguments that control the creation of the shelf.</param>
        /// <param name="window">The desktop window with which the shelf is associated.</param>
        /// <returns>A new shelf instance.</returns>
        Shelf CreateShelf(ShelfCreationArgs args, DesktopWindow window);
    }

    /// <summary>
    /// Default shelf factory used when no extensions are provided.
    /// </summary>
    internal class DefaultShelfFactory : IShelfFactory
    {
        #region IShelfFactory Members

        public Shelf CreateShelf(ShelfCreationArgs args, DesktopWindow window)
        {
            return new Shelf(args, window);
        }

        #endregion
    }

    /// <summary>
    /// Defines an extension point for providing a custom factory for creating instances of <see cref="DialogBox"/>.
    /// </summary>
    /// <remarks>
    /// Provide an extension to this point if you have subclassed <see cref="DialogBox"/> and you want to be 
    /// able to provide instances of your subclass to the framework when it requests creation of a new dialog box.
    /// </remarks>
    [ExtensionPoint]
	public sealed class DialogBoxFactoryExtensionPoint : ExtensionPoint<IDialogBoxFactory>
    {
    }

    /// <summary>
    /// Defines the interface to extensions of <see cref="DialogBoxFactoryExtensionPoint"/>.
    /// </summary>
    public interface IDialogBoxFactory
    {
        /// <summary>
        /// Creates a new dialog box for the specified arguments.
        /// </summary>
        /// <param name="args">Arguments that control the creation of the dialog.</param>
        /// <param name="window">The desktop window with which the dialog is associated.</param>
        /// <returns>A new dialog instance.</returns>
        DialogBox CreateDialogBox(DialogBoxCreationArgs args, DesktopWindow window);
    }

    /// <summary>
    /// Default dialog factory used when no extensions are provided.
    /// </summary>
    internal class DefaultDialogBoxFactory : IDialogBoxFactory
    {
        #region IDialogBoxFactory Members

        public DialogBox CreateDialogBox(DialogBoxCreationArgs args, DesktopWindow window)
        {
            return new DialogBox(args, window);
        }

        #endregion
    }
}
