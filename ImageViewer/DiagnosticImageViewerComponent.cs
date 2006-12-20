using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	[ExtensionPoint()]
	public class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
	{
	}

	[AssociateView(typeof(ImageViewerComponentViewExtensionPoint))]
	public class DiagnosticImageViewerComponent : LayoutCapableImageViewerComponent
	{

		public DiagnosticImageViewerComponent()
		{
		}

		public DiagnosticImageViewerComponent(ILayoutManager layoutManager)
			: base(layoutManager)
		{
		}

		#region Public methods

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

		#region Private methods

		protected override void CreateLayoutManager()
		{
			try
			{
				LayoutManagerExtensionPoint xp = new LayoutManagerExtensionPoint();
				this.LayoutManager = (ILayoutManager)xp.CreateExtension();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(e, LogLevel.Warn);
				throw e;
			}
		}

		#endregion
	}
}
