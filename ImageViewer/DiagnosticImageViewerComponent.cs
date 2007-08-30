using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an extension point for image layout management.
	/// </summary>
	[ExtensionPoint()]
	public class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
	{
	}

	/// <summary>
	/// An <see cref="ImageViewerComponent"/> that supports layouts.
	/// </summary>
	public class DiagnosticImageViewerComponent : ImageViewerComponent
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DiagnosticImageViewerComponent"/>.
		/// </summary>
		/// <remarks>Upon instantiation, an extension of 
		/// <see cref="LayoutManagerExtensionPoint"/> is automatically created.</remarks>
		/// <exception cref="NotSupportedException">An extension of <see cref="LayoutManagerExtensionPoint"/>
		/// could not be found.</exception>
		public DiagnosticImageViewerComponent() : base(CreateLayoutManager())
		{
		}


		#region IApplicationComponent methods

		public override void Start()
		{
			base.Start();
			
			// Can add DiagnosticImageViewerComponent specific tools here by calling 
			// this.ToolSet.AddTool().  We would need to define a 
			// DiagnosticImageViewerToolContext first.

		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		private static ILayoutManager CreateLayoutManager()
		{
			try
			{
				LayoutManagerExtensionPoint xp = new LayoutManagerExtensionPoint();
				ILayoutManager layoutManager = (ILayoutManager)xp.CreateExtension();
				return layoutManager;
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Error, e);
				throw e;
			}
		}
	}
}
