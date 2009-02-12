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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public sealed class AutoPresetVoiLutOperationComponent : DefaultPresetVoiLutOperationComponent
	{
		#region Lut Application

		private class LutApplicator
		{
			private readonly IPresentationImage _image;

			public LutApplicator(IPresentationImage image)
			{
				_image = image;
			}

			private Frame Frame
			{
				get
				{
					if (_image is IImageSopProvider)
						return ((IImageSopProvider)_image).Frame;

					return null;
				}
			}

			private IDicomVoiLutsProvider DicomVoiLutsProvider
			{
				get
				{
					if (_image is IDicomVoiLutsProvider)
						return (IDicomVoiLutsProvider) _image;
					return null;
				}
			}

			private GrayscalePixelData PixelData
			{
				get { return (GrayscalePixelData)((IImageGraphicProvider)_image).ImageGraphic.PixelData; }
			}

			private IVoiLutManager VoiLutManager
			{
				get { return ((IVoiLutProvider)_image).VoiLutManager; }
			}

			private IComposableLut CurrentLut
			{
				get { return VoiLutManager.GetLut(); }
			}

			private AutoVoiLutData GetDataLut()
			{
				IDicomVoiLutsProvider voiLutsProvider = this.DicomVoiLutsProvider;
				if (voiLutsProvider == null)
					return null;

				return AutoVoiLutData.CreateFrom(voiLutsProvider);
			}

			private AutoVoiLutLinear GetLinearLut()
			{
				IDicomVoiLutsProvider voiLutsProvider = this.DicomVoiLutsProvider;
				if (voiLutsProvider == null)
					return null;

				return AutoVoiLutLinear.CreateFrom(voiLutsProvider);
			}

			private MinMaxPixelCalculatedLinearLut GetMinMaxLut()
			{
				if (IsModalityLutProvider(_image))
					return new MinMaxPixelCalculatedLinearLut(PixelData, ((IModalityLutProvider)_image).ModalityLut);
				else
					return new MinMaxPixelCalculatedLinearLut(PixelData);
			}

			public IComposableLut GetInitialLut()
			{
				IComposableLut lut = GetDataLut();
				if (lut != null)
					lut = new AdjustableDataLut((AutoVoiLutData)lut);

				if (lut == null)
					lut = GetLinearLut();

				if (lut == null)
					lut = GetMinMaxLut();

				return lut;
			}

			public void ApplyInitialLut()
			{
				VoiLutManager.InstallLut(GetInitialLut());
			}

			public void ApplyNextLut()
			{
				IComposableLut currentLut = CurrentLut;
				IDicomVoiLutsProvider voiLutsProvider = this.DicomVoiLutsProvider;

				AdjustableDataLut adjustableDataLut = currentLut as AdjustableDataLut;
				if (adjustableDataLut != null && !(adjustableDataLut.DataLut is AutoVoiLutData))
					adjustableDataLut = null;

				AutoVoiLutLinear linearLut = currentLut as AutoVoiLutLinear;

				if (adjustableDataLut != null)
				{
					AutoVoiLutData dataLut = (AutoVoiLutData) adjustableDataLut.DataLut;
					if (dataLut.IsLast && AutoVoiLutLinear.CanCreateFrom(voiLutsProvider))
					{
						VoiLutManager.InstallLut(AutoVoiLutLinear.CreateFrom(voiLutsProvider));
					}
					else
					{
						dataLut.ApplyNext();
						adjustableDataLut.Reset();
					}
				}
				else if (linearLut != null)
				{
					if (linearLut.IsLast && AutoVoiLutData.CanCreateFrom(voiLutsProvider))
						VoiLutManager.InstallLut(new AdjustableDataLut(AutoVoiLutData.CreateFrom(voiLutsProvider)));
					else
						linearLut.ApplyNext();
				}
				else
				{
					ApplyInitialLut();
				}
			}

			public static bool CanCreateFrom(IPresentationImage presentationImage)
			{
				if (!IsVoiLutProvider(presentationImage))
					return false;

				if (!IsGrayScaleImage(presentationImage))
					return false;

				if (IsImageSopProvider(presentationImage))
				{
					IDicomVoiLutsProvider voiLutsProvider = presentationImage as IDicomVoiLutsProvider;
					if (AutoVoiLutLinear.CanCreateFrom(voiLutsProvider) || AutoVoiLutData.CanCreateFrom(voiLutsProvider))
						return true;
				}

				return false;
			}
		}

		#endregion

		public AutoPresetVoiLutOperationComponent()
		{
		}

		public override string Name
		{
			get { return SR.AutoPresetVoiLutOperationName; }
		}

		public override string Description
		{
			get { return SR.AutoPresetVoiLutOperationDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return LutApplicator.CanCreateFrom(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			// TODO: Later, when we've enabled all the factories, we need to change this functionality so it 
			// is purely 'auto'; no min/max algorithm, as it is currently.

			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException("The input presentation image is not supported.");
				
			new LutApplicator(presentationImage).ApplyNextLut();
		}

		internal static IComposableLut GetInitialLut(IPresentationImage presentationImage)
		{
			if (!LutApplicator.CanCreateFrom(presentationImage))
				return null;

			return new LutApplicator(presentationImage).GetInitialLut();
		}
	}
}
