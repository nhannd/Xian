#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An extension point for graphic tools.
	/// </summary>
	[ExtensionPoint()]
	public sealed class GraphicToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// An interface for graphic tools.
	/// </summary>
	public interface IGraphicToolContext : IToolContext
	{
		/// <summary>
		/// Gets the <see cref="IGraphic"/> that the tool applies to.
		/// </summary>
		IGraphic Graphic { get; }

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/>.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }
	}

	/// <summary>
	/// Base implementation of <see cref="IGraphicToolContext"/>.
	/// </summary>
	public class GraphicToolContext : ToolContext, IGraphicToolContext
	{
		private readonly IGraphic _graphic;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GraphicToolContext(IGraphic graphic)
		{
			_graphic = graphic;
		}

		/// <summary>
		/// Gets the graphic that the tool applies to.
		/// </summary>
		public IGraphic Graphic
		{
			get { return _graphic; }
		}

		/// <summary>
		/// Gets the owning <see cref="IDesktopWindow"/>.
		/// </summary>
		public IDesktopWindow DesktopWindow
		{
			get
			{
				if (_graphic.ImageViewer == null)
					return null;
				return _graphic.ImageViewer.DesktopWindow;
			}
		}
	}
}
