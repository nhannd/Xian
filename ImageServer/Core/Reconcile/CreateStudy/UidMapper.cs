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
		private readonly Dictionary<string, SeriesMapping> _seriesMap = new Dictionary<string, SeriesMapping>();
		private readonly Dictionary<string, string> _sopMap = new Dictionary<string, string>();
        public event EventHandler<SeriesMapUpdatedEventArgs> SeriesMapUpdated;
        public event EventHandler<SopMapUpdatedEventArgs> SopMapUpdated;

        public UidMapper(UidMapXml xml)
        {
            foreach (Map map in xml.Series)
                _seriesMap.Add(map.Source,
                               new SeriesMapping() {OriginalSeriesUid = map.Source, NewSeriesUid = map.Target});

            foreach (Map map in xml.Instances)
                _sopMap.Add(map.Source, map.Target);

        }

		public UidMapper(IList<SeriesMapping> seriesList)
		{
			foreach (SeriesMapping map in seriesList)
				_seriesMap.Add(map.OriginalSeriesUid, map);
		}

		public UidMapper()
		{
		}

		public bool ContainsSop(string originalSopUid)
		{
		    return _sopMap.ContainsKey(originalSopUid);
		}

        public string FindNewSopUid(string originalSopUid)
        {
            string newSopUid;
            if (_sopMap.TryGetValue(originalSopUid, out newSopUid))
                return newSopUid;
            else
                return null;
        }

        public string FindNewSeriesUid(string originalSeriesUid)
        {
            SeriesMapping mapping;
            if (_seriesMap.TryGetValue(originalSeriesUid, out mapping))
                return mapping.NewSeriesUid;
            else
                return null;
        }

	    public bool ContainsSeries(string originalSeriesUid)
        {
            return _seriesMap.ContainsKey(originalSeriesUid);
        }

        public void AddSop(string originalSopUid, string newSopUid)
        {
            _sopMap.Add(originalSopUid, newSopUid);
            EventsHelper.Fire(SeriesMapUpdated, this, new SeriesMapUpdatedEventArgs() {SeriesMap = _seriesMap});
        }

        public void AddSeries(string originalSeriesUid, string newSeriesUid)
        {
            _seriesMap.Add(originalSeriesUid,
                           new SeriesMapping() {OriginalSeriesUid = originalSeriesUid, NewSeriesUid = newSeriesUid});

            EventsHelper.Fire(SopMapUpdated, this, new SopMapUpdatedEventArgs() { SopMap = _sopMap });
        }

	    public IEnumerable<SeriesMapping> GetSeriesMappings()
	    {
	        return _seriesMap.Values;
	    }

	    public void Save(string path)
	    {
	        UidMapXml xml = new UidMapXml();
	        xml.Series = new List<Map>();
            foreach(SeriesMapping seriesMap in _seriesMap.Values)
            {
                xml.Series.Add(new Map() {Source = seriesMap.OriginalSeriesUid, Target = seriesMap.NewSeriesUid});
            }

            xml.Instances = new List<Map>();
            foreach (var sop in _sopMap)
            {
                xml.Instances.Add(new Map() {Source = sop.Key, Target = sop.Value});
            }

	        XmlDocument doc = XmlUtils.SerializeAsXmlDoc(xml);
	        doc.Save(path);
	    }
	}
}
