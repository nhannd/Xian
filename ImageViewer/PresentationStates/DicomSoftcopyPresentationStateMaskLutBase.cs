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
using DataLut = ClearCanvas.Dicom.Iod.DataLut;

namespace ClearCanvas.ImageViewer.PresentationStates {
	[Cloneable]
	internal abstract class DicomSoftcopyPresentationStateMaskLutBase<T> : DicomSoftcopyPresentationStateBase<T> where T : IPresentationImage, IImageSopProvider, ISpatialTransformProvider, IImageGraphicProvider, IOverlayGraphicsProvider, IVoiLutProvider {
		protected DicomSoftcopyPresentationStateMaskLutBase(SopClass psSopClass) : base(psSopClass) {}

		protected DicomSoftcopyPresentationStateMaskLutBase(SopClass psSopClass, DicomFile dicomFile) : base(psSopClass, dicomFile) {}

		protected DicomSoftcopyPresentationStateMaskLutBase(SopClass psSopClass, DicomAttributeCollection dataSource) : base(psSopClass, dataSource) {}

		protected DicomSoftcopyPresentationStateMaskLutBase(DicomSoftcopyPresentationStateMaskLutBase<T> source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		#region Serialization Support

		protected void SerializePresentationStateMask(PresentationStateMaskModuleIod presentationStateMaskModule) {
			presentationStateMaskModule.InitializeAttributes();
		}

		protected void SerializeMask(MaskModuleIod maskModule) {
			// TODO : fix this dummy implementation
		}

		protected void SerializeModalityLut(ModalityLutModuleIod modalityLutModule) {
			// TODO : fix this dummy implementation
		}

		protected void SerializeSoftcopyVoiLut(SoftcopyVoiLutModuleIod softcopyVoiLutModule, IList<T> imagesByList) {
			// TODO : finish implementation
			List<SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem> voiLutSequenceItems = new List<SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem>();

			foreach (T image in imagesByList) {
				SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem sequenceItem = new SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem();
				sequenceItem.InitializeAttributes();
				sequenceItem.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] { CreateImageSopInstanceReference(image.Frame) };

				IComposableLut lut = image.VoiLutManager.GetLut();
				if (lut is IVoiLutLinear) {
					IVoiLutLinear voiLut = (IVoiLutLinear)lut;
					sequenceItem.WindowWidth = new double[] { voiLut.WindowWidth };
					sequenceItem.WindowCenter = new double[] { voiLut.WindowCenter };
					sequenceItem.WindowCenterWidthExplanation = null;
					sequenceItem.VoiLutFunction = VoiLutFunction.Linear; // we don't support sigmoid
				} else if (lut is IDataLut) {
					IDataLut voiLut = (IDataLut)lut;
					sequenceItem.VoiLutSequence = new VoiLutSequenceItem[] { VoiLutSequenceDataLutSerializer.Serialize(voiLut) };
				} else {
					// lut type is not supported (yet)
					Platform.Log(LogLevel.Debug, "A LUT of type {0} was not serialized to presentation state.");
					continue;
				}

				voiLutSequenceItems.Add(sequenceItem);
			}

			if (voiLutSequenceItems.Count > 0)
				softcopyVoiLutModule.SoftcopyVoiLutSequence = voiLutSequenceItems.ToArray();
		}

		#endregion

		#region Deserialization Support

		protected void DeserializeModalityLut(ModalityLutModuleIod module, T image) {
			// TODO : fix this dummy implementation
		}

