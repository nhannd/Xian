using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides selection support.
	/// </summary>
	public interface ISelectableGraphic : IGraphic
	{
		/// <summary>
		/// Gets or set a value indicating whether the <see cref="IGraphic"/> is selected.
		/// </summary>
		bool Selected { get; set; }
	}
}
