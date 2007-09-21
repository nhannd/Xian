using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using System.Runtime.InteropServices;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
    public abstract class DicomImageStoreBase
    {
    	protected abstract IDataStoreReader GetIDataStoreReader();
    	protected abstract IDataStoreWriter GetIDataStoreWriter();

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

			attribute = sopInstanceDataset[DicomTags.StudyId];
			study.StudyId = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.StudyInstanceUid];
			study.StudyInstanceUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientId];
			study.PatientId = new PatientId(attribute.ToString() ?? "");

			attribute = sopInstanceDataset[DicomTags.PatientsSex];
			study.PatientsSex = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientsBirthDate];
			study.PatientsBirthDateRaw = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			study.SpecificCharacterSet = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.PatientsName];            
			study.PatientsName = new PersonName(attribute.ToString());
            study.PatientsNameRaw = DicomImplementation.CharacterParser.EncodeAsIsomorphicString(study.PatientsName, sopInstanceDataset.SpecificCharacterSet);

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

			attribute = sopInstanceDataset[DicomTags.SeriesInstanceUid];
			series.SeriesInstanceUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SeriesNumber];
			int intValue;
			if (attribute.TryGetInt32(0, out intValue))
				series.SeriesNumber = intValue;

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			series.SpecificCharacterSet = attribute.ToString();

            return series;
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

			attribute = sopInstanceDataset[DicomTags.SamplesPerPixel];
			if (attribute.TryGetUInt16(0, out uintValue))
				image.SamplesPerPixel = (int)uintValue;

			attribute = sopInstanceDataset[DicomTags.SopClassUid];
			image.SopClassUid = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SopInstanceUid];
			image.SopInstanceUid = attribute.ToString();

			attribute = metaInfo[DicomTags.TransferSyntaxUid];
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

		protected void AddStudyToCache(Study study)
		{
			if (this.ExistingStudyCache.ContainsKey(study.StudyInstanceUid))
				return;

			if (this.NewStudyCache.ContainsKey(study.StudyInstanceUid))
				return;

			IDataStoreReader reader = GetIDataStoreReader();
			IStudy existingStudy = reader.GetStudy(new Uid(study.GetStudyInstanceUid()));
			if (existingStudy != null)
			{
				this.ExistingStudyCache[existingStudy.GetStudyInstanceUid()] = existingStudy;
				foreach(ISeries series in existingStudy.GetSeries())
					this.ExistingSeriesCache[series.GetSeriesInstanceUid()] = series;
			}
			else
			{
				this.NewStudyCache[study.GetStudyInstanceUid()] = study;
			}
		}

    	protected void AddSeriesToCache(Series series, string parentStudyUid)
		{
			if (this.ExistingSeriesCache.ContainsKey(series.SeriesInstanceUid))
				return;

			if (this.NewSeriesCache.ContainsKey(series.SeriesInstanceUid))
				return;

			IStudy study = GetStudyFromCache(parentStudyUid);
			if (study == null)
				throw new ArgumentException("The parent study must be added to the cache first.");

			this.NewSeriesCache[series.GetSeriesInstanceUid()] = series;
			series.SetParentStudy(study);
		}

		protected void AddSopInstanceToCache(SopInstance sop, string parentSeriesUid)
		{
			if (this.ExistingSeriesCache.ContainsKey(parentSeriesUid))
			{
				ISeries series = this.ExistingSeriesCache[parentSeriesUid];
				ISopInstance existingSop = SingleSessionDataAccessLayer.GetIDataStoreReader().GetSopInstance(new Uid(sop.SopInstanceUid));

				if (existingSop != null)
				{
				}

				NewSopInstancesBelongingToExistingSeries[sop.SopInstanceUid] = sop;
				sop.SetParentSeries(series);
			}
			else if (this.NewSeriesCache.ContainsKey(parentSeriesUid))
			{
				ISeries series = this.NewSeriesCache[parentSeriesUid];
				series.AddSopInstance(sop);
			}
			else
				throw new ArgumentException("The parent series must be added to the cache first.");
		}

		protected virtual void Flush()
		{
			try
			{
				IDataStoreWriter writer = GetIDataStoreWriter();
				foreach (KeyValuePair<string, IStudy> pair in this.NewStudyCache)
				{
					writer.StoreStudy(pair.Value);
				}

				foreach (KeyValuePair<string, ISeries> pair in this.NewSeriesCache)
				{
					writer.StoreSeries(pair.Value);
				}

				writer.StoreSopInstances(this.NewSopInstancesBelongingToExistingSeries.Values);
			}
			finally
			{
				ClearCache();
			}
		}

		private IStudy GetStudyFromCache(string studyInstanceUid)
		{
			if (this.ExistingStudyCache.ContainsKey(studyInstanceUid))
				return this.ExistingStudyCache[studyInstanceUid];
			if (this.NewStudyCache.ContainsKey(studyInstanceUid))
				return this.NewStudyCache[studyInstanceUid];

			return null;
		}
		
		private Dictionary<string, IStudy> ExistingStudyCache
        {
            get { return _existingStudyCache; }
        }

		private Dictionary<string, ISeries> ExistingSeriesCache
        {
            get { return _existingSeriesCache; }
        }

		private Dictionary<string, ISopInstance> NewSopInstancesBelongingToExistingSeries
    	{
			get { return _newSopInstancesBelongingToExistingSeries; }
    	}

		private Dictionary<string, IStudy> NewStudyCache
    	{
			get { return _newStudyCache; }
    	}

		private Dictionary<string, ISeries> NewSeriesCache
		{
			get { return _newSeriesCache; }
		}
		
		#region Private Members
        private Dictionary<string, IStudy> _existingStudyCache = new Dictionary<string, IStudy>();
        private Dictionary<string, ISeries> _existingSeriesCache = new Dictionary<string, ISeries>();

		private Dictionary<string, ISopInstance> _newSopInstancesBelongingToExistingSeries  = new Dictionary<string, ISopInstance>();

		private Dictionary<string, IStudy> _newStudyCache = new Dictionary<string, IStudy>();
		private Dictionary<string, ISeries> _newSeriesCache = new Dictionary<string, ISeries>();
		#endregion

		protected void ClearCache()
		{
			this.ExistingStudyCache.Clear();
			this.ExistingSeriesCache.Clear();

			this.NewSopInstancesBelongingToExistingSeries.Clear();
			this.NewStudyCache.Clear();
			this.NewSeriesCache.Clear();
		}
	}
}
