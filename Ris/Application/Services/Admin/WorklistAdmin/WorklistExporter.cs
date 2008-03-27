#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Authentication;


namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class WorklistExporter : DataExporterBase
    {
        private IReadContext _context;

        private const string tagWorklists = "worklists";

        private const string tagWorklist = "worklist";
        private const string attrName = "name";
        private const string attrDescription = "description";
        private const string attrEnabled = "enabled";

        private const string tagFilters = "filters";
        private const string tagValue = "value";
        private const string tagProcedureTypeGroups = "procedure-type-groups";
        private const string tagFacilities = "facilities";
        private const string tagPriorities = "priorities";
        private const string tagPatientClasses = "patient-classes";
        private const string tagPortable = "portable";
        private const string tagTimeRange = "time-range";
        private const string tagStartTime = "start-time";
        private const string tagEndTime = "end-time";
        private const string attrFixedValue = "fixed-value";
        private const string attrRelativeValue = "relative-value";
        private const string attrResolution = "resolution";
        private const string attrIncludeWorkingFacility = "includeWorkingFacility";

        private const string attrCode = "code";
        private const string attrClass = "class";

        private const string tagSubscribers = "subscribers";
        private const string tagSubscriber = "subscriber";
        private const string attrSubscriberName = "name";
        private const string attrSubscriberType = "type";
        private const string subscriberType = "user";

        #region DateExporter overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            _context = context;

            writer.WriteStartDocument();
            writer.WriteStartElement(tagWorklists);

            IList<Worklist> worklists = context.GetBroker<IWorklistBroker>().FindAll();
            CollectionUtils.ForEach(worklists,
                delegate(Worklist worklist) { WriteWorklistXml(worklist, writer); });

            writer.WriteEndElement();
        }

        #endregion

        #region Private methods

        private void WriteWorklistXml(Worklist worklist, XmlWriter writer)
        {
            writer.WriteStartElement(tagWorklist);
            writer.WriteAttributeString(attrName, worklist.Name);
            writer.WriteAttributeString(attrClass, worklist.GetClass().FullName);
            writer.WriteAttributeString(attrDescription, worklist.Description);

            WriteFilters(worklist, writer);
            WriteSubscribers(worklist, writer);

            writer.WriteEndElement();
        }

        private void WriteFilters(Worklist worklist, XmlWriter writer)
        {
            writer.WriteStartElement(tagFilters);

            WriteFilter<ProcedureTypeGroup>(worklist.ProcedureTypeGroupFilter, writer, tagProcedureTypeGroups,
                delegate(ProcedureTypeGroup value)
                {
                    writer.WriteAttributeString(attrName, value.Name);
                    writer.WriteAttributeString(attrClass, value.GetClass().FullName);
                });

            WriteFilter<Facility>(worklist.FacilityFilter, writer, tagFacilities,
                delegate(Facility value)
                {
                    writer.WriteAttributeString(attrCode, value.Code);
                },
                delegate
                {
                    if(worklist.FacilityFilter.IsEnabled)
                        writer.WriteAttributeString(attrIncludeWorkingFacility, worklist.FacilityFilter.IncludeWorkingFacility.ToString());
                });

            WriteFilter<OrderPriorityEnum>(worklist.OrderPriorityFilter, writer, tagPriorities,
                delegate(OrderPriorityEnum value)
                {
                    writer.WriteAttributeString(attrCode, value.Code);
                });

            WriteFilter<PatientClassEnum>(worklist.PatientClassFilter, writer, tagPatientClasses,
               delegate(PatientClassEnum value)
               {
                   writer.WriteAttributeString(attrCode, value.Code);
               });

            WriteFilter<bool>(worklist.PortableFilter, writer, tagPortable,
                delegate (bool value)
                {
                    writer.WriteAttributeString(tagPortable, value.ToString());
                });

            WriteFilter<WorklistTimeRange>(worklist.TimeFilter, writer, tagTimeRange,
                delegate (WorklistTimeRange value)
                {
                    WriteTimeRange(value, writer);
                });
            
            writer.WriteEndElement();
        }

        private void WriteFilter<T>(WorklistFilter filter, XmlWriter writer, string tagName, Action<T> writeValueCallback)
        {
            WriteFilter(filter, writer, tagName, writeValueCallback, null);
        }

        private void WriteFilter<T>(WorklistFilter filter, XmlWriter writer, string tagName, Action<T> writeValueCallback, Action<XmlWriter> writeFilterCallback)
        {
            writer.WriteStartElement(tagName);
            writer.WriteAttributeString(attrEnabled, filter.IsEnabled.ToString());
            if (writeFilterCallback != null)
                writeFilterCallback(writer);
            if (filter.IsEnabled)
            {
                if(filter is WorklistMultiValuedFilter<T>)
                {
                    WorklistMultiValuedFilter<T> multiFilter = (WorklistMultiValuedFilter<T>) filter;
                    foreach (T value in multiFilter.Values)
                    {
                        writer.WriteStartElement(tagValue);
                        writeValueCallback(value);
                        writer.WriteEndElement();
                    }
                }
                else if(filter is WorklistSingleValuedFilter<T>)
                {
                    WorklistSingleValuedFilter<T> singleFilter = (WorklistSingleValuedFilter<T>)filter;
                    writer.WriteStartElement(tagValue);
                    writeValueCallback(singleFilter.Value);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private void WriteTimeRange(WorklistTimeRange range, XmlWriter writer)
        {
            if(range.Start != null)
                WriteTimePoint(range.Start, writer, tagStartTime);
            if(range.End != null)
                WriteTimePoint(range.End, writer, tagEndTime);
        }

        private void WriteTimePoint(WorklistTimePoint point, XmlWriter writer, string tagName)
        {
            writer.WriteStartElement(tagName);
            if(point.IsFixed)
                writer.WriteAttributeString(attrFixedValue, DateTimeUtils.FormatISO(point.FixedValue.Value));
            else 
                writer.WriteAttributeString(attrRelativeValue, point.RelativeValue.Value.ToString());
            writer.WriteAttributeString(attrResolution, point.Resolution.ToString());
            writer.WriteEndElement();
        }

        private void WriteSubscribers(Worklist worklist, XmlWriter writer)
        {
            writer.WriteStartElement(tagSubscribers);
            CollectionUtils.ForEach<string>(worklist.Users,
                delegate(string user) { WriteSubscriber(user, writer); });
            writer.WriteEndElement();
        }

        private void WriteSubscriber(string user, XmlWriter writer)
        {
            writer.WriteStartElement(tagSubscriber);
            writer.WriteAttributeString(attrSubscriberName, user);
            writer.WriteAttributeString(attrSubscriberType, subscriberType);
            writer.WriteEndElement();
        }

        #endregion
    }
}