		protected void DeserializeSoftcopyVoiLut(SoftcopyVoiLutModuleIod module, T image) {
			// TODO : support multiple views
			SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem[] lutSequences = module.SoftcopyVoiLutSequence;
			if (lutSequences == null)
				return;

			foreach (SoftcopyVoiLutModuleIod.SoftcopyVoiLutSequenceItem lutSequence in lutSequences) {
				ImageSopInstanceReferenceDictionary dictionary = new ImageSopInstanceReferenceDictionary(lutSequence.ReferencedImageSequence, true);
				if (dictionary.ReferencesFrame(image.ImageSop.SopInstanceUID, image.Frame.FrameNumber)) {
					IComposableLut lut = null;

					if (lutSequence.CountWindows > 0) {
						if (lutSequence.VoiLutFunction == VoiLutFunction.Sigmoid) {
							Platform.Log(LogLevel.Warn, "Sigmoid LUTs are not currently supported.");
						} else // default is linear
						{
							double[] window = lutSequence.WindowWidth;
							double[] level = lutSequence.WindowCenter;
							string[] explanation = lutSequence.WindowCenterWidthExplanation;
							string explanationstr = "From PR";
							if (explanation != null && explanation.Length > 0)
								explanationstr = explanation[0];
							lut = new WindowedSoftcopyVoiLut(window[0], level[0], explanationstr);
						}
					}

					if (lutSequence.CountDataLuts > 0) {
						foreach (VoiLutSequenceItem item in lutSequence.VoiLutSequence) {
							lut = VoiLutSequenceDataLutSerializer.Deserialize(item);
						}
					}

					if (lut != null)
						image.VoiLutManager.InstallLut(lut);

					break; // only apply the first sequence applicable to the presentation image
				}
			}
		}

		#endregion

		#region LUT Classes

		private static class VoiLutSequenceDataLutSerializer {
			private static int GetPixelRepresentation(IDicomAttributeProvider dicomAttributeProvider) {
				DicomAttribute pixelRepresentationAttribute = dicomAttributeProvider[DicomTags.PixelRepresentation];
				if (pixelRepresentationAttribute == null)
					return 0;
				else
					return pixelRepresentationAttribute.GetInt32(0, 0);
			}

			public static IDataLut Deserialize(VoiLutSequenceItem item) {
				DataLut dl = DataLut.Create(item.DicomSequenceItem, GetPixelRepresentation(item.DicomAttributeProvider) != 0, false);
				return new SimpleDataLut(dl.FirstMappedPixelValue, dl.Data, dl.MinOutputValue, dl.MaxOutputValue, Guid.NewGuid().ToString(), string.Format("From PR ({0})", dl.Explanation));
			}

			public static VoiLutSequenceItem Serialize(IDataLut voiLut) {
				int[] descriptor = new int[3];
				descriptor[0] = voiLut.Data.Length;
				if (descriptor[0] == 65536)
					descriptor[0] = 0;
				descriptor[1] = voiLut.MinInputValue;
				descriptor[2] = 16;

				ushort[] data = ToUint16Array(voiLut.Data);

				VoiLutSequenceItem lutseq = new VoiLutSequenceItem();
				lutseq.LutDescriptor = descriptor;
				lutseq.LutData = data;
				lutseq.LutExplanation = null;
				return lutseq;
			}

			private static unsafe ushort[] ToUint16Array(int[] lutData) {
				int inputLength = lutData.Length;
				ushort[] buffer = new ushort[inputLength];
				fixed (ushort* output = buffer) {
					fixed (int* input = lutData) {
						for (int n = 0; n < inputLength; n++) {
							output[n] = (ushort)(input[n] & 0x0000FFFF);
						}
					}
				}
				return buffer;
			}
		}

		[Cloneable(true)]
		private class WindowedSoftcopyVoiLut : CalculatedVoiLutLinear {
			private readonly double _windowWidth;
			private readonly double _windowCenter;
			private readonly string _explanation;

			private WindowedSoftcopyVoiLut() { }

			public WindowedSoftcopyVoiLut(double windowWidth, double windowCenter, string explanation) {
				_windowWidth = windowWidth;
				_windowCenter = windowCenter;
				_explanation = explanation;
			}

			public override double WindowWidth {
				get { return _windowWidth; }
			}

			public override double WindowCenter {
				get { return _windowCenter; }
			}

			public override string GetDescription() {
				return string.Format(SR.FormatDescriptionPresentationStateLinearLut, _windowWidth, _windowCenter, _explanation);
			}
		}

		#endregion
	}
}
