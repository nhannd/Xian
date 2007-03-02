using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An extension point for image layout management.
	/// </summary>
	[ExtensionPoint()]
	public class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
	{
	}

	/// <summary>
	/// An <see cref="ImageViewerComponent"/> that supports layouts.
	/// </summary>
	[AssociateView(typeof(ImageViewerComponentViewExtensionPoint))]
	public class DiagnosticImageViewerComponent : LayoutCapableImageViewerComponent
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DiagnosticImageViewerComponent"/>.
		/// </summary>
		/// <remarks>If this constructor is called, an extension of 
		/// <see cref="LayoutManagerExtensionPoint"/> is automatically created.</remarks>
		/// <exception cref="NotSupportedException">An extension of <see cref="LayoutManagerExtensionPoint"/>
		/// could not be found.</exception>
		public DiagnosticImageViewerComponent()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DiagnosticImageViewerComponent"/>
		/// with a specified layout manager.
		/// </summary>
		/// <param name="layoutManager">The layout manager to be used.</param>
		public DiagnosticImageViewerComponent(ILayoutManager layoutManager)
			: base(layoutManager)
		{
		}

		#region Public methods

		/// <summary>
		/// Lays out the images in the <see cref="DiagnosticImageViewerComponent"/> using
		/// the current layout manager.
		/// </summary>
		public void Layout()
		{
			this.LayoutManager.Layout();
		}

		#endregion

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

		/// <summary>
		/// Creates an <see cref="ILayoutManager"/>.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <see cref="CreateLayoutManager"/> creates the first extension that
		/// extends the <see cref="LayoutManagerExtensionPoint"/>.
		/// </remarks>
		/// <exception cref="NotSupportedException">An extension that extends
		/// the <see cref="LayoutManagerExtensionPoint"/> could not be found.
		/// </exception>
		protected override ILayoutManager CreateLayoutManager()
		{
			try
			{
				LayoutManagerExtensionPoint xp = new LayoutManagerExtensionPoint();
				ILayoutManager layoutManager = (ILayoutManager)xp.CreateExtension();
				return layoutManager;
			}
			catch (NotSupportedException e)
			{
				Platform.Log(e, LogLevel.Warn);
				throw e;
			}
		}
	}
}
