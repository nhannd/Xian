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

            StringBuilder studyInstanceUid = new StringBuilder(1024);
            OFCondition cond = sopInstanceDataset.findAndGetOFString(Dcm.StudyInstanceUID, studyInstanceUid);
            if (cond.good())
            {
                Study study = null;
                if (this.StudyCache.ContainsKey(studyInstanceUid.ToString()))
                    study = this.StudyCache[studyInstanceUid.ToString()];

                if (null == study)
                {
                    // we haven't come across this study yet, so let's see if it already exists in the DataStore
                    study = dataStoreReader.GetStudy(new Uid(studyInstanceUid.ToString())) as Study;
                    if (null == study)
                    {
                        // the study doesn't exist in the data store either
                        study = CreateNewStudy(metaInfo, sopInstanceDataset);
                        this.StudyCache.Add(studyInstanceUid.ToString(), study);
                        return study;
                    }
                    else
                    {
                        // the study was found in the data store
                        this.StudyCache.Add(studyInstanceUid.ToString(), study);

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
            StringBuilder seriesInstanceUid = new StringBuilder(1024);
            OFCondition cond = sopInstanceDataset.findAndGetOFString(Dcm.SeriesInstanceUID, seriesInstanceUid);
            if (cond.good())
            {
                Series series = null;
                if (this.SeriesCache.ContainsKey(seriesInstanceUid.ToString()))
                    series = this.SeriesCache[seriesInstanceUid.ToString()];

                if (null == series)
                {
                    // if the series was in the datastore, it would also exist
                    // in the cache at this point, because the study would have
                    // been loaded, and Series is not lazy-initialized.
                    // Therefore, this series is not in the datastore either.
                    series = CreateNewSeries(metaInfo, sopInstanceDataset);
                    this.SeriesCache.Add(seriesInstanceUid.ToString(), series);
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
            StringBuilder stringValue = new StringBuilder(1024);
            Study study = new Study();

            OFCondition cond = sopInstanceDataset.findAndGetOFString(Dcm.AccessionNumber, stringValue);
            if (cond.good())
                study.AccessionNumber = stringValue.ToString();

            //
            // TODO: can't access sequences yet. We will have to get
            // ProcedureCodeSequence.CodeValue and ProcedureCodeSequence.SchemeDesignator
            //

            cond = sopInstanceDataset.findAndGetOFString(Dcm.StudyDate, stringValue);
            if (cond.good())
                study.StudyDate = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.StudyTime, stringValue);
            if (cond.good())
                study.StudyTime = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.StudyDescription, stringValue);
            if (cond.good())
                study.StudyDescription = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.StudyID, stringValue);
            if (cond.good())
                study.StudyId = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.StudyInstanceUID, stringValue);
            if (cond.good())
                study.StudyInstanceUid = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientId, stringValue);
            if (cond.good())
                study.PatientId = new PatientId(stringValue.ToString());

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientsSex, stringValue);
            if (cond.good())
                study.PatientsSex = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientsBirthDate, stringValue);
            if (cond.good())
                study.PatientsBirthDate = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.SpecificCharacterSet, stringValue);
            if (cond.good())
                study.SpecificCharacterSet = stringValue.ToString();

            //cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientsName, stringValue);

            byte[] rawBytes = new byte[1025];
            int length = 0;
            cond = OffisDcm.findAndGetRawStringFromItem(sopInstanceDataset, Dcm.PatientsName, rawBytes, ref length, false);

            if (cond.good())
            {
                study.PatientsNameRaw = System.Text.Encoding.GetEncoding("Windows-1252").GetString(rawBytes, 0, length);

				if (null == study.SpecificCharacterSet || study.SpecificCharacterSet == String.Empty)
					study.PatientsName = new PersonName(study.PatientsNameRaw);
				else
					study.PatientsName = new PersonName(SpecificCharacterSetParser.Parse(study.SpecificCharacterSet, study.PatientsNameRaw));
            }

            study.StoreTime = Platform.Time;

            return study;
        }

        protected Series CreateNewSeries(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            StringBuilder stringValue = new StringBuilder(1024);
            Series series = new Series();

            OFCondition cond = sopInstanceDataset.findAndGetOFString(Dcm.Laterality, stringValue);
            if (cond.good())
                series.Laterality = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.Modality, stringValue);
            if (cond.good())
                series.Modality = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.SeriesDescription, stringValue);
            if (cond.good())
                series.SeriesDescription = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.SeriesInstanceUID, stringValue);
            if (cond.good())
                series.SeriesInstanceUid = stringValue.ToString();

            int integerValue;
            cond = sopInstanceDataset.findAndGetSint32(Dcm.SeriesNumber, out integerValue);
            if (cond.good())
                series.SeriesNumber = integerValue;

            cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.SpecificCharacterSet, stringValue);
            if (cond.good())
                series.SpecificCharacterSet = stringValue.ToString();

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
            StringBuilder stringValue = new StringBuilder(1024);

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

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PhotometricInterpretation, stringValue);
            if (cond.good())
                image.PhotometricInterpretation = PhotometricInterpretationHelper.FromString(stringValue.ToString());

            cond = sopInstanceDataset.findAndGetUint16(Dcm.PixelRepresentation, out ushortValue);
            if (cond.good())
                image.PixelRepresentation = Convert.ToInt32(ushortValue);

            // TODO: this way of getting the string representations of these arrays
            // and then converting the types to the appropriate double values in an
            // array is inefficient. We should create some more sophisticated typemaps
            // for SWIG that will allow us to pass in a managed double array, and have
            // the values returned there.
            cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.PixelSpacing, stringValue);
            if (cond.good())
            {
                // parse out the string of two values
                string[] components = stringValue.ToString().Split('\\');
                image.PixelSpacing = new PixelSpacing(Convert.ToDouble(components[0]), Convert.ToDouble(components[1]));
            }

            cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.PixelAspectRatio, stringValue);
            if (cond.good())
            {
                // parse out the string of two values
                string[] components = stringValue.ToString().Split('\\');
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

            cond = sopInstanceDataset.findAndGetOFString(Dcm.SOPClassUID, stringValue);
            if (cond.good())
                image.SopClassUid = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.SOPInstanceUID, stringValue);
            if (cond.good())
                image.SopInstanceUid = stringValue.ToString();

            cond = metaInfo.findAndGetOFString(Dcm.TransferSyntaxUID, stringValue);
            if (cond.good())
                image.TransferSyntaxUid = stringValue.ToString();


            // TODO: this way of getting the string representations of these arrays
            // and then converting the types to the appropriate double values in an
            // array is inefficient. We should create some more sophisticated typemaps
            // for SWIG that will allow us to pass in a managed double array, and have
            // the values returned there.
            cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.WindowWidth, stringValue);
            if (cond.good())
            {
                StringBuilder stringValue2 = new StringBuilder(1024);
                cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.WindowCenter, stringValue2);
                if (cond.good())
                {
                    string[] widthComponents = stringValue.ToString().Split('\\');
                    string[] centerComponents = stringValue2.ToString().Split('\\');

                    for (int i = 0; i < widthComponents.Length; ++i)
                    {
                        image.WindowValues.Add(new Window(Convert.ToDouble(widthComponents[i]), Convert.ToDouble(centerComponents[i])));
                    }
                }
            }

            cond = sopInstanceDataset.findAndGetOFStringArray(Dcm.SpecificCharacterSet, stringValue);
            if (cond.good())
                image.SpecificCharacterSet = stringValue.ToString();

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
