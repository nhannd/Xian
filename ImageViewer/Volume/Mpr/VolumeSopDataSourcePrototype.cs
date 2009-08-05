using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	partial class Volume
	{
		/// <summary>
		/// The SOP data source prototype object that will be shared between actual SOPs
		/// </summary>
		/// <remarks>
		/// For now, we will have this prototype shared between each SOP (a single slice of any plane)
		/// Ideally, we should have a single multiframe SOP for each plane, and then this prototype is shared between those SOPs
		/// </remarks>
		private class VolumeSopDataSourcePrototype : IDicomAttributeProvider
		{
			private readonly DicomAttributeCollection _collection = new DicomAttributeCollection();

			public DicomAttribute this[DicomTag tag]
			{
				get { return _collection[tag]; }
				set { _collection[tag] = value; }
			}

			public DicomAttribute this[uint tag]
			{
				get { return _collection[tag]; }
				set { _collection[tag] = value; }
			}

			public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
			{
				return _collection.TryGetAttribute(tag, out attribute);
			}

			public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
			{
				return _collection.TryGetAttribute(tag, out attribute);
			}

			public static VolumeSopDataSourcePrototype Create(IDicomAttributeProvider source)
			{
				VolumeSopDataSourcePrototype prototype = new VolumeSopDataSourcePrototype();
				DicomAttributeCollection volumeDataSet = prototype._collection;

				// perform exact copy on the Patient Module
				foreach (uint tag in PatientModuleIod.DefinedTags)
					volumeDataSet[tag] = source[tag].Copy();

				// perform exact copy on the Clinical Trial Subject Module
				foreach (uint tag in ClinicalTrialSubjectModuleIod.DefinedTags)
					volumeDataSet[tag] = source[tag].Copy();

				// perform exact copy on the General Study Module
				foreach (uint tag in GeneralStudyModuleIod.DefinedTags)
					volumeDataSet[tag] = source[tag].Copy();

				// perform exact copy on the Patient Study Module
				foreach (uint tag in PatientStudyModuleIod.DefinedTags)
					volumeDataSet[tag] = source[tag].Copy();

				// perform exact copy on the Clinical Trial Study Module
				foreach (uint tag in ClinicalTrialStudyModuleIod.DefinedTags)
					volumeDataSet[tag] = source[tag].Copy();

				// TODO JY: flesh out these other modules.

				// generate and cache Series Module attributes that are common among all slicings
				volumeDataSet[DicomTags.Modality] = source[DicomTags.Modality].Copy();
				volumeDataSet[DicomTags.SeriesNumber].SetStringValue("0");
				volumeDataSet[DicomTags.SeriesDescription] = source[DicomTags.SeriesDescription].Copy();

				// generate General Equipment Module

				// generate SC Equipment Module
				volumeDataSet[DicomTags.ConversionType].SetStringValue("WSD");

				// generate General Image Module
				volumeDataSet[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY");
				volumeDataSet[DicomTags.PixelSpacing] = source[DicomTags.PixelSpacing].Copy();
				volumeDataSet[DicomTags.FrameOfReferenceUid] = source[DicomTags.FrameOfReferenceUid].Copy();

				// generate Image Pixel Module
				volumeDataSet[DicomTags.SamplesPerPixel] = source[DicomTags.SamplesPerPixel].Copy();
				volumeDataSet[DicomTags.PhotometricInterpretation] = source[DicomTags.PhotometricInterpretation].Copy();
				volumeDataSet[DicomTags.BitsAllocated] = source[DicomTags.BitsAllocated].Copy();
				volumeDataSet[DicomTags.BitsStored] = source[DicomTags.BitsStored].Copy();
				volumeDataSet[DicomTags.HighBit] = source[DicomTags.HighBit].Copy();
				volumeDataSet[DicomTags.PixelRepresentation] = source[DicomTags.PixelRepresentation].Copy();
				volumeDataSet[DicomTags.PixelPaddingValue] = source[DicomTags.PixelPaddingValue];
				volumeDataSet[DicomTags.PixelPaddingRangeLimit] = source[DicomTags.PixelPaddingRangeLimit];
				volumeDataSet[DicomTags.SmallestImagePixelValue] = source[DicomTags.SmallestImagePixelValue];
				volumeDataSet[DicomTags.LargestImagePixelValue] = source[DicomTags.LargestImagePixelValue];
				volumeDataSet[DicomTags.SmallestPixelValueInSeries] = source[DicomTags.SmallestPixelValueInSeries];
				volumeDataSet[DicomTags.LargestPixelValueInSeries] = source[DicomTags.LargestPixelValueInSeries];

				// generate VOI LUT Module
				volumeDataSet[DicomTags.WindowWidth] = source[DicomTags.WindowWidth].Copy();
				volumeDataSet[DicomTags.WindowCenter] = source[DicomTags.WindowCenter].Copy();
				volumeDataSet[DicomTags.RescaleSlope] = source[DicomTags.RescaleSlope].Copy();
				volumeDataSet[DicomTags.RescaleIntercept] = source[DicomTags.RescaleIntercept].Copy();

				// generate SOP Common Module
				volumeDataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);

				return prototype;
			}
		}
	}
}