using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Imaging;
using System.Drawing;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Mathematics;
using System.Diagnostics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuProbe", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuProbe", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarProbe", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardProbe/Activate", KeyStroke = XKeys.B)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ProbeToolSmall.png", "Icons.ProbeToolMedium.png", "Icons.ProbeToolLarge.png")]
	[ClickHandler("activate", "Select")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[GroupHint("activate", "Tools.Image.Interrogation.Probe")]

	[MouseToolButton(XMouseButtons.Left, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ProbeTool : MouseImageViewerTool
	{
		private Tile _selectedTile;
		private ImageGraphic _selectedImageGraphic;
		private ImageSop _selectedImageSop;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public ProbeTool()
			: base(SR.TooltipProbe)
		{
			this.CursorToken = new CursorToken("ProbeCursor.png", this.GetType().Assembly);
		}

		public override string Tooltip
		{
			get { return base.Tooltip; }
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}


		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedImageGraphicProvider == null)
				return false;

			_selectedTile = mouseInformation.Tile as Tile;
			_selectedTile.InformationBox = new InformationBox();
			_selectedImageGraphic = this.SelectedImageGraphicProvider.ImageGraphic;
			_selectedImageSop = (this.SelectedPresentationImage as IImageSopProvider).ImageSop;

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
			if (_selectedTile == null || _selectedImageGraphic == null)
				return false;

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
			if (_selectedTile == null || _selectedImageGraphic == null)
				return;

			_selectedImageGraphic = null;

			_selectedTile.InformationBox.Visible = false;
			_selectedTile.InformationBox = null;
			_selectedTile = null;
		}

		public override bool ConstrainToTile
		{
			get { return true; }
		}

		private void Probe(Point destinationPoint)
		{
			PointF sourcePoint = _selectedImageGraphic.SpatialTransform.ConvertToSource(destinationPoint);
			Point sourcePointRounded = Point.Truncate(_selectedImageGraphic.SpatialTransform.ConvertToSource(destinationPoint));

			//!! Make these user preferences later.
			bool showPixelValue = true;
			bool showModalityValue = true;
			bool showVoiValue = true;
			bool showPresentationValue = true;

			string probeString = String.Format("LOC: x={0}, y={1}", SR.LabelNotApplicable, SR.LabelNotApplicable);
			string pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, SR.LabelNotApplicable);
			string modalityLutString = String.Format("{0}: {1}", SR.LabelModalityLut, SR.LabelNotApplicable);
			string voiLutString = String.Format("{0}: {1}", SR.LabelVOILut, SR.LabelNotApplicable);
			string presentationLutString = String.Format("{0}: {1}", SR.LabelPresentationLut, SR.LabelNotApplicable);

			Rectangle imageRectangle = new Rectangle(0, 0, _selectedImageGraphic.Columns, _selectedImageGraphic.Rows);

			if (_selectedImageGraphic.HitTest(destinationPoint))
			{
				probeString = String.Format("LOC: x={0}, y={1}", sourcePointRounded.X, sourcePointRounded.Y);

				if (_selectedImageGraphic is GrayscaleImageGraphic)
				{
					GrayscaleImageGraphic grayscaleImage = _selectedImageGraphic as GrayscaleImageGraphic;

					if (grayscaleImage != null)
					{
						int pixelValue = 0;
						int modalityLutValue = 0;
						int voiLutValue = 0;
						int presentationLutValue = 0;

						GetPixelValue(grayscaleImage, sourcePointRounded, ref pixelValue, ref pixelValueString);
						GetModalityLutValue(grayscaleImage, pixelValue, ref modalityLutValue, ref modalityLutString);
						GetVoiLutValue(grayscaleImage, modalityLutValue, ref voiLutValue, ref voiLutString);
						GetPresentationLutValue(grayscaleImage, voiLutValue, ref presentationLutValue, ref presentationLutString);
					}
				}
				else
				{
					showModalityValue = false;
					showVoiValue = false;
					showPresentationValue = false;

					Color color = ((ColorImageGraphic)_selectedImageGraphic).PixelData.GetPixelAsColor(sourcePointRounded.X, sourcePointRounded.Y);
					string rgbFormatted = String.Format("R={0}, G={1}, B={2})", color.R, color.G, color.B);
					pixelValueString = String.Format("{0:F2}: {1:F2}", SR.LabelPixelValue, rgbFormatted);
				}
			}

			//string probeString = String.Format("LOC: x={0}, y={1}", sourcePoint.X, sourcePoint.Y);

			if (showPixelValue)
				probeString += "\n" + pixelValueString;
			if (showModalityValue)
				probeString += "\n" + modalityLutString;
			if (showVoiValue)
				probeString += "\n" + voiLutString;
			if (showPresentationValue)
				probeString += "\n" + presentationLutString;

			_selectedTile.InformationBox.Update(probeString, destinationPoint);
		}

		private void GetPixelValue(
			GrayscaleImageGraphic grayscaleImage, 
			Point sourcePointRounded, 
			ref int pixelValue,
			ref string pixelValueString)
		{
			pixelValue = grayscaleImage.PixelData.GetPixel(sourcePointRounded.X, sourcePointRounded.Y);
			pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, pixelValue);
		}

		private void GetModalityLutValue(
			GrayscaleImageGraphic grayscaleImage, 
			int pixelValue, 
			ref int modalityLutValue,
			ref string modalityLutString)
		{
			if (grayscaleImage.ModalityLut != null)
			{
				modalityLutValue = grayscaleImage.ModalityLut[pixelValue];
				modalityLutString = String.Format("{0}: {1}", SR.LabelModalityLut, modalityLutValue);

				if (_selectedImageSop != null)
				{
					if (String.Compare(_selectedImageSop.Modality, "CT", true) == 0)
						modalityLutString += String.Format(" ({0})", SR.LabelHounsfieldUnitsAbbreviation);
				}
			}
		}

		private void GetVoiLutValue(
			GrayscaleImageGraphic grayscaleImage, 
			int modalityLutValue, 
			ref int voiLutValue, 
			ref string voiLutString)
		{
			if (grayscaleImage.VoiLut != null)
			{
				voiLutValue = grayscaleImage.VoiLut[modalityLutValue];
				voiLutString = String.Format("{0}: {1}", SR.LabelVOILut, voiLutValue);
			}
		}


		private void GetPresentationLutValue(
			GrayscaleImageGraphic grayscaleImage, 
			int voiLutValue, 
			ref int presentationLutValue,
			ref string presentationLutString)
		{
			if (grayscaleImage.PresentationLut != null)
			{
				presentationLutValue = grayscaleImage.PresentationLut[voiLutValue];
				Color color = Color.FromArgb(presentationLutValue);
				presentationLutString = String.Format("{0}: R={1}, G={2}, B={3}", SR.LabelPresentationLut, color.R, color.G, color.B);
			}
		}
	}
}
