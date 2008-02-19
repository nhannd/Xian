using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// A base class for tools that operate on <see cref="IGraphic"/>
	/// objects.
	/// </summary>
	public abstract class GraphicTool : Tool<IGraphicToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the tool is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

	}
}
