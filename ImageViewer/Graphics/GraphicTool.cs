using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An extension point for graphic tools.
	/// </summary>
	[ExtensionPoint()]
	public class GraphicToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// An interface for graphic tools.
	/// </summary>
	public interface IGraphicToolContext : IToolContext
	{
		/// <summary>
		/// Gets the graphic that the tool applies to.
		/// </summary>
		IGraphic Graphic { get; }
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
	}
}
