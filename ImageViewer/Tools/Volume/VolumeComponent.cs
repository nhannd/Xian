using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	/// <summary>
	/// Extension point for views onto <see cref="VolumeComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class VolumeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// VolumeComponent class
	/// </summary>
	[AssociateView(typeof(VolumeComponentViewExtensionPoint))]
	public class VolumeComponent : ImageViewerToolComponent
	{
		private TissueSettingsCollection _tissueSettingsCollection;

		/// <summary>
		/// Constructor
		/// </summary>
		public VolumeComponent(IImageViewer imageViewer)
		{
			this.ImageViewer = imageViewer;
		}

		public bool CreateVolumeEnabled
		{
			get 
			{
				if (this.ImageViewer == null)
				{
					return false;
				}
				else
				{
					if (this.ImageViewer.SelectedTile == null)
						return false;
					else
						return !(this.ImageViewer.SelectedPresentationImage is VolumePresentationImage);
				}
			}
		}

		public bool VolumeSettingsEnabled
		{
			get
			{
				if (this.ImageViewer == null)
				{
					return false;
				}
				else
				{
					if (this.ImageViewer.SelectedTile == null)
						return false;
					else
						return this.ImageViewer.SelectedPresentationImage is VolumePresentationImage;
				}
			}
		}

		public LayerCollection VolumeLayers
		{
			get
			{
				if (this.ImageViewer == null)
					return null;

				if (this.ImageViewer.SelectedPresentationImage == null)
					return null;

				return this.ImageViewer.SelectedPresentationImage.LayerManager.RootLayerGroup.Layers;
			}
		}

		#region IApplicationComponent methods

		public override void Start()
		{
			base.Start();

		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#endregion


		public void CreateVolume()
		{
			if (this.ImageViewer == null)
				return;

			if (this.ImageViewer.SelectedImageBox == null)
				return;

			IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			VolumePresentationImage image = new VolumePresentationImage(selectedDisplaySet);

			AddTissueLayers(image);

			IDisplaySet displaySet = new DisplaySet();
			displaySet.Name = String.Format("{0} (3D)", selectedDisplaySet.Name);
			displaySet.PresentationImages.Add(image);
			this.ImageViewer.LogicalWorkspace.DisplaySets.Add(displaySet);

			IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			imageBox.DisplaySet = displaySet;
			imageBox.Draw();
			imageBox[0, 0].Select();

			UpdateFromImageViewer();
		}

		private void AddTissueLayers(VolumePresentationImage image)
		{
			LayerCollection layers = image.LayerManager.RootLayerGroup.Layers;

			TissueSettings tissue = new TissueSettings();
			tissue.SelectPreset("Bone");
			layers.Add(new VolumeLayer(tissue));

			tissue = new TissueSettings();
			tissue.SelectPreset("Blood");
			layers.Add(new VolumeLayer(tissue));
		}

	}
}
