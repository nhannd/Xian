#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
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
				get { return (GrayscalePixelData) ((IImageGraphicProvider) _image).ImageGraphic.PixelData; }
			}

			private IVoiLutManager VoiLutManager
			{
				get { return ((IVoiLutProvider) _image).VoiLutManager; }
			}

			private IComposableLut CurrentLut
			{
				get { return this.VoiLutManager.VoiLut; }
			}

			private MinMaxPixelCalculatedLinearLut GetDefaultMinMaxLut()
			{
				if (IsModalityLutProvider(_image))
					return new MinMaxPixelCalculatedLinearLut(this.PixelData, ((IModalityLutProvider) _image).ModalityLut);
				else
					return new MinMaxPixelCalculatedLinearLut(this.PixelData);
			}

			public IComposableLut GetInitialLut()
			{
				foreach (State state in _stateProgression)
				{
					IComposableLut lut = state.GetLut(this);
					if (lut != null)
						return lut;
				}
				return this.GetDefaultMinMaxLut();
			}

			#region State Machine

			private class State
			{
				private delegate IComposableLut LutGetter(LutApplicator applicator);

				private readonly LutGetter _lutGetter;

				private State(LutGetter lutGetter)
				{
					_lutGetter = lutGetter;
				}

				public IComposableLut GetLut(LutApplicator applicator)
				{
					return _lutGetter(applicator);
				}

				public static State GetState(IComposableLut currentLut)
				{
					if (currentLut is AdjustableDataLut)
					{
						AdjustableDataLut adj = (AdjustableDataLut) currentLut;
						if (adj.DataLut is AutoPresentationVoiDataLut)
							return PresentationData;
						else if (adj.DataLut is AutoImageVoiDataLut)
							return ImageData;
					}
					else if (currentLut is AutoPresentationVoiLutLinear)
					{
						return PresentationLinear;
					}
					else if (currentLut is AutoImageVoiLutLinear)
					{
						return ImageLinear;
					}
					return null;
				}

				public static readonly State PresentationData = new State(GetPresentationDataLut);
				public static readonly State PresentationLinear = new State(GetPresentationLinearLut);
				public static readonly State ImageData = new State(GetImageDataLut);
				public static readonly State ImageLinear = new State(GetImageLinearLut);

				private static AdjustableDataLut GetImageDataLut(LutApplicator applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;

					AutoVoiDataLut dataLut = AutoImageVoiDataLut.CreateFrom(voiLutsProvider);
					if (dataLut == null)
						return null;
					return new AdjustableAutoVoiDataLut(dataLut);
				}

				private static AutoVoiLutLinear GetImageLinearLut(LutApplicator applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;
					return AutoImageVoiLutLinear.CreateFrom(voiLutsProvider);
				}

				private static AdjustableDataLut GetPresentationDataLut(LutApplicator applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;

					AutoVoiDataLut dataLut = AutoPresentationVoiDataLut.CreateFrom(voiLutsProvider);
					if (dataLut == null)
						return null;
					return new AdjustableAutoVoiDataLut(dataLut);
				}

				private static AutoVoiLutLinear GetPresentationLinearLut(LutApplicator applicator)
				{
					IDicomVoiLutsProvider voiLutsProvider = applicator.DicomVoiLutsProvider;
					if (voiLutsProvider == null)
						return null;
					return AutoPresentationVoiLutLinear.CreateFrom(voiLutsProvider);
				}
			}

			#endregion

			private static readonly IList<State> _stateProgression = new State[] {State.PresentationData, State.PresentationLinear, State.ImageData, State.ImageLinear};

			public void ApplyInitialLut()
			{
				this.VoiLutManager.InstallVoiLut(this.GetInitialLut());
			}

			public void ApplyNextLut()
			{
				IComposableLut currentLut = this.CurrentLut;
				State currentState = State.GetState(currentLut);

				if (currentLut is IAutoVoiLut)
				{
					IAutoVoiLut autoVoiLut = (IAutoVoiLut) currentLut;
					if (autoVoiLut.IsLast)
					{
						int nextState = _stateProgression.IndexOf(currentState) + 1;
						for (int n = nextState; n < nextState + _stateProgression.Count; n++)
						{
							IComposableLut lut = _stateProgression[(n%_stateProgression.Count)].GetLut(this);
							if (lut != null)
							{
								this.VoiLutManager.InstallVoiLut(lut);
								return;
							}
						}
					}
					else
					{
						autoVoiLut.ApplyNext();
						return;
					}
				}
				else
				{
					this.ApplyInitialLut();
				}
			}

			public static bool CanCreateFrom(IPresentationImage presentationImage)
			{
				if (IsVoiLutProvider(presentationImage) && IsVoiLutEnabled(presentationImage) && IsGrayScaleImage(presentationImage) && IsImageSopProvider(presentationImage))
				{
					// this preset applies to any IDicomVoiLutsProvider, since in the absence of any presentation or image luts, we always have a min/max algorithm
					return presentationImage is IDicomVoiLutsProvider;
				}
				return false;
			}
		}

		#endregion

		#region Lut Application for Color Images

		private class ColorLutApplicator
		{
			private readonly IPresentationImage _image;

			public ColorLutApplicator(IPresentationImage image)
			{
				_image = image;
			}

			private IVoiLutManager VoiLutManager
			{
				get { return ((IVoiLutProvider) _image).VoiLutManager; }
			}

			public IComposableLut GetInitialLut()
			{
				return new IdentityVoiLinearLut();
			}

			public void ApplyInitialLut()
			{
				this.VoiLutManager.InstallVoiLut(this.GetInitialLut());
			}

			public void ApplyNextLut()
			{
				this.ApplyInitialLut();
			}

			public static bool CanCreateFrom(IPresentationImage presentationImage)
			{
				if (IsVoiLutProvider(presentationImage) && IsVoiLutEnabled(presentationImage) && IsColorImage(presentationImage) && IsImageSopProvider(presentationImage))
				{
					return true;
				}
				return false;
			}
		}

		#endregion

		public AutoPresetVoiLutOperationComponent() {}

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
			return LutApplicator.CanCreateFrom(presentationImage)
			       || ColorLutApplicator.CanCreateFrom(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			// TODO: Later, when we've enabled all the factories, we need to change this functionality so it 
			// is purely 'auto'; no min/max algorithm, as it is currently.

			if (LutApplicator.CanCreateFrom(presentationImage))
				new LutApplicator(presentationImage).ApplyNextLut();
			else if (ColorLutApplicator.CanCreateFrom(presentationImage))
				new ColorLutApplicator(presentationImage).ApplyNextLut();
			else
				throw new InvalidOperationException("The input presentation image is not supported.");
		}

		internal static IComposableLut GetInitialLut(IPresentationImage presentationImage)
		{
			if (LutApplicator.CanCreateFrom(presentationImage))
				return new LutApplicator(presentationImage).GetInitialLut();
			else if (ColorLutApplicator.CanCreateFrom(presentationImage))
				return new ColorLutApplicator(presentationImage).GetInitialLut();
			else
				return null;
		}
	}
}