#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionPoint]
	public sealed class ProbeToolDropDownToolExtensionPoint : ExtensionPoint<ITool> { }

	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarProbe", "Select", "DropDownMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.B)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ProbeToolSmall.png", "Icons.ProbeToolMedium.png", "Icons.ProbeToolLarge.png")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[GroupHint("activate", "Tools.Image.Interrogation.Probe")]

	[MouseToolButton(XMouseButtons.Left, false)]

	#region Tool Settings Actions

	[MenuAction("showCTPix", "probetool-dropdown/ShowCTPix", "ToggleShowCTPix")]
	[CheckedStateObserver("showCTPix", "ShowCTPix", "ShowCTPixChanged")]
	[Tooltip("showCTPix", "TooltipShowCTPix")]
	[GroupHint("showCTPix", "Tools.Image.Interrogation.Probe.Modality.CT.ShowPixel")]

	[MenuAction("showNonCTMod", "probetool-dropdown/ShowNonCTMod", "ToggleShowNonCTMod")]
	[CheckedStateObserver("showNonCTMod", "ShowNonCTMod", "ShowNonCTModChanged")]
	[Tooltip("showNonCTMod", "TooltipShowNonCTMod")]
	[GroupHint("showNonCTMod", "Tools.Image.Interrogation.Probe.Modality.NonCT.ShowMod")]

	[MenuAction("showVoiLut", "probetool-dropdown/ShowVoiLut", "ToggleShowVoiLut")]
	[CheckedStateObserver("showVoiLut", "ShowVoiLut", "ShowVoiLutChanged")]
	[Tooltip("showVoiLut", "TooltipShowVoiLut")]
	[GroupHint("showVoiLut", "Tools.Image.Interrogation.Probe.General.ShowVoiLut")]

	#endregion

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ProbeTool : MouseImageViewerTool
	{
		private Tile _selectedTile;
		private ImageGraphic _selectedImageGraphic;
		private ImageSop _selectedImageSop;
		private ActionModelNode _actionModel;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public ProbeTool()
			: base(SR.TooltipProbe)
		{
			this.CursorToken = new CursorToken("ProbeCursor.png", this.GetType().Assembly);
			Behaviour |= MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode DropDownMenuModel
		{
			get
			{
				if (_actionModel == null)
				{
					_actionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "probetool-dropdown", this.Actions);
				}

				return _actionModel;
			}
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

			ToolSettings settings = ToolSettings.Default;
			bool isCT = (String.Compare(_selectedImageSop.Modality, "CT", true) == 0);
			bool showPixelValue = !isCT || settings.ShowCTRawPixelValue;
			bool showModalityValue = isCT || settings.ShowNonCTModPixelValue;
			bool showVoiValue = settings.ShowVOIPixelValue;

			string probeString = String.Format("LOC: x={0}, y={1}", SR.LabelNotApplicable, SR.LabelNotApplicable);
			string pixelValueString = String.Format("{0}: {1}", SR.LabelPixelValue, SR.LabelNotApplicable);
			string modalityLutString = String.Format("{0}: {1}", SR.LabelModalityLut, SR.LabelNotApplicable);
			string voiLutString = String.Format("{0}: {1}", SR.LabelVOILut, SR.LabelNotApplicable);

			try
			{
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
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				probeString = SR.MessageProbeToolError;
			}

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

		#region Probe Tool Settings

		private event EventHandler _showCTPixChanged;
		private event EventHandler _showNonCTModChanged;
		private event EventHandler _showVoiLutChanged;
		private ToolSettings _settings;

		public override void Initialize()
		{
			base.Initialize();

			_settings = ToolSettings.Default;
			_settings.PropertyChanged += OnPropertyChanged;
		}

		protected override void Dispose(bool disposing)
		{

			_settings.PropertyChanged -= OnPropertyChanged;
			_settings.Save();
			_settings = null;

			base.Dispose(disposing);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "ShowCTRawPixelValue":
					EventsHelper.Fire(_showCTPixChanged, this, EventArgs.Empty);
					break;
				case "ShowNonCTModPixelValue":
					EventsHelper.Fire(_showNonCTModChanged, this, EventArgs.Empty);
					break;
				case "ShowVOIPixelValue":
					EventsHelper.Fire(_showVoiLutChanged, this, EventArgs.Empty);
					break;
			}
		}

		public event EventHandler ShowCTPixChanged
		{
			add { _showCTPixChanged += value; }
			remove { _showCTPixChanged -= value; }
		}

		public event EventHandler ShowNonCTModChanged
		{
			add { _showNonCTModChanged += value; }
			remove { _showNonCTModChanged -= value; }
		}

		public event EventHandler ShowVoiLutChanged
		{
			add { _showVoiLutChanged += value; }
			remove { _showVoiLutChanged -= value; }
		}

		public bool ShowCTPix
		{
			get
			{
				try
				{
					return _settings.ShowCTRawPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				_settings.ShowCTRawPixelValue = value;
			}
		}

		public bool ShowNonCTMod
		{
			get
			{
				try
				{
					return _settings.ShowNonCTModPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				_settings.ShowNonCTModPixelValue = value;
			}
		}

		public bool ShowVoiLut
		{
			get
			{
				try
				{
					return _settings.ShowVOIPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				_settings.ShowVOIPixelValue = value;
			}
		}

		public void ToggleShowCTPix()
		{
			this.ShowCTPix = !this.ShowCTPix;
		}

		public void ToggleShowNonCTMod()
		{
			this.ShowNonCTMod = !this.ShowNonCTMod;
		}

		public void ToggleShowVoiLut()
		{
			this.ShowVoiLut = !this.ShowVoiLut;
		}

		#endregion
	}
}
