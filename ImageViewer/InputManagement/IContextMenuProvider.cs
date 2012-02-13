#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A provider of an <see cref="ActionModelNode"/> that is returned based on the current state of the mouse.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The framework will look for this interface on graphic objects (<see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>) 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (see <see cref="PresentationImage.SceneGraph"/>) when the mouse 
	/// is right-clicked.  If a non-empty <see cref="ActionModelNode"/> is returned, a context menu will be shown in the <see cref="ITile"/>.
    /// </para>
    /// <para>
    /// A context menu can also be invoked from within a tool (for example) by calling <see cref="ITile.ShowContextMenu"/>. For this
    /// reason, it is important to check that the input mouse coordinate is appropriate when deciding whether or not to return
    /// a non-empty <see cref="ActionModelNode"/>.
    /// </para>
	/// </remarks>
	/// <seealso cref="ITile"/>
	/// <seealso cref="IMouseInformation"/>
	/// <seealso cref="ActionModelNode"/>
	/// <seealso cref="IAction"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	public interface IContextMenuProvider
	{
		/// <summary>
		/// Gets the context menu <see cref="ActionModelNode"/> based on the current state of the mouse.
		/// </summary>
		/// <remarks>
		/// Because the context menu can be invoked at any time via <see cref="ITile.ShowContextMenu"/>,
		/// implementors of this method should check the <see cref="IMouseInformation.Location"/> property
		/// of the input <paramref name="mouseInformation"/> when deciding whether or not to return
		/// a non-empty <see cref="ActionModelNode"/>.
		/// </remarks>
		ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation);
	}
}
