#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		internal sealed class DicomPersistentStore : IDicomPersistentStore
		{
			private readonly Dictionary<string, Study> _existingStudyCache = new Dictionary<string, Study>();
			private readonly Dictionary<string, Series> _existingSeriesCache = new Dictionary<string, Series>();

			private readonly Dictionary<string, Study> _studiesToUpdate = new Dictionary<string, Study>();
			private readonly Dictionary<string, Series> _seriesToUpdate = new Dictionary<string, Series>();
			private readonly Dictionary<string, SopInstance> _sopInstancesToUpdate = new Dictionary<string, SopInstance>();

			private IDataStoreReader _dataStoreReader;

			public DicomPersistentStore()
			{
			}

			~DicomPersistentStore()
			{
				try
				{
					DisposeReader();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			private Dictionary<string, Study> ExistingStudyCache
			{
				get { return _existingStudyCache; }
			}

			private Dictionary<string, Series> ExistingSeriesCache
			{
				get { return _existingSeriesCache; }
			}

			private Dictionary<string, SopInstance> SopInstancesToUpdate
			{
				get { return _sopInstancesToUpdate; }
			}

			private Dictionary<string, Study> StudiesToUpdate
			{
				get { return _studiesToUpdate; }
			}

			private Dictionary<string, Series> SeriesToUpdate
			{
				get { return _seriesToUpdate; }
			}

			private void ClearCache()
			{
				this.ExistingStudyCache.Clear();
				this.ExistingSeriesCache.Clear();

				this.SopInstancesToUpdate.Clear();
				this.StudiesToUpdate.Clear();
				this.SeriesToUpdate.Clear();
			}

			private IDataStoreReader GetIDataStoreReader()
			{
				if (_dataStoreReader == null)
					_dataStoreReader = DataAccessLayer.GetIDataStoreReader();

				return _dataStoreReader;
			}

			private Study GetStudy(string studyInstanceUid)
			{
				if (this.ExistingStudyCache.ContainsKey(studyInstanceUid))
					return this.ExistingStudyCache[studyInstanceUid];

				if (this.StudiesToUpdate.ContainsKey(studyInstanceUid))
					return this.StudiesToUpdate[studyInstanceUid];

				Study existingStudy = (Study)GetIDataStoreReader().GetStudy(new Uid(studyInstanceUid));
				if (existingStudy != null)
				{
					this.ExistingStudyCache[existingStudy.GetStudyInstanceUid()] = existingStudy;
					foreach (Series series in existingStudy.GetSeries())
						this.ExistingSeriesCache[series.GetSeriesInstanceUid()] = series;
				}

				return existingStudy;
			}

			private Series GetSeries(string seriesInstanceUid)
			{
				if (this.ExistingSeriesCache.ContainsKey(seriesInstanceUid))
					return this.ExistingSeriesCache[seriesInstanceUid];

				if (this.SeriesToUpdate.ContainsKey(seriesInstanceUid))
					return this.SeriesToUpdate[seriesInstanceUid];

				Series existingSeries = (Series)GetIDataStoreReader().GetSeries(new Uid(seriesInstanceUid));
				if (existingSeries != null)
					this.ExistingSeriesCache[seriesInstanceUid] = existingSeries;

				return existingSeries;
			}

			private SopInstance GetSopInstance(string sopInstanceUid)
			{
				if (this.SopInstancesToUpdate.ContainsKey(sopInstanceUid))
					return this.SopInstancesToUpdate[sopInstanceUid];

				return (SopInstance)GetIDataStoreReader().GetSopInstance(new Uid(sopInstanceUid));
			}

			#region IDicomPersistentStore Members

			public void UpdateSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, string fileName)
			{
				string studyInstanceUid = sopInstanceDataset[DicomTags.StudyInstanceUid];
				string seriesInstanceUid = sopInstanceDataset[DicomTags.SeriesInstanceUid];
				string sopInstanceUid = sopInstanceDataset[DicomTags.SopInstanceUid];

				Study study = GetStudy(studyInstanceUid);
				Series series = GetSeries(seriesInstanceUid);
				SopInstance image = GetSopInstance(sopInstanceUid);

				bool newStudy = false;
				bool newSeries = false;
				bool newSop = false;

				bool studyDirty = false;
				bool seriesDirty = false;
				bool sopDirty = false;

				if (study == null)
				{
					study = new Study();
					study.StoreTime = Platform.Time;
					newStudy = true;
				}

				if (series == null)
				{
					series = new Series();
					series.Study = study;
					newSeries = true;
				}
				else
				{
					if (series.Study != study)
					{
						string message = String.Format(SR.ExceptionFormatSeriesAlreadyBelongsToExistingStudy, series.SeriesInstanceUid, study.StudyInstanceUid);
						throw new InvalidOperationException(message);
					}
				}

				if (image == null)
				{
					image = new ImageSopInstance();
					image.Series = series;
					newSop = true;
				}
				else
				{
					if (image.Series != series)
					{
						string message = String.Format(SR.ExceptionFormatSopAlreadyBelongsToExistingSeries, series.SeriesInstanceUid, image.SopInstanceUid);
						throw new InvalidOperationException(message);
					}
				}

				EventHandler studyDirtyDelegate = delegate { studyDirty = true; };
				EventHandler seriesDirtyDelegate = delegate { seriesDirty = true; };
				EventHandler sopDirtyDelegate = delegate { sopDirty = true; };

				if (!newStudy)
					study.Changed += studyDirtyDelegate;
				
				if (!newSeries)
					series.Changed += seriesDirtyDelegate;

				if (!newSop)
					image.Changed += sopDirtyDelegate;

				try
				{
					study.Update(metaInfo, sopInstanceDataset);
					series.Update(metaInfo, sopInstanceDataset);
					image.Update(metaInfo, sopInstanceDataset);
				}
				finally
				{
					if (!newStudy)
						study.Changed -= studyDirtyDelegate;
					if (!newSeries)
						series.Changed -= seriesDirtyDelegate;
					if (!newSop)
						image.Changed -= sopDirtyDelegate;
				}

				if (!System.IO.Path.IsPathRooted(fileName))
					fileName = System.IO.Path.GetFullPath(fileName);

				UriBuilder uriBuilder = new UriBuilder();
				uriBuilder.Scheme = "file";
				uriBuilder.Path = fileName;
				image.LocationUri = new DicomUri(uriBuilder.Uri);

				if (studyDirty || newStudy)
					StudiesToUpdate[study.StudyInstanceUid] = study;

				if (seriesDirty || newSeries)
					SeriesToUpdate[series.SeriesInstanceUid] = series;

				if (sopDirty || newSop)
					SopInstancesToUpdate[image.SopInstanceUid] = image;
			}

			public void Commit()
			{
				try
				{
					DisposeReader();

					using (IDataStoreWriter writer = DataAccessLayer.GetIDataStoreWriter())
					{
						writer.StoreStudies(this.StudiesToUpdate.Values);
						writer.StoreSeries(this.SeriesToUpdate.Values);
						writer.StoreSopInstances(this.SopInstancesToUpdate.Values);
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToCommitImagesToDatastore, e);
				}
				finally
				{
					ClearCache();
				}
			}

			#endregion

			private void DisposeReader()
			{
				try
				{
					if (_dataStoreReader != null)
						_dataStoreReader.Dispose();
				}
				finally
				{
					_dataStoreReader = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					DisposeReader();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}

				GC.SuppressFinalize(this);
			}

			#endregion
		}
	}
}