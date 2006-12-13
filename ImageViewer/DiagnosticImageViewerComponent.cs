using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	[ExtensionPoint()]
	public class DiagnosticLayoutManagerExtensionPoint : ExtensionPoint<IDiagnosticLayoutManager>
	{
	}

	[AssociateView(typeof(ImageViewerComponentViewExtensionPoint))]
	public class DiagnosticImageViewerComponent : LayoutCapableImageViewerComponent
	{

		public DiagnosticImageViewerComponent()
		{
		}

		public DiagnosticImageViewerComponent(IDiagnosticLayoutManager layoutManager)
			: base(layoutManager)
		{
		}

		private IDiagnosticLayoutManager DiagnosticLayoutManager
		{
			get { return this.LayoutManager as IDiagnosticLayoutManager; }
		}

		#region Public methods

		public void AddStudy(string studyInstanceUID)
		{
			this.DiagnosticLayoutManager.AddStudy(studyInstanceUID);
		}

		public void AddSeries(string seriesInstanceUID)
		{
			this.DiagnosticLayoutManager.AddSeries(seriesInstanceUID);
		}

		public void AddImage(string sopInstanceUID)
		{
			this.DiagnosticLayoutManager.AddImage(sopInstanceUID);
		}

		public void Layout()
		{
			this.DiagnosticLayoutManager.Layout();
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
			// TODO: What would be better is if the study tree listened for workspaces
			// being addded/removed then increased/decreased the reference count itself.
			//StudyManager.StudyTree.DecrementStudyReferenceCount(_studyInstanceUID);

			base.Stop();
		}

		#endregion

		#region Private methods

		protected override void CreateLayoutManager()
		{
			try
			{
				DiagnosticLayoutManagerExtensionPoint xp = new DiagnosticLayoutManagerExtensionPoint();
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
