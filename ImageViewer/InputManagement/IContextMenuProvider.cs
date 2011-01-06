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
	/// The framework will look for this interface on graphic objects (<see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>) 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (see <see cref="PresentationImage.SceneGraph"/>) when the mouse 
	/// is right-clicked.  If an <see cref="ActionModelNode"/> is returned, a context menu will be shown in the <see cref="ITile"/>.
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
		ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation);
	}
}
