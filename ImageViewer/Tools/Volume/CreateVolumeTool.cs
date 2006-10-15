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
	// This template provides the boiler-plate code for creating a basic tool
	// that performs a single action when its menu item or toolbar button is clicked.

	// Declares a menu action with action ID "apply"
	// TODO: Change the action path hint to your desired menu path, or
	// remove this attribute if you do not want to create a menu item for this tool
	[MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/CreateVolumeTool")]

	// Declares a toolbar button action with action ID "apply"
	// TODO: Change the action path hint to your desired toolbar path, or
	// remove this attribute if you do not want to create a toolbar button for this tool
	[ButtonAction("apply", "global-toolbars/ToolbarMyTools/CreateVolumeTool")]

	// Specifies tooltip text for the "apply" action
	// TODO: Replace tooltip text
	[Tooltip("apply", "Place tooltip text here")]

	// Specifies icon resources to use for the "apply" action
	// TODO: Replace the icon resource names with your desired icon resources
	[IconSet("apply", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolMedium.png", "Icons.CreateVolumeToolLarge.png")]

	// Specifies that the "apply" action will be handled by a method named "Apply"
	[ClickHandler("apply", "Apply")]

	// Specifies that the enablement of the "apply" action in the user-interface
	// is controlled by observing a boolean property named "Enabled", listening to
	// an event named "EnabledChanged" for changes to this property
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	// Declares this tool as an extension of the ImageViewerToolExtensionPoint extension point.
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]

	public class CreateVolumeTool : Tool<IImageViewerToolContext>
	{
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

			// TODO: add any significant initialization code here rather than in the constructor


			// subscribe to any relevant events
			// you may wish to add other event subscriptions here and/or delete these if 
			// they are not needed
			this.ImageViewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
			this.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;

		}

		/// <summary>
		/// Called by the framework to determine whether this tool is enabled/disabled in the UI.
		/// You may change the name of this property as desired, but be sure to change the
		/// EnabledStateObserver attribute accordingly.
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
		/// Notifies the framework that the Enabled state of this tool has changed.
		/// You may change the name of this event as desired, but be sure to change the
		/// EnabledStateObserver attribute accordingly.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// You may change the name of this method as desired, but be sure to change the
		/// ClickHandler attribute accordingly.
		/// </summary>
		public void Apply()
		{
			IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			VolumePresentationImage image = new VolumePresentationImage(selectedDisplaySet);
			IDisplaySet displaySet = new DisplaySet();
			displaySet.PresentationImages.Add(image);
			this.ImageViewer.LogicalWorkspace.DisplaySets.Add(displaySet);
			IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			imageBox.DisplaySet = displaySet;
			imageBox.Draw();
		}

		/// <summary>
		/// Provides access to the <see cref="IImageViewer"/> associated with this tool.
		/// </summary>
		private IImageViewer ImageViewer
		{
			get { return this.Context.Viewer; }
		}

		/// <summary>
		/// Provides access to the currently selected ImageLayer.  Use methods on 
		/// DicomImageLayer to access pixel data and other image related paramters.
		/// Be sure to check for null before using.  If you do not require
		/// access to image data, delete this property.
		/// </summary>
		private DicomImageLayer SelectedImageLayer
		{
			get
			{
				IPresentationImage selectedPresentationImage = this.ImageViewer.SelectedPresentationImage;

				if (selectedPresentationImage == null)
					return null;

				LayerManager layerManager = selectedPresentationImage.LayerManager;

				if (layerManager == null)
					return null;

				ImageLayer imageLayer = layerManager.SelectedImageLayer;

				if (imageLayer == null)
					return null;

				return imageLayer as DicomImageLayer;
			}
		}


		/// <summary>
		/// Event handler called when a presentation image is selected.
		/// </summary>
		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			// TODO: add code to handle this event if necessary,
			// or optionally delete this handler if not needed
		}

		/// <summary>
		/// Event handler called when an image is about to be drawn.
		/// </summary>
		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			// TODO: add code to handle this event if necessary,
			// or optionally delete this handler if not needed
		}

	}
}
