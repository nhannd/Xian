
using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Abstract base class for <see cref="ILayoutManager"/>s.
	/// </summary>
	public abstract class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected LayoutManager()
		{
		}

		#region Protected Properties

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer"/>.
		/// </summary>
		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer.PhysicalWorkspace"/> property.
		/// </summary>
		protected IPhysicalWorkspace PhysicalWorkspace
		{
			get { return _imageViewer.PhysicalWorkspace; }	
		}

		/// <summary>
		/// Convenience property for retrieving the owner <see cref="IImageViewer.LogicalWorkspace"/> property.
		/// </summary>
		protected ILogicalWorkspace LogicalWorkspace
		{
			get { return _imageViewer.LogicalWorkspace; }
		}

		#endregion

		#region ILayoutManager Members

		/// <summary>
		/// Sets the owning <see cref="IImageViewer"/>.
		/// </summary>
		public virtual void SetImageViewer(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		/// <summary>
		/// Builds the <see cref="ILogicalWorkspace"/>, lays out and fills the <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// Internally, this method calls <see cref="BuildLogicalWorkspace"/>, <see cref="LayoutPhysicalWorkspace"/>
		/// and <see cref="FillPhysicalWorkspace"/> in that order, followed by a call to <see cref="IPhysicalWorkspace.Draw"/>.
		/// You can override this method entirely, or you can override any of the methods called by this method.
		/// </remarks>
		public virtual void Layout()
		{
			BuildLogicalWorkspace();
			LayoutPhysicalWorkspace();
			FillPhysicalWorkspace();
		
			ImageViewer.PhysicalWorkspace.Draw();
		}

		#endregion

		/// <summary>
		/// Builds the <see cref="ILogicalWorkspace"/>, creating an populating <see cref="ILogicalWorkspace.ImageSets"/>
		/// from the contents of <see cref="IImageViewer.StudyTree"/>.
		/// </summary>
		protected virtual void BuildLogicalWorkspace()
		{
			//TODO: Move the contents of this helper into here?
			SimpleLogicalWorkspaceBuilder.Build(ImageViewer);
		}

		//TODO: Just put the contents of SimpleLayoutManager in here and remove that class?
		/// <summary>
		/// Lays out the physical workspace, adding and setting up the <see cref="IPhysicalWorkspace.ImageBoxes"/>.
		/// </summary>
		protected abstract void LayoutPhysicalWorkspace();

		//TODO: moved SimplePhysicalWorkspaceFiller helper contents into here.  Any reason why we shouldn't do that?
		/// <summary>
		/// Fills <see cref="IPhysicalWorkspace.ImageBoxes"/> with <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <remarks>
		/// <see cref="IPhysicalWorkspace.ImageBoxes"/> is filled starting with the first 
		/// <see cref="IImageSet"/>'s <see cref="IDisplaySet"/>, and continuing until there
		/// are no empty <see cref="IImageBox"/>es or all <see cref="IDisplaySet"/>s have been assigned to an <see cref="IImageBox"/>.
		/// </remarks>
		protected virtual void FillPhysicalWorkspace()
		{
			IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
			ILogicalWorkspace logicalWorkspace = ImageViewer.LogicalWorkspace;

			int imageSetIndex = 0;
			int displaySetIndex = 0;

			foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
			{
				if (displaySetIndex == logicalWorkspace.ImageSets[imageSetIndex].DisplaySets.Count)
				{
					imageSetIndex++;
					displaySetIndex = 0;

					if (imageSetIndex == logicalWorkspace.ImageSets.Count)
						break;
				}

				imageBox.DisplaySet = logicalWorkspace.ImageSets[imageSetIndex].DisplaySets[displaySetIndex].CreateFreshCopy();
				displaySetIndex++;
			}
		}

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		#endregion
	}
}
