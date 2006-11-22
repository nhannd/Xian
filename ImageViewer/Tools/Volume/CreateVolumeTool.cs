using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	[MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/CreateVolumeTool")]
	[ButtonAction("apply", "global-toolbars/ToolbarMyTools/CreateVolumeTool")]
	[Tooltip("apply", "Create Volume")]
	[IconSet("apply", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolMedium.png", "Icons.CreateVolumeToolLarge.png")]
	[ClickHandler("apply", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]

	public class CreateVolumeTool : Tool<IImageViewerToolContext>
	{
		private VolumeComponent _volumeComponent;

		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public CreateVolumeTool()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			this.ImageViewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
			this.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;

		}

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

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void Apply()
		{
			if (_volumeComponent == null)
			{
				// create and initialize the layout component
				_volumeComponent = new VolumeComponent();
				//_volumeComponent.Subject = GetSubjectImageViewer();

				// launch the layout component in a shelf
				// note that the component is thrown away when the shelf is closed by the user
				ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_volumeComponent,
					"Volume Controller",
					ShelfDisplayHint.DockLeft,
					delegate(IApplicationComponent component) { _volumeComponent = null; });
			}


			//IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			//VolumePresentationImage image = new VolumePresentationImage(selectedDisplaySet);
			//IDisplaySet displaySet = new DisplaySet();
			//displaySet.Name = String.Format("{0} (3D)", selectedDisplaySet.Name);
			//displaySet.PresentationImages.Add(image);
			//this.ImageViewer.LogicalWorkspace.DisplaySets.Add(displaySet);
			//IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			//imageBox.DisplaySet = displaySet;
			//imageBox.Draw();
		}

		private IImageViewer ImageViewer
		{
			get { return this.Context.Viewer; }
		}


		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
		}
	}
}
