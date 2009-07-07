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
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class DataStoreQueryHelper
	{
		public static IStudy GetStudy(string studyInstanceUid)
		{
			List<string> studyUids = new List<string>();
			studyUids.Add(studyInstanceUid);

			foreach (IStudy study in GetStudies(studyUids))
				return study;

			//this can't actually happen because GetStudies will throw if the study isn't found
			return null;
		}

		public static IEnumerable<IStudy> GetStudies(IEnumerable<string> studyInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (string studyInstanceUid in studyInstanceUids)
				{
					IStudy study = reader.GetStudy(studyInstanceUid);
					if (study == null)
					{
						string message =
							String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
						throw new ArgumentException(message);
					}

					yield return study;
				}
			}
		}

		public static IEnumerable<ISeries> GetSeries(string studyInstanceUid, IEnumerable<string> seriesInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUid);
				if (study == null)
				{
					string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
					throw new ArgumentException(message);
				}

				foreach (string seriesInstanceUid in seriesInstanceUids)
				{
					ISeries series = CollectionUtils.SelectFirst(study.GetSeries(),
										delegate(ISeries test) { return test.SeriesInstanceUid == seriesInstanceUid; });

					if (series == null)
					{
						string message = String.Format("The specified series does not exist in the data store (study = {0}, series = {1}).", studyInstanceUid, seriesInstanceUid);
						throw new ArgumentException(message);
					}

					yield return series;
				}
			}
		}

		public static IEnumerable<ISopInstance> GetStudySopInstances(IEnumerable<string> studyInstanceUids)
		{
			foreach (IStudy study in GetStudies(studyInstanceUids))
			{
				foreach (ISopInstance sop in study.GetSopInstances())
					yield return sop;
			}
		}

		public static IEnumerable<ISopInstance> GetSeriesSopInstances(string studyInstanceUid, IEnumerable<string> seriesInstanceUids)
		{
			foreach (ISeries series in GetSeries(studyInstanceUid, seriesInstanceUids))
			{
				foreach (ISopInstance sop in series.GetSopInstances())
					yield return sop;
			}
		}

		public static IEnumerable<ISopInstance> GetSopInstances(string studyInstanceUid, string seriesInstanceUid, IEnumerable<string> sopInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUid);
				if (study == null)
				{
					string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
					throw new ArgumentException(message);
				}

				ISeries series = CollectionUtils.SelectFirst(study.GetSeries(),
									delegate(ISeries test) { return test.SeriesInstanceUid == seriesInstanceUid; });

				if (series == null)
				{
					string message = String.Format("The specified series does not exist in the data store (study = {0}, series = {1}).", studyInstanceUid, seriesInstanceUid);
					throw new ArgumentException(message);
				}

				foreach (string sopInstanceUid in sopInstanceUids)
				{
					ISopInstance sop = CollectionUtils.SelectFirst(series.GetSopInstances(),
										delegate(ISopInstance test) { return test.SopInstanceUid == sopInstanceUid; });

					if (sop == null)
					{
						string message = String.Format("The specified sop instance does not exist in the data store (study = {0}, series = {1}, sop = {2}).", studyInstanceUid, seriesInstanceUid, sopInstanceUid);
						throw new ArgumentException(message);
					}

					yield return sop;
				}
			}
		}
	}
}
