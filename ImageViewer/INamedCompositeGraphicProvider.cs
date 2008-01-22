using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A provider interface for images that have named <see cref="CompositeGraphic"/>s, providing
	/// a way to group certain graphics together.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Due the the plugin nature of the framework, the only way to enforce the order
	/// of graphic layers in the scene graph is by providing these named <see cref="CompositeGraphic"/>s
	/// that are ordered internally through configuration.  Graphics added to these
	/// <see cref="CompositeGraphic"/>s will appear to be grouped together, which is
	/// the desired effect.
	/// </para>
	/// <para>
	/// As an example, consider a reference line tool and a spine labelling tool.  In both cases,
	/// we would want these graphics to appear separately from each other and also from any user
	/// annotations such as ROIs.  Typically, we would want these non-interactive graphics (spine labels
	/// and reference lines) to appear not only grouped together, but 'underneath' any user defined graphics.
	/// </para>
	/// </remarks>
	public interface INamedCompositeGraphicProvider
	{
		/// <summary>
		/// Gets the <see cref="CompositeGraphic"/> with the given name, or null.
		/// </summary>
		CompositeGraphic GetNamedCompositeGraphic(string name);
	}
}
