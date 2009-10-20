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
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;

namespace ClearCanvas.ImageServer.Core.Reconcile.CreateStudy
{
    public class SeriesMapUpdatedEventArgs:EventArgs
    {
        public Dictionary<string, SeriesMapping> SeriesMap { get; set; }
    }

    public class SopMapUpdatedEventArgs : EventArgs
    {
        public Dictionary<string, string> SopMap { get; set; }
    }

	public class UidMapper
	{
		#region Private Members

		private readonly Dictionary<string, string> _studyMap = new Dictionary<string, string>();
        private readonly Dictionary<string, SeriesMapping> _seriesMap = new Dictionary<string, SeriesMapping>();
		private readonly Dictionary<string, string> _sopMap = new Dictionary<string, string>();        
	    private object _sync = new object();

		#endregion

		#region Events

		public event EventHandler<SeriesMapUpdatedEventArgs> SeriesMapUpdated;
		public event EventHandler<SopMapUpdatedEventArgs> SopMapUpdated;
		
		#endregion

		#region Public Properties

		public bool Dirty { get; set; }

		#endregion

		#region Constructors

		public UidMapper(UidMapXml xml)
        {
            foreach(StudyUidMap studyMap in xml.StudyUidMaps)
            {
                _studyMap.Add(studyMap.Source, studyMap.Target);

                foreach (Map seriesMap in studyMap.Series)
                    _seriesMap.Add(seriesMap.Source,
                                   new SeriesMapping { OriginalSeriesUid = seriesMap.Source, NewSeriesUid = seriesMap.Target });

                foreach (Map sopMap in studyMap.Instances)
                    _sopMap.Add(sopMap.Source, sopMap.Target);
            
            }           
        }

		public UidMapper(IEnumerable<SeriesMapping> seriesList)
		{
			foreach (SeriesMapping map in seriesList)
				_seriesMap.Add(map.OriginalSeriesUid, map);
		}

		public UidMapper()
		{
		}

		#endregion

		#region Public Methods

		public bool ContainsSop(string originalSopUid)
		{
            lock(_sync)
            {
                return _sopMap.ContainsKey(originalSopUid);
            }
		    
		}

        public string FindNewSopUid(string originalSopUid)
        {
            lock (_sync)
            {
                string newSopUid;
                if (_sopMap.TryGetValue(originalSopUid, out newSopUid))
                    return newSopUid;
            	return null;
            }
        }

        public string FindNewSeriesUid(string originalSeriesUid)
        {
            lock (_sync)
            {
                SeriesMapping mapping;
                if (_seriesMap.TryGetValue(originalSeriesUid, out mapping))
                    return mapping.NewSeriesUid;
            	return null;
            }
        }

	    public bool ContainsSeries(string originalSeriesUid)
        {
            lock (_sync)
            {
                return _seriesMap.ContainsKey(originalSeriesUid);
            }
        }

        public void AddSop(string originalSopUid, string newSopUid)
        {
            lock (_sync)
            {
                _sopMap.Add(originalSopUid, newSopUid);
                Dirty = true;
            }

            EventsHelper.Fire(SeriesMapUpdated, this, new SeriesMapUpdatedEventArgs { SeriesMap = _seriesMap });
        }

        public void AddSeries(string originalStudyUid, string newStudyUid, string originalSeriesUid, string newSeriesUid)
        {
            lock (_sync)
            {
                if (!ContainsStudyMap(originalStudyUid, newStudyUid))
                {
                    _studyMap.Add(originalStudyUid, newStudyUid);
                }

                _seriesMap.Add(originalSeriesUid,
                               new SeriesMapping {OriginalSeriesUid = originalSeriesUid, NewSeriesUid = newSeriesUid});

                Dirty = true;
            }
            EventsHelper.Fire(SopMapUpdated, this, new SopMapUpdatedEventArgs { SopMap = _sopMap });
		}
		
		public IEnumerable<SeriesMapping> GetSeriesMappings()
	    {
	        return _seriesMap.Values;
	    }

	    public void Save(string path)
	    {
            lock (_sync)
            {
                UidMapXml xml = new UidMapXml();
                xml.StudyUidMaps = new List<StudyUidMap>();

                foreach (var entry in _studyMap)
                {
                    StudyUidMap studyMap = new StudyUidMap {Source = entry.Key, Target = entry.Value};
                    xml.StudyUidMaps.Add(studyMap);

                    studyMap.Series = new List<Map>();
                    foreach (SeriesMapping seriesMap in _seriesMap.Values)
                    {
                        studyMap.Series.Add(new Map {Source = seriesMap.OriginalSeriesUid, Target = seriesMap.NewSeriesUid});
                    }

                    studyMap.Instances = new List<Map>();
                    foreach (var sop in _sopMap)
                    {
                        studyMap.Instances.Add(new Map {Source = sop.Key, Target = sop.Value});
                    }
                }

                XmlDocument doc = XmlUtils.SerializeAsXmlDoc(xml);
                doc.Save(path);

                Dirty = false;
            }
	    }

		#endregion

		#region Private Methods

		private bool ContainsStudyMap(string originalStudyUid, string newStudyUid)
		{
			lock (_sync)
			{
				foreach (var entry in _studyMap)
				{
					if (entry.Key == originalStudyUid && entry.Value == newStudyUid)
					{
						return true;
					}
				}

				return false;
			}
		}

		#endregion
	}
}
