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
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Worklist Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class WorklistImporter : DataImporterBase
    {
        private const string tagRoot = "worklists";

        private const string tagWorklist = "worklist";

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
        private const string attrName = "name";
        private const string attrId = "id";
        private const string attrClass = "class";
        private const string attrCode = "code";
        private const string attrDescription = "description";
        private const string attrEnabled = "enabled";

        private const string tagStaffSubscribers = "staff-subscribers";
        private const string tagGroupSubscribers = "group-subscribers";

        private IUpdateContext _context;
        private IList<Type> _procedureTypeGroupClasses;

        #region DateImporterBase overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;
            if (_procedureTypeGroupClasses == null)
            {
                _procedureTypeGroupClasses = ProcedureTypeGroup.ListSubClasses(context);
            }

            while (reader.Read())
            {
                if (reader.IsStartElement(tagRoot))
                {
                    ReadWorklists(reader.ReadSubtree());
                }
            }
        }

        #endregion

        private void ReadWorklists(XmlReader xmlReader)
        {
            for (bool elementExists = xmlReader.ReadToDescendant(tagWorklist);
                elementExists;
                elementExists = xmlReader.ReadToNextSibling(tagWorklist))
            {
                ReadWorklist(xmlReader.ReadSubtree());
            }
        }

        private void ReadWorklist(XmlReader reader)
        {
            reader.Read();

            string name = reader.GetAttribute(attrName);
            string className = reader.GetAttribute(attrClass);

            Worklist worklist = LoadOrCreateWorklist(name, className);
            if (worklist != null)
            {
                worklist.Description = reader.GetAttribute(attrDescription);

                reader.ReadToFollowing(tagFilters);
                ReadFilters(worklist, reader.ReadSubtree());

                ReadSubscribers(worklist.StaffSubscribers, reader, tagStaffSubscribers,
                    delegate
                    {
                        StaffSearchCriteria criteria = new StaffSearchCriteria();
                        criteria.Id.EqualTo(reader.GetAttribute(attrId));

                        IStaffBroker broker = _context.GetBroker<IStaffBroker>();
                        return CollectionUtils.FirstElement(broker.Find(criteria));
                    });

                ReadSubscribers(worklist.GroupSubscribers, reader, tagGroupSubscribers,
                    delegate
                    {
                        StaffGroupSearchCriteria criteria = new StaffGroupSearchCriteria();
                        criteria.Name.EqualTo(reader.GetAttribute(attrName));

                        IStaffGroupBroker broker = _context.GetBroker<IStaffGroupBroker>();
                        return CollectionUtils.FirstElement(broker.Find(criteria));
                    });
            }

            while (reader.Read()) ;
            reader.Close();
        }

        private void ReadFilters(Worklist worklist, XmlReader reader)
        {
            ReadFilter<ProcedureTypeGroup>(worklist.ProcedureTypeGroupFilter, reader, tagProcedureTypeGroups,
                delegate
                {
                    ProcedureTypeGroupSearchCriteria criteria = new ProcedureTypeGroupSearchCriteria();
                    criteria.Name.EqualTo(reader.GetAttribute(attrName));

                    string groupClassName = reader.GetAttribute(attrClass);
                    Type groupClass = CollectionUtils.SelectFirst(_procedureTypeGroupClasses,
                       delegate(Type t)
                       {
                           return t.FullName.Equals(groupClassName, StringComparison.InvariantCultureIgnoreCase);
                       });

                    IProcedureTypeGroupBroker broker = _context.GetBroker<IProcedureTypeGroupBroker>();
                    return CollectionUtils.FirstElement(broker.Find(criteria, groupClass));
                });

            ReadFilter<Facility>(worklist.FacilityFilter, reader, tagFacilities,
                delegate
                {
                    FacilitySearchCriteria criteria = new FacilitySearchCriteria();
                    criteria.Code.EqualTo(reader.GetAttribute(attrCode));

                    IFacilityBroker broker = _context.GetBroker<IFacilityBroker>();
                    return CollectionUtils.FirstElement(broker.Find(criteria));
                },
                delegate
                {
                    string b = reader.GetAttribute(attrIncludeWorkingFacility);
                    if(!string.IsNullOrEmpty(b))
                        worklist.FacilityFilter.IncludeWorkingFacility = bool.Parse(b);
                });

            ReadFilter<OrderPriorityEnum>(worklist.OrderPriorityFilter, reader, tagPriorities,
                delegate
                {
                    IEnumBroker broker = _context.GetBroker<IEnumBroker>();
                    return broker.Find<OrderPriorityEnum>(reader.GetAttribute(attrCode));
                });

            ReadFilter<PatientClassEnum>(worklist.PatientClassFilter, reader, tagPatientClasses,
                delegate
                {
                    IEnumBroker broker = _context.GetBroker<IEnumBroker>();
                    return broker.Find<PatientClassEnum>(reader.GetAttribute(attrCode));
                });

            ReadFilter<bool>(worklist.PortableFilter, reader, tagPortable,
                delegate
                {
                    return bool.Parse(reader.GetAttribute(tagPortable));
                });

            ReadFilter<WorklistTimeRange>(worklist.TimeFilter, reader, tagTimeRange,
                delegate
                {
                    return ReadTimeRange(reader);
                });


            while (reader.Read()) ;
            reader.Close();
        }

        private delegate T ReadValueDelegate<T>();

        private void ReadFilter<T>(WorklistFilter filter, XmlReader reader, string tagName, ReadValueDelegate<T> readValueCallback)
        {
            ReadFilter(filter, reader, tagName, readValueCallback, null);
        }

        private void ReadFilter<T>(WorklistFilter filter, XmlReader reader, string tagName, ReadValueDelegate<T> readValueCallback, Action<XmlReader> readFilterCallback)
        {
            if(reader.ReadToFollowing(tagName))
            {
                filter.IsEnabled = bool.Parse(reader.GetAttribute(attrEnabled));
                if (readFilterCallback != null)
                    readFilterCallback(reader);

                if (filter.IsEnabled)
                {
                    for (bool elementExists = reader.ReadToDescendant(tagValue);
                        elementExists;
                        elementExists = reader.ReadToNextSibling(tagValue))
                    {
                        T value = readValueCallback();
                        if (value != null)
                        {
                            if (filter is WorklistMultiValuedFilter<T>)
                                (filter as WorklistMultiValuedFilter<T>).Values.Add(value);
                            else if(filter is WorklistSingleValuedFilter<T>)
                                (filter as WorklistSingleValuedFilter<T>).Value = value;
                        }
                    }
                }
            }
        }

        private WorklistTimeRange ReadTimeRange(XmlReader reader)
        {
            WorklistTimePoint startPoint = reader.ReadToFollowing(tagStartTime) ? ReadTimePoint(reader) : null;
            WorklistTimePoint endPoint = reader.ReadToFollowing(tagEndTime) ? ReadTimePoint(reader) : null;

            if(startPoint != null || endPoint != null)
                return new WorklistTimeRange(startPoint, endPoint);
            else
                return null;
        }

        private WorklistTimePoint ReadTimePoint(XmlReader reader)
        {
            string relativeTimeString = reader.GetAttribute(attrRelativeValue);
            string fixedTimeString = reader.GetAttribute(attrFixedValue);
            int resolution = int.Parse(reader.GetAttribute(attrResolution));

            if (!string.IsNullOrEmpty(relativeTimeString))
                return new WorklistTimePoint(TimeSpan.Parse(relativeTimeString), resolution);
            if (!string.IsNullOrEmpty(fixedTimeString))
                return new WorklistTimePoint(DateTimeUtils.ParseISO(fixedTimeString).Value, resolution);

            return null;
        }

        private void ReadSubscribers<T>(ICollection<T> subscribers, XmlReader reader, string tagName, ReadValueDelegate<T> readValueCallback)
        {
            if (reader.ReadToFollowing(tagName))
            {
                for (bool elementExists = reader.ReadToDescendant(tagValue);
                    elementExists;
                    elementExists = reader.ReadToNextSibling(tagValue))
                {
                    T value = readValueCallback();
                    if (value != null)
                    {
                        subscribers.Add(value);
                    }
                }
            }
        }
        
        private Worklist LoadOrCreateWorklist(string name, string worklistClassName)
        {
            Worklist worklist;

            try
            {
                worklist = _context.GetBroker<IWorklistBroker>().FindWorklist(name, worklistClassName);
            }
            catch (EntityNotFoundException)
            {
                worklist = WorklistFactory.Instance.CreateWorklist(worklistClassName);
                worklist.Name = name;

                _context.Lock(worklist, DirtyState.New);
            }

            return worklist;
        }
    }
}
