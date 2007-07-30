using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.DataStore
{
    public abstract class DicomImageStoreBase
    {
		protected IStudy GetStudy(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, Type dataAccessLayerType)
        {
            // determine whether we're going to use a SingleSessionDataAccessLayer or a regular DataAccessLayer
            IDataStoreReader dataStoreReader = null;
            if (dataAccessLayerType == typeof(SingleSessionDataAccessLayer))
            {
                dataStoreReader = SingleSessionDataAccessLayer.GetIDataStoreReader();
            }
            else if (dataAccessLayerType == typeof(DataAccessLayer))
            {
                dataStoreReader = DataAccessLayer.GetIDataStoreReader();
            }
            else
            {
                // TODO: throw a meaningful exception
                throw new System.ArgumentException();
            }

			DicomAttribute attribute = sopInstanceDataset[DicomTags.StudyInstanceUID];
			string studyInstanceUid = attribute.ToString();
			if (String.IsNullOrEmpty(studyInstanceUid))
				return null;

            Study study = null;
            if (this.StudyCache.ContainsKey(studyInstanceUid))
                study = this.StudyCache[studyInstanceUid];

            if (null == study)
            {
                // we haven't come across this study yet, so let's see if it already exists in the DataStore
                study = dataStoreReader.GetStudy(new Uid(studyInstanceUid)) as Study;
                if (null == study)
                {
                    // the study doesn't exist in the data store either
                    study = CreateNewStudy(metaInfo, sopInstanceDataset);
                    this.StudyCache.Add(studyInstanceUid, study);
                    return study;
                }
                else
                {
                    // the study was found in the data store
                    this.StudyCache.Add(studyInstanceUid, study);

                    // since Study-Series is not lazy initialized, all the series
                    // should be loaded. Let's add them to the cache
                    foreach (ISeries series in study.Series)
                    {
                        this.SeriesCache.Add(series.GetSeriesInstanceUid().ToString(), series as Series);
                    }

                    return study;
                }
            }
            else
            {
                // the study was found in the cache
                return study;
            }
        }

		protected ISeries GetSeries(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
        {
			DicomAttribute attribute = sopInstanceDataset[DicomTags.SeriesInstanceUID];
			string seriesInstanceUid = attribute.ToString();
			if (String.IsNullOrEmpty(seriesInstanceUid))
				return null;

			Series series = null;
            if (this.SeriesCache.ContainsKey(seriesInstanceUid))
                series = this.SeriesCache[seriesInstanceUid];

            if (null == series)
            {
                // if the series was in the datastore, it would also exist
                // in the cache at this point, because the study would have
                // been loaded, and Series is not lazy-initialized.
                // Therefore, this series is not in the datastore either.
                series = CreateNewSeries(metaInfo, sopInstanceDataset);
                this.SeriesCache.Add(seriesInstanceUid, series);
                return series;
            }
            else
            {
                // the series was found in the cache
                return series;
            }
        }

		protected Study CreateNewStudy(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
        {
            Study study = new Study();

			DicomAttribute attribute = sopInstanceDataset[DicomTags.AccessionNumber];
			study.AccessionNumber = attribute.ToString();

            //
            // TODO: can't access sequences yet. We will have to get
            // ProcedureCodeSequence.CodeValue and ProcedureCodeSequence.SchemeDesignator
            //

			attribute = sopInstanceDataset[DicomTags.StudyDate];
			study.StudyDateRaw = attribute.ToString();
			study.StudyDate = DateParser.Parse(study.StudyDateRaw);

			attribute = sopInstanceDataset[DicomTags.StudyTime];
			study.StudyTimeRaw = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyDescription];
			study.StudyDescription = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyID];
			study.StudyId = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyInstanceUID];
			study.StudyInstanceUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientID];
			study.PatientId = new PatientId(attribute.ToString() ?? "");

			attribute = sopInstanceDataset[DicomTags.PatientsSex];
			study.PatientsSex = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientsBirthDate];
			study.PatientsBirthDateRaw = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			study.SpecificCharacterSet = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientsName];
			study.PatientsName = new PersonName(attribute.ToString());

            study.StoreTime = Platform.Time;

            return study;
        }

		protected Series CreateNewSeries(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
        {
            Series series = new Series();

			DicomAttribute attribute = sopInstanceDataset[DicomTags.Laterality];
			series.Laterality = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.Modality];
			series.Modality = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SeriesDescription];
			series.SeriesDescription = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SeriesInstanceUID];
			series.SeriesInstanceUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SeriesNumber];
			int intValue;
			if (attribute.TryGetInt32(0, out intValue))
				series.SeriesNumber = intValue;

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			series.SpecificCharacterSet = attribute.ToString();

            return series;
        }

		protected ISopInstance GetSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, string fileName)
        {
            return CreateNewSopInstance(metaInfo, sopInstanceDataset, fileName);
        }

		protected void AssignSopInstanceUri(SopInstance sopInstance, string fileName)
		{
			if (!System.IO.Path.IsPathRooted(fileName))
				fileName = System.IO.Path.GetFullPath(fileName);

			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Scheme = "file";
			uriBuilder.Path = fileName;
			sopInstance.LocationUri = new DicomUri(uriBuilder.Uri);
		}

		protected SopInstance CreateNewSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, string fileName)
		{
			SopInstance newSop = CreateNewSopInstance(metaInfo, sopInstanceDataset);
			AssignSopInstanceUri(newSop, fileName);
			return newSop;
		}

		protected SopInstance CreateNewSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
        {
            // TODO: we need to generalize this to be able to create the correct
            // type in the SopInstance hierarchy instead of always creating a 
            // ImageSopInstance.
            ImageSopInstance image = new ImageSopInstance();

			UInt16 uintValue;
			DicomAttribute attribute = sopInstanceDataset[DicomTags.BitsAllocated];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.BitsAllocated = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.BitsStored];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.BitsStored = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.HighBit];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.HighBit = (int)uintValue;

			int intValue;
			attribute = sopInstanceDataset[DicomTags.InstanceNumber];
			if (attribute.TryGetInt32(0, out intValue))
				image.InstanceNumber = intValue;

			attribute = sopInstanceDataset[DicomTags.PhotometricInterpretation];
			image.PhotometricInterpretation = PhotometricInterpretationHelper.FromString(attribute.ToString());

			attribute = sopInstanceDataset[DicomTags.PixelRepresentation];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.PixelRepresentation = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.PixelSpacing];
			if (attribute.Count == 2)
			{
				double doubleValue1, doubleValue2;
				if (attribute.TryGetFloat64(0, out doubleValue1) && attribute.TryGetFloat64(1, out doubleValue2))
					image.PixelSpacing = new PixelSpacing(doubleValue1, doubleValue2);
			}

			attribute = sopInstanceDataset[DicomTags.PixelAspectRatio];
			if (attribute.Count == 2)
			{
				double doubleValue1, doubleValue2;
				if (attribute.TryGetFloat64(0, out doubleValue1) && attribute.TryGetFloat64(1, out doubleValue2))
					image.PixelAspectRatio = new PixelAspectRatio(doubleValue1, doubleValue2);
			}

			attribute = sopInstanceDataset[DicomTags.PlanarConfiguration];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.PlanarConfiguration = (int)uintValue;

			double doubleValue;
			attribute = sopInstanceDataset[DicomTags.RescaleIntercept];
			if (attribute.TryGetFloat64(0, out doubleValue))
				image.RescaleIntercept = doubleValue;

			attribute = sopInstanceDataset[DicomTags.RescaleSlope];
			if (attribute.TryGetFloat64(0, out doubleValue))
				image.RescaleSlope = doubleValue;

			attribute = sopInstanceDataset[DicomTags.Rows];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.Rows = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.Columns];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.Columns = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.SamplesperPixel];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.SamplesPerPixel = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.SOPClassUID];
			image.SopClassUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SOPInstanceUID];
			image.SopInstanceUid = attribute.ToString();

			attribute = metaInfo[DicomTags.TransferSyntaxUID];
			image.TransferSyntaxUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.WindowWidth];
			if (attribute.Count > 0 && !attribute.IsNull && !attribute.IsEmpty)
			{
				DicomAttribute attribute2 = sopInstanceDataset[DicomTags.WindowCenter];
				if (attribute.Count == attribute2.Count && !attribute2.IsNull && !attribute2.IsEmpty)
				{
					for (int i = 0; i < attribute.Count; ++i)
					{
						double doubleValue1, doubleValue2;
						if (attribute.TryGetFloat64(i, out doubleValue1) && attribute2.TryGetFloat64(i, out doubleValue2))
							image.WindowValues.Add(new Window(doubleValue1, doubleValue2));
					}
                }
            }

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			image.SpecificCharacterSet = attribute.ToString();

            return image;
        }

        protected Dictionary<string, Study> StudyCache
        {
            get { return _studyCache; }
        }

		protected Dictionary<string, Series> SeriesCache
        {
            get { return _seriesCache; }
        }


        #region Private Members
        private Dictionary<string, Study> _studyCache = new Dictionary<string, Study>();
        private Dictionary<string, Series> _seriesCache = new Dictionary<string, Series>();
        #endregion
    }
}
