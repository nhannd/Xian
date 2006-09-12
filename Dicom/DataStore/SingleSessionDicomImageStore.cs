using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    /// <summary>
    /// Assumes that the objects have non-null and valid UIDs.
    /// </summary>
    public class SingleSessionDicomImageStore : IDicomPersistentStore
    {
        #region IDicomPersistentStore Members

        public void InsertSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName)
        {
            ISopInstance newSop = GetSopInstance(metaInfo, sopInstanceDataset, fileName);
            IStudy study = GetStudy(metaInfo, sopInstanceDataset);
            ISeries series = GetSeries(metaInfo, sopInstanceDataset);

            series.AddSopInstance(newSop);
            study.AddSeries(series);
        }

        public void Flush()
        {
            foreach (KeyValuePair<string, Study> pair in this.StudyCache)
            {
                SingleSessionDataAccessLayer.GetIDataStoreWriter().StoreStudy(pair.Value);
            }
            
            this.SeriesCache.Clear();
            this.StudyCache.Clear();
            
            // TODO:
            // This is a hack. For some reason, when we do a whole succession of Stores,
            // which involves SaveOrUpdate(), Commit(), Flush(), 
            // the next time we try to do a query (on the next image rebuild), the Session
            // seems closed, even though its state declares that it is not closed.
            // The hack is to close the current session after flushing occurs, and then
            // the SingleSessionDataAccessLayer will detect closed sessions, and open a new one
            // when that occurs.
            // Addendum:
            // The hack employed worked, except that subsequent queries on objects that
            // did not exist in the database, on UIDs, would take very very very long.
            // A workaround to this, is to clear the session of domain objects explicitly
            // before closing the session.
            SingleSessionDataAccessLayer.ClearCurrentSession();
            SingleSessionDataAccessLayer.CloseCurrentSession();
        }

        public int GetCachedStudiesCount()
        {
            return this.StudyCache.Count;
        }

        #endregion

        #region Handcoded Members
        private IStudy GetStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
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
                    study = SingleSessionDataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyInstanceUid.ToString())) as Study;
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

        private ISeries GetSeries(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
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
                    // we haven't come across this study yet, so let's see if it already exists in the DataStore
                    series = SingleSessionDataAccessLayer.GetIDataStoreReader().GetSeries(new Uid(seriesInstanceUid.ToString())) as Series;
                    if (null == series)
                    {
                        // the study doesn't exist in the data store either
                        series = CreateNewSeries(metaInfo, sopInstanceDataset);
                        this.SeriesCache.Add(seriesInstanceUid.ToString(), series);
                        return series;
                    }
                    else
                    {
                        // the study was found in the data store
                        this.SeriesCache.Add(seriesInstanceUid.ToString(), series);
                        return series;
                    }
                }
                else
                {
                    // the study was found in the cache
                    return series;
                }
            }
            return null;
        }

        private Study CreateNewStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            StringBuilder stringValue = new StringBuilder(1024);
            Study study = new Study();

            OFCondition cond = sopInstanceDataset.findAndGetOFString(Dcm.AccessionNumber, stringValue);
            if (cond.good())
                study.AccessionNumber = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.AdmittingDiagnosesDescription, stringValue);
            if (cond.good())
                study.AdmittingDiagnosesDescription = stringValue.ToString();

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

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientsName, stringValue);
            if (cond.good())
                study.PatientsName = new PatientsName(stringValue.ToString());

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientsSex, stringValue);
            if (cond.good())
                study.PatientsSex = stringValue.ToString();

            cond = sopInstanceDataset.findAndGetOFString(Dcm.PatientsBirthDate, stringValue);
            if (cond.good())
                study.PatientsBirthDate = stringValue.ToString();

            return study;
        }

        private Series CreateNewSeries(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
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

            return series;
        }

        private ISopInstance GetSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName)
        {
            return CreateNewSopInstance(metaInfo, sopInstanceDataset, fileName);
        }

        private SopInstance CreateNewSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName)
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
            {
                PhotometricInterpretation pi;
                if (stringValue.ToString() == "ARGB")
                    pi = PhotometricInterpretation.Argb;
                else if (stringValue.ToString() == "CMYK")
                    pi = PhotometricInterpretation.Cmyk;
                else if (stringValue.ToString() == "HSV")
                    pi = PhotometricInterpretation.Hsv;
                else if (stringValue.ToString() == "MONOCHROME1")
                    pi = PhotometricInterpretation.Monochrome1;
                else if (stringValue.ToString() == "MONOCHROME2")
                    pi = PhotometricInterpretation.Monochrome2;
                else if (stringValue.ToString() == "PALETTE_COLOR")
                    pi = PhotometricInterpretation.PaletteColor;
                else if (stringValue.ToString() == "RGB")
                    pi = PhotometricInterpretation.Rgb;
                else if (stringValue.ToString() == "YBR_FULL")
                    pi = PhotometricInterpretation.YbrFull;
                else if (stringValue.ToString() == "YBR_FULL_422")
                    pi = PhotometricInterpretation.YbrFull422;
                else if (stringValue.ToString() == "YBR_ICT")
                    pi = PhotometricInterpretation.YbrIct;
                else if (stringValue.ToString() == "YBR_PARTIAL_420")
                    pi = PhotometricInterpretation.YbrPartial420;
                else if (stringValue.ToString() == "YBR_PARTIAL_422")
                    pi = PhotometricInterpretation.YbrPartial422;
                else if (stringValue.ToString() == "YBR_RCT")
                    pi = PhotometricInterpretation.YbrRct;
                else
                    pi = PhotometricInterpretation.Monochrome1;

                image.PhotometricInterpretation = pi;
            }

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
                    string[] centerComponents = stringValue.ToString().Split('\\');

                    for (int i = 0; i < widthComponents.Length; ++i)
                    {
                        image.WindowValues.Add(new Window(Convert.ToDouble(widthComponents[i]), Convert.ToDouble(centerComponents[i])));
                    }
                }
            }

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "file";
            uriBuilder.Path = fileName;
            image.LocationUri = new DicomUri(uriBuilder.Uri);

            return image;
        }

        public Dictionary<string,Study> StudyCache
        {
            get { return _studyCache; }
        }

        public Dictionary<string, Series> SeriesCache
        {
            get { return _seriesCache; }
        }
	

        #region Private Members
        private Dictionary<string, Study> _studyCache = new Dictionary<string,Study>();
        private Dictionary<string, Series> _seriesCache = new Dictionary<string, Series>();
        #endregion
        #endregion

    }
}
