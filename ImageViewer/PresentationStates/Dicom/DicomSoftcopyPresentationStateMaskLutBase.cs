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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.VoiLut;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using DataLutIod=ClearCanvas.Dicom.Iod.DataLut;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	[Cloneable]
	internal abstract class DicomSoftcopyPresentationStateMaskLutBase<T> : DicomSoftcopyPresentationStateBase<T> where T : IDicomPresentationImage, IVoiLutProvider, IDicomVoiLutsProvider
	{
		protected DicomSoftcopyPresentationStateMaskLutBase(SopClass psSopClass) : base(psSopClass) {}

		protected DicomSoftcopyPresentationStateMaskLutBase(SopClass psSopClass, DicomFile dicomFile) : base(psSopClass, dicomFile) {}

		protected DicomSoftcopyPresentationStateMaskLutBase(SopClass psSopClass, DicomAttributeCollection dataSource) : base(psSopClass, dataSource) {}

		protected DicomSoftcopyPresentationStateMaskLutBase(DicomSoftcopyPresentationStateMaskLutBase<T> source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		#region Serialization Support

		protected void SerializePresentationStateMask(PresentationStateMaskModuleIod module, DicomPresentationImageCollection<T> images)
		{
			// NOTE: Not supported
			module.InitializeAttributes();
		}

		protected void SerializeMask(MaskModuleIod module, DicomPresentationImageCollection<T> images)
		{
			// NOTE: Not supported
		}

		protected void SerializeModalityLut(ModalityLutModuleIod module, DicomPresentationImageCollection<T> images) { }

		protected void SerializeSoftcopyVoiLut(SoftcopyVoiLutModuleIod module, DicomPresentationImageCollection<T> images)
		{
			List<SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem> voiLutSequenceItems = new List<SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem>();

			foreach (T image in images)
			{
				if (!image.VoiLutManager.Enabled)
					continue;

				SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem sequenceItem = new SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem();
				sequenceItem.InitializeAttributes();
				sequenceItem.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};

				IComposableLut lut = image.VoiLutManager.GetLut();
				if (lut is IDataLut)
				{
					IDataLut voiLut = (IDataLut) lut;
					sequenceItem.VoiLutSequence = new VoiLutSequenceItem[] {SerializeDataLut(voiLut)};
				}
				else if (lut is IVoiLutLinear)
				{
					IVoiLutLinear voiLut = (IVoiLutLinear) lut;
					sequenceItem.WindowWidth = new double[] {voiLut.WindowWidth};
					sequenceItem.WindowCenter = new double[] {voiLut.WindowCenter};
					sequenceItem.WindowCenterWidthExplanation = null;
					sequenceItem.VoiLutFunction = VoiLutFunction.Linear; // we don't support sigmoid
				}
				else
				{
					// should never happen - all VOI LUT object should implement either interface
					continue;
				}

				voiLutSequenceItems.Add(sequenceItem);
			}

			if (voiLutSequenceItems.Count > 0)
				module.SoftcopyVoiLutSequence = voiLutSequenceItems.ToArray();
		}

		#endregion

		#region Deserialization Support

		protected void DeserializePresentationStateMask(PresentationStateMaskModuleIod module, T image)
		{
			// NOTE: Not supported
		}

		protected void DeserializeMask(MaskModuleIod module, T image)
		{
			// NOTE: Not supported
		}

		protected void DeserializeModalityLut(ModalityLutModuleIod module, T image) {}

		protected void DeserializeSoftcopyVoiLut(SoftcopyVoiLutModuleIod module, T image)
		{
			SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem[] lutSequences = module.SoftcopyVoiLutSequence;
			if (lutSequences == null)
				return;

			DicomVoiLuts voiLuts = (DicomVoiLuts) image.DicomVoiLuts;
			voiLuts.ReinitializePresentationLuts(this.PresentationSopInstanceUid);

			foreach (SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem lutSequence in lutSequences)
			{
				ImageSopInstanceReferenceDictionary dictionary = new ImageSopInstanceReferenceDictionary(lutSequence.ReferencedImageSequence, true);
				if (dictionary.ReferencesFrame(image.ImageSop.SopInstanceUID, image.Frame.FrameNumber))
				{
					if (lutSequence.CountWindows > 0)
					{
						double[] widths = lutSequence.WindowWidth;
						double[] centers = lutSequence.WindowCenter;
						int countWindows = Math.Min(widths.Length, centers.Length);
						string[] explanation = lutSequence.WindowCenterWidthExplanation;
						if (explanation == null || explanation.Length < countWindows)
							explanation = new string[countWindows];

						if (lutSequence.VoiLutFunction == VoiLutFunction.Sigmoid)
						{
							Platform.Log(LogLevel.Warn, "Sigmoid LUTs are not currently supported.");
						}
						else // default is linear
						{
							for (int n = 0; n < countWindows; n++)
							{
								voiLuts.AddPresentationLinearLut(widths[n], centers[n], explanation[n] ?? SR.LabelPresentationStateLut);
							}
						}
					}

					if (lutSequence.CountDataLuts > 0)
					{
						foreach (VoiLutSequenceItem item in lutSequence.VoiLutSequence)
						{
							DataLutIod dl = DataLutIod.Create(item.DicomSequenceItem, item.DicomAttributeProvider[DicomTags.LutDescriptor] is DicomAttributeSS, false);
							voiLuts.AddPresentationDataLut(new VoiDataLut(dl.FirstMappedPixelValue, dl.BitsPerEntry, dl.Data, dl.Explanation));
						}
					}
				}
			}
		}

		#endregion

		#region VOI LUT Support Members

		private static VoiLutSequenceItem SerializeDataLut(IDataLut voiLut)
		{
			int inputLength = voiLut.Data.Length;
			int[] descriptor = new int[3];
			descriptor[0] = inputLength;
			if (descriptor[0] == 65536)
				descriptor[0] = 0;
			descriptor[1] = voiLut.FirstMappedPixelValue;
			descriptor[2] = 16;

			ushort[] data = new ushort[inputLength];
			unsafe
			{
				fixed (ushort* output = data)
				{
					fixed (int* input = voiLut.Data)
					{
						for (int n = 0; n < inputLength; n++)
							output[n] = (ushort) (input[n] & 0x0000FFFF);
					}
				}
			}

			VoiLutSequenceItem lutseq = new VoiLutSequenceItem();
			lutseq.LutDescriptor = descriptor;
			lutseq.LutData = data;
			lutseq.LutExplanation = null;
			return lutseq;
		}

		[Cloneable(true)]
		private class LinearSoftcopyVoiLut : CalculatedVoiLutLinear
		{
			private readonly double _windowWidth;
			private readonly double _windowCenter;
			private readonly string _explanation;

			protected LinearSoftcopyVoiLut() {}

			public LinearSoftcopyVoiLut(double windowWidth, double windowCenter, string explanation)
			{
				_windowWidth = windowWidth;
				_windowCenter = windowCenter;
				_explanation = explanation;
			}

			public override double WindowWidth
			{
				get { return _windowWidth; }
			}

			public override double WindowCenter
			{
				get { return _windowCenter; }
			}

			public override string GetDescription()
			{
				return string.Format(SR.FormatDescriptionPresentationStateLinearLut, _windowWidth, _windowCenter, _explanation);
			}
		}

		#endregion
	}
}