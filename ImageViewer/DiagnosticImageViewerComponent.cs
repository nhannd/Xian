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
	public class DiagnosticImageViewerComponent : ImageViewerComponent
	{
		private ILayoutManager _layoutManager;
		private string _studyInstanceUID;

		public DiagnosticImageViewerComponent(string studyInstanceUID)
		{
			Platform.CheckForEmptyString(studyInstanceUID, "studyInstanceUID");

			_studyInstanceUID = studyInstanceUID;
		}

		public override void Start()
		{
			base.Start();
			
			// Can add DiagnosticImageViewerComponent specific tools here by calling 
			// this.ToolSet.AddTool().  We would need to define a 
			// DiagnosticImageViewerToolContext first.

			CreateLayoutManager();
			ApplyLayout();
		}

		public override void Stop()
		{
			// TODO: What would be better is if the study tree listened for workspaces
			// being addded/removed then increased/decreased the reference count itself.
			StudyManager.StudyTree.DecrementStudyReferenceCount(_studyInstanceUID);

			base.Stop();
		}

		protected void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				if (_layoutManager != null)
					_layoutManager.Dispose();
			}
		}

		private void CreateLayoutManager()
		{
			try
			{
				LayoutManagerExtensionPoint xp = new LayoutManagerExtensionPoint();
				_layoutManager = (ILayoutManager)xp.CreateExtension();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(e, LogLevel.Warn);
			}
		}

		/// <summary>
		/// Applies a layout to the workspace.
		/// </summary>
		/// <remarks>
		/// This method signature is preliminary and will likely change.
		/// </remarks>
		private void ApplyLayout()
		{
			if (_layoutManager == null)
				throw new NotSupportedException(SR.ExceptionLayoutManagerDoesNotExist);

			_layoutManager.ApplyLayout(this.LogicalWorkspace, this.PhysicalWorkspace, _studyInstanceUID);
			StudyManager.StudyTree.IncrementStudyReferenceCount(_studyInstanceUID);
		}
	}
}
