#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarProbe", "Select", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardProbe/Activate", "Select", KeyStroke = XKeys.B)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ProbeToolSmall.png", "Icons.ProbeToolMedium.png", "Icons.ProbeToolLarge.png")]
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
			this.Behaviour = MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
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

		private void Probe(Point destinationPoint)
		{
			Point sourcePointRounded = Point.Truncate(_selectedImageGraphic.SpatialTransform.ConvertToSource(destinationPoint));

			//!! Make these user preferences later.
			bool showPixelValue = true;
			bool showModalityValue = true;
			bool showVoiValue = true;

			string probeString = String.Format("LOC: x={0}, y={1}", SR.LabelNotApplicable, SR.LabelNotApplicable);
			string pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, SR.LabelNotApplicable);
			string modalityLutString = String.Format("{0}: {1}", SR.LabelModalityLut, SR.LabelNotApplicable);
			string voiLutString = String.Format("{0}: {1}", SR.LabelVOILut, SR.LabelNotApplicable);

			if (_selectedImageGraphic.HitTest(destinationPoint))
			{
				probeString = String.Format("LOC: x={0}, y={1}", sourcePointRounded.X, sourcePointRounded.Y);

				if (_selectedImageGraphic is GrayscaleImageGraphic)
				{
					GrayscaleImageGraphic image = _selectedImageGraphic as GrayscaleImageGraphic;

					int pixelValue = 0;
					int modalityLutValue = 0;
					int voiLutValue = 0;

					GetPixelValue(image, sourcePointRounded, ref pixelValue, ref pixelValueString);
					GetModalityLutValue(image, pixelValue, ref modalityLutValue, ref modalityLutString);
					GetVoiLutValue(image, modalityLutValue, ref voiLutValue, ref voiLutString);
				}
				else if (_selectedImageGraphic is ColorImageGraphic)
				{
					showModalityValue = false;
					showVoiValue = false;

					ColorImageGraphic image = _selectedImageGraphic as ColorImageGraphic;
					Color color = image.PixelData.GetPixelAsColor(sourcePointRounded.X, sourcePointRounded.Y);
					string rgbFormatted = String.Format(SR.FormatRGB, color.R, color.G, color.B);
					pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, rgbFormatted);
				}
				else if (_selectedImageGraphic is PaletteColorImageGraphic)
				{
					showModalityValue = false;
					showVoiValue = false;

					PaletteColorImageGraphic image = _selectedImageGraphic as PaletteColorImageGraphic;

					int pixelValue = image.PixelData.GetPixel(sourcePointRounded.X, sourcePointRounded.Y);
					Color color = Color.FromArgb(image.ColorMap[pixelValue]);
					string rgbFormatted = String.Format(SR.FormatPaletteColor, pixelValue, color.R, color.G, color.B);
					pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, rgbFormatted);
				}
				else
				{
					showPixelValue = false;
					showModalityValue = false;
					showVoiValue = false;
				}
			}

			if (showPixelValue)
				probeString += "\n" + pixelValueString;
			if (showModalityValue)
				probeString += "\n" + modalityLutString;
			if (showVoiValue)
				probeString += "\n" + voiLutString;

			_selectedTile.InformationBox.Update(probeString, destinationPoint);
		}

		private void GetPixelValue(
			IndexedImageGraphic grayscaleImage, 
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
	}
}
