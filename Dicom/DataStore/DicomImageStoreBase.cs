using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.DataStore
{
    public abstract class DicomImageStoreBase
    {
        protected IStudy GetStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, Type dataAccessLayerType)
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

			string studyInstanceUid;
            OFCondition cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.StudyInstanceUID, out studyInstanceUid);
            if (cond.good())
            {
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
            return null;
        }

        protected ISeries GetSeries(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
			string seriesInstanceUid;
			OFCondition cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.SeriesInstanceUID, out seriesInstanceUid);
			if (cond.good())
            {
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
            return null;
        }

        protected Study CreateNewStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            Study study = new Study();
			string value;
			OFCondition cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.AccessionNumber, out value);
			if (cond.good())
				study.AccessionNumber = value;

            //
            // TODO: can't access sequences yet. We will have to get
            // ProcedureCodeSequence.CodeValue and ProcedureCodeSequence.SchemeDesignator
            //

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.StudyDate, out value);
			if (cond.good())
			{
				study.StudyDateRaw = value;
				study.StudyDate = DateParser.Parse(value);
			}

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.StudyTime, out value);
			if (cond.good())
				study.StudyTimeRaw = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.StudyDescription, out value);
			if (cond.good())
				study.StudyDescription = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.StudyID, out value);
			if (cond.good())
				study.StudyId = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.StudyInstanceUID, out value);
			if (cond.good())
				study.StudyInstanceUid = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.PatientId, out value);
            if (cond.good())
                study.PatientId = new PatientId(value);

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.PatientsSex, out value);
			if (cond.good())
				study.PatientsSex = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.PatientsBirthDate, out value);
			if (cond.good())
				study.PatientsBirthDateRaw = value;

			cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.SpecificCharacterSet, out value);
			if (cond.good())
				study.SpecificCharacterSet = value;

            byte[] patientsNameRawBytes;
            cond = DicomHelper.FindAndGetRawStringFromItem(sopInstanceDataset, Dcm.PatientsName, out patientsNameRawBytes);

            if (cond.good())
            {
                // of course we shouldn't be converting this yet again, we should be able to store the raw
                study.PatientsNameRaw = patientsNameRawBytes;
				study.PatientsName = new PersonName(SpecificCharacterSetParser.Parse(study.SpecificCharacterSet, study.PatientsNameRaw));
            }

            study.StoreTime = Platform.Time;

            return study;
        }

        protected Series CreateNewSeries(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            Series series = new Series();

			string value;
			OFCondition cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.Laterality, out value);
			if (cond.good())
				series.Laterality = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.Modality, out value);
			if (cond.good())
				series.Modality = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.SeriesDescription, out value);
            if (cond.good())
                series.SeriesDescription = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.SeriesInstanceUID, out value);
            if (cond.good())
                series.SeriesInstanceUid = value;

            int integerValue;
            cond = sopInstanceDataset.findAndGetSint32(Dcm.SeriesNumber, out integerValue);
            if (cond.good())
                series.SeriesNumber = integerValue;

			cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.SpecificCharacterSet, out value);
            if (cond.good())
                series.SpecificCharacterSet = value;

            return series;
        }

        protected ISopInstance GetSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName)
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

		protected SopInstance CreateNewSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName)
		{
			SopInstance newSop = CreateNewSopInstance(metaInfo, sopInstanceDataset);
			AssignSopInstanceUri(newSop, fileName);
			return newSop;
		}

		protected SopInstance CreateNewSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            ushort ushortValue;
            int integerValue;
            double doubleValue;

            // TODO: we need to generalize this to be able to create the correct
            // type in the SopInstance hierarchy instead of always creating a 
            // ImageSopInstance.
            ImageSopInstance image = new ImageSopInstance();

            OFCondition cond = sopInstanceDataset.findAndGetUint16(Dcm.BitsAllocated, out ushortValue);
            if (cond.good())
                image.BitsAllocated = Convert.ToInt32(ushortValue);

            cond = sopInstanceDataset.findAndGetUint16(Dcm.BitsStored, out ushortValue);
            if (cond.good())
                image.BitsStored = Convert.ToInt32(ushortValue);

            cond = sopInstanceDataset.findAndGetUint16(Dcm.Columns, out ushortValue);
            if (cond.good())
                image.Columns = Convert.ToInt32(ushortValue);

            cond = sopInstanceDataset.findAndGetUint16(Dcm.HighBit, out ushortValue);
            if (cond.good())
                image.HighBit = Convert.ToInt32(ushortValue);

            cond = sopInstanceDataset.findAndGetSint32(Dcm.InstanceNumber, out integerValue);
            if (cond.good())
                image.InstanceNumber = integerValue;

			string value;
			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.PhotometricInterpretation, out value);
            if (cond.good())
                image.PhotometricInterpretation = PhotometricInterpretationHelper.FromString(value);

            cond = sopInstanceDataset.findAndGetUint16(Dcm.PixelRepresentation, out ushortValue);
            if (cond.good())
                image.PixelRepresentation = Convert.ToInt32(ushortValue);

            // TODO: this way of getting the string representations of these arrays
            // and then converting the types to the appropriate double values in an
            // array is inefficient. We should create some more sophisticated typemaps
            // for SWIG that will allow us to pass in a managed double array, and have
            // the values returned there.
			cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.PixelSpacing, out value);
            if (cond.good())
            {
                // parse out the string of two values
                string[] components = value.ToString().Split('\\');
                image.PixelSpacing = new PixelSpacing(Convert.ToDouble(components[0]), Convert.ToDouble(components[1]));
            }

			cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.PixelAspectRatio, out value);
            if (cond.good())
            {
                // parse out the string of two values
                string[] components = value.ToString().Split('\\');
                image.PixelAspectRatio = new PixelAspectRatio(Convert.ToDouble(components[0]), Convert.ToDouble(components[1]));
            }

            cond = sopInstanceDataset.findAndGetUint16(Dcm.PlanarConfiguration, out ushortValue);
            if (cond.good())
                image.PlanarConfiguration = Convert.ToInt32(ushortValue);

            cond = sopInstanceDataset.findAndGetFloat64(Dcm.RescaleIntercept, out doubleValue);
            if (cond.good())
                image.RescaleIntercept = doubleValue;

            cond = sopInstanceDataset.findAndGetFloat64(Dcm.RescaleSlope, out doubleValue);
            if (cond.good())
                image.RescaleSlope = doubleValue;

            cond = sopInstanceDataset.findAndGetUint16(Dcm.Rows, out ushortValue);
            if (cond.good())
                image.Rows = Convert.ToInt32(ushortValue);

            cond = sopInstanceDataset.findAndGetUint16(Dcm.SamplesPerPixel, out ushortValue);
            if (cond.good())
                image.SamplesPerPixel = Convert.ToInt32(ushortValue);

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.SOPClassUID, out value);
            if (cond.good())
                image.SopClassUid = value;

			cond = DicomHelper.TryFindAndGetOFString(sopInstanceDataset, Dcm.SOPInstanceUID, out value);
            if (cond.good())
                image.SopInstanceUid = value;

			cond = DicomHelper.TryFindAndGetOFString(metaInfo, Dcm.TransferSyntaxUID, out value);
            if (cond.good())
                image.TransferSyntaxUid = value;

            // TODO: this way of getting the string representations of these arrays
            // and then converting the types to the appropriate double values in an
            // array is inefficient. We should create some more sophisticated typemaps
            // for SWIG that will allow us to pass in a managed double array, and have
            // the values returned there.
			cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.WindowWidth, out value);
            if (cond.good())
            {
				string value2;
				cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.WindowCenter, out value2);
                if (cond.good())
                {
                    string[] widthComponents = value.Split('\\');
                    string[] centerComponents = value2.Split('\\');

                    for (int i = 0; i < widthComponents.Length; ++i)
                    {
                        image.WindowValues.Add(new Window(Convert.ToDouble(widthComponents[i]), Convert.ToDouble(centerComponents[i])));
                    }
                }
            }

			cond = DicomHelper.TryFindAndGetOFStringArray(sopInstanceDataset, Dcm.SpecificCharacterSet, out value);
            if (cond.good())
                image.SpecificCharacterSet = value;

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
