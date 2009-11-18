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
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory
{
    [ExtensionOf(typeof(WebDeleteProcessorExtensionPoint))]
    class LogHistoryExtension:IWebDeleteProcessorExtension
    {
        private StudyInformation _studyInfo;
        
        #region IWebDeleteProcessorExtension Members

        public void OnSeriesDeleting(WebDeleteProcessorContext context, Series series)
        {
            _studyInfo = StudyInformation.CreateFrom(context.StorageLocation.Study);
        }

        public void OnSeriesDeleted(WebDeleteProcessorContext context, Series _series)
        {
            
        }

        #endregion

        #region IWebDeleteProcessorExtension Members


        public void OnCompleted(WebDeleteProcessorContext context, IList<Series> series)
        {
            if (series.Count > 0)
            {
                Platform.Log(LogLevel.Info, "Logging history..");
                DateTime now = Platform.Time;
                using(IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                    StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
                    columns.InsertTime = Platform.Time;
                    columns.StudyHistoryTypeEnum = StudyHistoryTypeEnum.SeriesDeleted;
                    columns.StudyStorageKey = context.StorageLocation.Key;
                    columns.DestStudyStorageKey = context.StorageLocation.Key;
                    columns.StudyData = XmlUtils.SerializeAsXmlDoc(_studyInfo);
                    SeriesDeletionChangeLog changeLog =  new SeriesDeletionChangeLog();
                    changeLog.TimeStamp = now;
                    changeLog.Reason = context.Reason;
                    changeLog.UserId = context.UserId;
                    changeLog.UserName = context.UserName;
                    changeLog.Series = CollectionUtils.Map(series,
                                      delegate(Series ser)
                                          {
                                              ServerEntityAttributeProvider seriesWrapper = new ServerEntityAttributeProvider(ser);
                                              return new SeriesInformation(seriesWrapper);
                                          });
                    columns.ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog);
                    StudyHistory history = broker.Insert(columns);
                    if (history!=null)
                        ctx.Commit();
                    
                }
            }
            
        }

        #endregion
    }

    public class SeriesDeletionChangeLog
    {
        private DateTime _timeStamp;
        private string _userId;
        private string _userName;
        private string _reason;
        private List<SeriesInformation> _seriesInfo;
        
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
        [XmlArray("DeletedSeries")]
        public List<SeriesInformation> Series
        {
            get { return _seriesInfo; }
            set { _seriesInfo = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
    }
}
