using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;
using System.Drawing;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Mathematics;
using System.Diagnostics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuToolsStandardProbe", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/Standard/MenuToolsStandardProbe", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarToolsStandardProbe", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardProbe/Activate", KeyStroke = XKeys.B)]
	[Tooltip("activate", "ToolbarToolsStandardProbe")]
	[IconSet("activate", IconScheme.Colour, "Icons.ProbeToolSmall.png", "Icons.ProbeToolMedium.png", "Icons.ProbeToolLarge.png")]
	[ClickHandler("activate", "Select")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]

	[MouseToolButton(XMouseButtons.Left, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ProbeTool : MouseTool
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		private Tile _selectedTile;
		private ImageLayer _selectedImageLayer;
		private PixelDataWrapper _wrapper;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public ProbeTool()
		{
			_enabled = true;
			this.CursorToken = new CursorToken("Icons.ProbeToolMedium.png", this.GetType().Assembly);
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
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

		public override bool Start(IMouseInformation mouseInformation)
		{
			//base.Start(mouseInformation);

			if (!IsImageValid(mouseInformation.Tile.PresentationImage))
				return false;

			_selectedTile = mouseInformation.Tile as Tile;
			_selectedTile.InformationBox = new InformationBox();
			_selectedImageLayer = _selectedTile.PresentationImage.LayerManager.SelectedImageLayer;

			_wrapper = new PixelDataWrapper
				(
					_selectedImageLayer.Columns,
					_selectedImageLayer.Rows,
					_selectedImageLayer.BitsAllocated,
					_selectedImageLayer.BitsStored,
					_selectedImageLayer.HighBit,
					_selectedImageLayer.SamplesPerPixel,
					_selectedImageLayer.PixelRepresentation,
					_selectedImageLayer.PlanarConfiguration,
					_selectedImageLayer.PhotometricInterpretation,
					_selectedImageLayer.GetPixelData()
				);

			Probe(mouseInformation.Location);

			return true;
		}

		/// <summary>
		/// Called by the framework as the mouse moves while the assigned mouse button
		/// is pressed.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_selectedTile == null || _selectedImageLayer == null)
				return false;

			//base.Track(mouseInformation);

			Probe(mouseInformation.Location);
			
			return true;
		}

		/// <summary>
		/// Called by the framework when the assigned mouse button is released.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();			
			return false;
		}

		public override void Cancel()
		{
			if (_selectedTile == null || _selectedImageLayer == null)
				return;

			//base.Stop(mouseInformation);

			_selectedImageLayer = null;

			_selectedTile.InformationBox.Visible = false;
			_selectedTile.InformationBox = null;
			_selectedTile = null;

			_wrapper = null;
		}

		private void Probe(Point destinationPoint)
		{
			PointUtilities.ConfinePointToRectangle(ref destinationPoint, _selectedImageLayer.SpatialTransform.DestinationRectangle);
			Point sourcePointRounded = Point.Round(_selectedImageLayer.SpatialTransform.ConvertToSource(destinationPoint));

			//!! Make these user preferences later.
			bool showPixelValue = true;
			bool showModalityValue = true;
			bool showVoiValue = true;

			string pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, SR.LabelNotApplicable);
			string modalityLutString = String.Format("{0}: {1}", SR.LabelModalityLut, SR.LabelNotApplicable);
			string voiLutString = String.Format("{0}: {1}", SR.LabelVOILut, SR.LabelNotApplicable);

			if (_selectedImageLayer.SpatialTransform.SourceRectangle.Contains(sourcePointRounded))
			{
				if (!_selectedImageLayer.IsColor)
				{
					int pixelValue = 0;
					int modalityLutValue = 0;
					int voiLutValue = 0;

					string formatString = "{0}: {1}";

					if (_selectedImageLayer.BitsAllocated == 16)
						pixelValue = _wrapper.GetPixel16(sourcePointRounded.X, sourcePointRounded.Y);
					else
						pixelValue = _wrapper.GetPixel8(sourcePointRounded.X, sourcePointRounded.Y);

					if (_selectedImageLayer.IsSigned)
						pixelValue = (short)pixelValue;

					GrayscaleLUTPipeline pipeline = _selectedImageLayer.GrayscaleLUTPipeline;
					if (pipeline != null)
					{
						if (pipeline.ModalityLUT != null)
						{
							modalityLutValue = pipeline.ModalityLUT[pixelValue];
							modalityLutString = String.Format(formatString, SR.LabelModalityLut, modalityLutValue);

							if (_selectedTile.PresentationImage is DicomPresentationImage)
							{ 
								DicomPresentationImage image = _selectedTile.PresentationImage as DicomPresentationImage;
								if (String.Compare(image.ImageSop.Modality, "CT", true) == 0)
									modalityLutString += String.Format(" ({0})", SR.LabelHounsfieldUnitsAbbreviation);
							}
						}

						if (pipeline.VoiLUT != null)
						{
							voiLutValue = pipeline.VoiLUT[modalityLutValue];
							voiLutString = String.Format(formatString, SR.LabelVOILut, voiLutValue);
						}
					}

					pixelValueString = String.Format(formatString, SR.LabelPixelValue, pixelValue);
				}
				else
				{
					showModalityValue = false;
					showVoiValue = false;

					Color color = _wrapper.GetPixelRGB(sourcePointRounded.X, sourcePointRounded.Y);
					string rgbFormatted = String.Format("RGB({0}, {1}, {2})", color.R, color.G, color.B);
					pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, rgbFormatted);
				}
			}

			string probeString = String.Format("x, y: {0}, {1}", sourcePointRounded.X, sourcePointRounded.Y);

			if (showPixelValue)
				probeString = probeString + "\n" + pixelValueString;
			if (showModalityValue)
				probeString = probeString + "\n" + modalityLutString;
			if (showVoiValue)
				probeString = probeString + "\n" + voiLutString;

			_selectedTile.InformationBox.Update(probeString, destinationPoint);
		}
	}
}
