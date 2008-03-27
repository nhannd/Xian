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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistEditorComponentViewExtensionPoint))]
    public class WorklistEditorComponent : ApplicationComponent
    {
        #region Helper classes

        class UserTable : Table<string>
        {
            public UserTable()
            {
                this.Columns.Add(new TableColumn<string, string>("User", delegate(string user) { return user; }));
            }
        }

        class DummyItem
        {
            private readonly string _displayString;

            public DummyItem(string displayString)
            {
                _displayString = displayString;
            }

            public override string ToString()
            {
                return _displayString;
            }
        }

        #endregion

        #region Constants

        private static readonly object _nullFilterItem = new DummyItem("(None)");
        private static readonly object _workingFacilityItem = new DummyItem("(Working Facility)");
        private static readonly object _portableItem = new DummyItem("Portable");
        private static readonly object _nonPortableItem = new DummyItem("Non-portable");

        private static readonly object[] _portableChoices = new object[] { _portableItem, _nonPortableItem };

        private static readonly object Hours = new DummyItem("Hours");
        private static readonly object Days = new DummyItem("Days");

        private static readonly object[] _slidingScaleChoices = { Days, Hours };

        private static readonly RelativeTime[] _slidingDayChoices = 
            {
                new RelativeTimeInDays(120),
                new RelativeTimeInDays(90),
                new RelativeTimeInDays(60),
                new RelativeTimeInDays(45),
                new RelativeTimeInDays(30),
                new RelativeTimeInDays(21),
                new RelativeTimeInDays(14),
                new RelativeTimeInDays(7),
                new RelativeTimeInDays(6),
                new RelativeTimeInDays(5),
                new RelativeTimeInDays(4),
                new RelativeTimeInDays(3),
                new RelativeTimeInDays(2),
                new RelativeTimeInDays(1),
                new RelativeTimeInDays(0),
                new RelativeTimeInDays(-1),
                new RelativeTimeInDays(-2),
                new RelativeTimeInDays(-3),
                new RelativeTimeInDays(-4),
                new RelativeTimeInDays(-5),
                new RelativeTimeInDays(-6),
                new RelativeTimeInDays(-7),
                new RelativeTimeInDays(-14),
                new RelativeTimeInDays(-21),
                new RelativeTimeInDays(-30),
                new RelativeTimeInDays(-45),
                new RelativeTimeInDays(-60),
                new RelativeTimeInDays(-90),
                new RelativeTimeInDays(-120)
            };

        private static readonly RelativeTime[] _slidingHourChoices = 
            {
                new RelativeTimeInHours(24),
                new RelativeTimeInHours(18),
                new RelativeTimeInHours(12),
                new RelativeTimeInHours(8),
                new RelativeTimeInHours(4),
                new RelativeTimeInHours(0),
                new RelativeTimeInHours(-4),
                new RelativeTimeInHours(-8),
                new RelativeTimeInHours(-12),
                new RelativeTimeInHours(-18),
                new RelativeTimeInHours(-24)
            };

        #endregion

        private readonly bool _isNew;

        private EntityRef _editedItemEntityRef;
        private WorklistAdminSummary _editedItemSummary;
        private WorklistAdminDetail _editedItemDetail;

        private List<string> _typeChoices;

        private ProcedureTypeGroupSummaryTable _availableProcedureTypeGroups;
        private ProcedureTypeGroupSummaryTable _selectedProcedureTypeGroups;
        private event EventHandler _availableItemsChanged;

        private UserTable _availableUsers;
        private UserTable _selectedUsers;


        private ArrayList _facilityChoices;
        private ArrayList _selectedFacilities;

        private List<EnumValueInfo> _priorityChoices;
        private List<EnumValueInfo> _patientClassChoices;
        private ArrayList _selectedPortabilities;


        private bool _startTimeChecked;
        private bool _isFixedTimeWindow;
        private bool _endTimeChecked;
        private RelativeTime _slidingStartTime;
        private RelativeTime _slidingEndTime;
        private DateTime _fixedStartTime;
        private DateTime _fixedEndTime;
        private object _slidingScale;


        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistEditorComponent()
        {
            _isNew = true;
        }

        public WorklistEditorComponent(EntityRef entityRef)
        {
            _isNew = false;
            _editedItemEntityRef = entityRef;
        }

        public override void Start()
        {
            _availableProcedureTypeGroups = new ProcedureTypeGroupSummaryTable();
            _selectedProcedureTypeGroups = new ProcedureTypeGroupSummaryTable();

            _availableUsers = new UserTable();
            _selectedUsers = new UserTable();

            Platform.GetService<IWorklistAdminService>(
                delegate(IWorklistAdminService service)
                {
                    GetWorklistEditFormDataResponse formDataResponse = service.GetWorklistEditFormData(new GetWorklistEditFormDataRequest());

                    _typeChoices = formDataResponse.WorklistTypes;
                    _availableUsers.Items.AddRange(formDataResponse.Users);

                    _facilityChoices = new ArrayList();
                    _facilityChoices.Add(_workingFacilityItem);
                    _facilityChoices.AddRange(formDataResponse.FacilityChoices);

                    _priorityChoices = formDataResponse.OrderPriorityChoices;
                    _patientClassChoices = formDataResponse.PatientClassChoices;

                    if (_isNew)
                    {
                        _editedItemDetail = new WorklistAdminDetail();
                        _editedItemDetail.WorklistType = _typeChoices[0];
                    }
                    else
                    {
                        LoadWorklistForEditResponse response =
                            service.LoadWorklistForEdit(new LoadWorklistForEditRequest(_editedItemEntityRef));

                        _editedItemDetail = response.Detail;

                        _selectedProcedureTypeGroups.Items.AddRange(_editedItemDetail.ProcedureTypeGroups);
                        _selectedUsers.Items.AddRange(_editedItemDetail.Users);

                        _selectedFacilities = new ArrayList();
                        if (_editedItemDetail.FilterByWorkingFacility)
                            _selectedFacilities.Add(_workingFacilityItem);
                        _selectedFacilities.AddRange(_editedItemDetail.Facilities);

                        _selectedPortabilities = new ArrayList();
                        if (_editedItemDetail.Portabilities.Contains(true))
                            _selectedPortabilities.Add(_portableItem);
                        if(_editedItemDetail.Portabilities.Contains(false))
                            _selectedPortabilities.Add(_nonPortableItem);
                    }

                    InitializeTimeWindow();

                    ListProcedureTypeGroupsForWorklistCategoryRequest groupsRequest = new ListProcedureTypeGroupsForWorklistCategoryRequest(_editedItemDetail.WorklistType);
                    ListProcedureTypeGroupsForWorklistCategoryResponse groupsResponse = service.ListProcedureTypeGroupsForWorklistCategory(groupsRequest);
                    _availableProcedureTypeGroups.Items.AddRange(groupsResponse.ProcedureTypeGroups);

                    foreach (ProcedureTypeGroupSummary selectedSummary in _selectedProcedureTypeGroups.Items)
                    {
                        _availableProcedureTypeGroups.Items.Remove(selectedSummary);
                    }

                    foreach (string userName in _selectedUsers.Items)
                    {
                        _availableUsers.Items.Remove(userName);
                    }

                });

            base.Start();
        }

        private void InitializeTimeWindow()
        {
            // init both fixed and sliding times to "now"
            _fixedStartTime = _fixedEndTime = Platform.Time;
            _slidingStartTime = _slidingEndTime = new RelativeTimeInDays(0);

            if(_editedItemDetail.StartTime != null)
            {
                _startTimeChecked = true;
                if(_editedItemDetail.StartTime.FixedTime != null)
                {
                    _fixedStartTime = _editedItemDetail.StartTime.FixedTime.Value;
                    _isFixedTimeWindow = true;
                }

                if (_editedItemDetail.StartTime.RelativeTime != null)
                {
                    _slidingStartTime = ConvertTimePointToRelativeTime(_editedItemDetail.StartTime);
                }
            }

            if (_editedItemDetail.EndTime != null)
            {
                _endTimeChecked = true;
                if (_editedItemDetail.EndTime.FixedTime != null)
                {
                    _fixedEndTime = _editedItemDetail.EndTime.FixedTime.Value;
                    _isFixedTimeWindow = true;
                }

                if (_editedItemDetail.EndTime.RelativeTime != null)
                {
                    _slidingEndTime = ConvertTimePointToRelativeTime(_editedItemDetail.EndTime);
                }
            }

            if(_slidingStartTime is RelativeTimeInHours || _slidingEndTime is RelativeTimeInHours)
                _slidingScale = Hours;
            else
                _slidingScale = Days;
        }

        public override void Stop()
        {
            base.Stop();
        }

        public WorklistAdminSummary EditedWorklistSummary
        {
            get { return _editedItemSummary; }
        }

        #region Presentation Model

        public string Name
        {
            get { return _editedItemDetail.Name; }
            set
            {
                _editedItemDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _editedItemDetail.Description; }
            set
            {
                _editedItemDetail.Description = value;
                this.Modified = true;
            }
        }

        public string Type
        {
            get { return _editedItemDetail.WorklistType; }
            set
            {
                if (_editedItemDetail.WorklistType != value)
                {
                    _editedItemDetail.WorklistType = value;
                    OnWorklistTypeChanged();
                    this.Modified = true;
                }
            }
        }

        public IList<string> TypeChoices
        {
            get { return _typeChoices; }
        }

        public bool TypeEnabled
        {
            get { return _isNew; }
        }

        public ITable AvailableProcedureTypeGroups
        {
            get { return _availableProcedureTypeGroups; }
        }

        public ITable SelectedProcedureTypeGroups
        {
            get { return _selectedProcedureTypeGroups; }
        }

        public event EventHandler AvailableItemsChanged
        {
            add { _availableItemsChanged += value; }
            remove { _availableItemsChanged -= value; }
        }

        public ITable AvailableUsers
        {
            get { return _availableUsers; }
        }

        public ITable SelectedUsers
        {
            get { return _selectedUsers; }
        }

        public object NullFilterItem
        {
            get { return _nullFilterItem; }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        public string FormatFacility(object item)
        {
            if (item is FacilitySummary)
            {
                FacilitySummary facility = (FacilitySummary)item;
                return facility.Code;
            }
            else
                return item.ToString(); // place-holder items
        }

        public IList SelectedFacilities
        {
            get { return _selectedFacilities; }
            set
            {
                if (!CollectionUtils.Equal(value, _selectedFacilities, false))
                {
                    _selectedFacilities = new ArrayList(value);
                    this.Modified = true;
                }
            }
        }

        public IList PriorityChoices
        {
            get { return _priorityChoices; }
        }

        public IList SelectedPriorities
        {
            get { return _editedItemDetail.OrderPriorities; }
            set
            {
                IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
                if (!CollectionUtils.Equal(list, _editedItemDetail.OrderPriorities, false))
                {
                    _editedItemDetail.OrderPriorities = new List<EnumValueInfo>(list);
                    this.Modified = true;
                }
            }
        }

        public IList PatientClassChoices
        {
            get { return _patientClassChoices; }
        }

        public IList SelectedPatientClasses
        {
            get { return _editedItemDetail.PatientClasses; }
            set
            {
                IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
                if (!CollectionUtils.Equal(list, _editedItemDetail.PatientClasses, false))
                {
                    _editedItemDetail.PatientClasses = new List<EnumValueInfo>(list);
                    this.Modified = true;
                }
            }
        }

        public IList PortableChoices
        {
            get { return _portableChoices; }
        }

        public IList SelectedPortabilities
        {
            get { return _selectedPortabilities; }
            set
            {
                if (!CollectionUtils.Equal(value, _selectedPortabilities, false))
                {
                    _selectedPortabilities = new ArrayList(value);
                    this.Modified = true;
                }
            }
        }

        public bool IsFixedTimeWindow
        {
            get { return _isFixedTimeWindow;  }
            set
            {
                if (value != _isFixedTimeWindow)
                {
                    _isFixedTimeWindow = value;
                    this.Modified = true;

                    NotifyPropertyChanged("SlidingStartTimeEnabled");
                    NotifyPropertyChanged("FixedStartTimeEnabled");
                    NotifyPropertyChanged("SlidingEndTimeEnabled");
                    NotifyPropertyChanged("FixedEndTimeEnabled");
                    NotifyPropertyChanged("SlidingScaleEnabled");
                }
            }
        }

        public bool IsSlidingTimeWindow
        {
            get { return !_isFixedTimeWindow; }
            set
            {
                // do nothing - the reciprocal IsFixedTimeWindow takes care of it
            }
        }

        public bool FixedSlidingChoiceEnabled
        {
            get { return _startTimeChecked || _endTimeChecked; }
        }

        public bool StartTimeChecked
        {
            get { return _startTimeChecked; }
            set
            {
                if(value != _startTimeChecked)
                {
                    _startTimeChecked = value;
                    this.Modified = true;

                    NotifyPropertyChanged("SlidingStartTimeEnabled");
                    NotifyPropertyChanged("FixedStartTimeEnabled");
                    NotifyPropertyChanged("FixedSlidingChoiceEnabled");
                    NotifyPropertyChanged("SlidingScaleEnabled");
                }
            }
        }

        public bool SlidingStartTimeEnabled
        {
            get { return _startTimeChecked && !_isFixedTimeWindow; }
        }

        public bool FixedStartTimeEnabled
        {
            get { return _startTimeChecked && _isFixedTimeWindow; }
        }

        public bool EndTimeChecked
        {
            get { return _endTimeChecked; }
            set
            {
                if (value != _endTimeChecked)
                {
                    _endTimeChecked = value;
                    this.Modified = true;

                    NotifyPropertyChanged("SlidingEndTimeEnabled");
                    NotifyPropertyChanged("FixedEndTimeEnabled");
                    NotifyPropertyChanged("FixedSlidingChoiceEnabled");
                    NotifyPropertyChanged("SlidingScaleEnabled");
                }
            }
        }

        public bool SlidingEndTimeEnabled
        {
            get { return _endTimeChecked && !_isFixedTimeWindow; }
        }

        public bool FixedEndTimeEnabled
        {
            get { return _endTimeChecked && _isFixedTimeWindow; }
        }

        public DateTime FixedStartTime
        {
            get { return _fixedStartTime; }
            set
            {
                if(_fixedStartTime != value)
                {
                    _fixedStartTime = value;
                    this.Modified = true;
                }
            }
        }

        public DateTime FixedEndTime
        {
            get { return _fixedEndTime; }
            set
            {
                if (_fixedEndTime != value)
                {
                    _fixedEndTime = value;
                    this.Modified = true;
                }
            }
        }

        public bool SlidingScaleEnabled
        {
            get { return !_isFixedTimeWindow && (_startTimeChecked || _endTimeChecked); }
        }

        public IList SlidingScaleChoices
        {
            get { return _slidingScaleChoices; }
        }

        public object SlidingScale
        {
            get { return _slidingScale; }
            set
            {
                if(value != _slidingScale)
                {
                    _slidingScale = value;
                    NotifyPropertyChanged("SlidingStartTimeChoices");
                    NotifyPropertyChanged("SlidingEndTimeChoices");

                    if (_slidingScale == Hours)
                    {
                        _slidingStartTime = _slidingEndTime = new RelativeTimeInHours(0);
                    }
                    else
                    {
                        _slidingStartTime = _slidingEndTime = new RelativeTimeInDays(0);
                    }

                    NotifyPropertyChanged("SlidingStartTime");
                    NotifyPropertyChanged("SlidingEndTime");
                }
            }
        }

        public IList SlidingStartTimeChoices
        {
            get { return new ArrayList(SlidingTimeChoices); }
        }

        public IList SlidingEndTimeChoices
        {
            get { return new ArrayList(SlidingTimeChoices); }
        }

        public RelativeTime SlidingStartTime
        {
            get
            {
                 return _slidingStartTime;
            }
            set
            {
                if(!Equals(value, _slidingStartTime))
                {
                    _slidingStartTime = value;
                    this.Modified = true;
                }
            }
        }

        public RelativeTime SlidingEndTime
        {
            get
            {
                 return _slidingEndTime;
            }
            set
            {
                if (!Equals(value, _slidingEndTime))
                {
                    _slidingEndTime = value;
                    this.Modified = true;
                }
            }
        }
        
        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    _editedItemDetail.ProcedureTypeGroups.Clear();
                    _editedItemDetail.ProcedureTypeGroups.AddRange(_selectedProcedureTypeGroups.Items);

                    _editedItemDetail.Facilities = new List<FacilitySummary>();
                    _editedItemDetail.Facilities.AddRange(
                        new TypeSafeListWrapper<FacilitySummary>(
                            CollectionUtils.Select(_selectedFacilities, delegate(object f) { return f is FacilitySummary; })));
                    _editedItemDetail.FilterByWorkingFacility = 
                        CollectionUtils.Contains(_selectedFacilities, delegate(object f) { return f == _workingFacilityItem; });

                    _editedItemDetail.Portabilities = CollectionUtils.Map<object, bool>(_selectedPortabilities,
                        delegate(object item) { return item == _portableItem ? true : false; });

                    _editedItemDetail.Users.Clear();
                    _editedItemDetail.Users.AddRange(_selectedUsers.Items);

                    if(_startTimeChecked)
                    {
                        if (_isFixedTimeWindow)
                            _editedItemDetail.StartTime = new WorklistAdminDetail.TimePoint(_fixedStartTime, 1440);
                        else
                            _editedItemDetail.StartTime = ConvertRelativeTimeToTimePoint(_slidingStartTime);
                    }
                    else
                        _editedItemDetail.StartTime = null;

                    if(_endTimeChecked)
                    {
                        if (_isFixedTimeWindow)
                            _editedItemDetail.EndTime = new WorklistAdminDetail.TimePoint(_fixedEndTime, 1440);
                        else
                            _editedItemDetail.EndTime = ConvertRelativeTimeToTimePoint(_slidingEndTime);
                    }
                    else
                        _editedItemDetail.EndTime = null;

                    Platform.GetService<IWorklistAdminService>(
                        delegate(IWorklistAdminService service)
                        {
                            if (_isNew)
                            {
                                AddWorklistResponse response =
                                    service.AddWorklist(new AddWorklistRequest(_editedItemDetail));
                                _editedItemEntityRef = response.WorklistAdminSummary.EntityRef;
                                _editedItemSummary = response.WorklistAdminSummary;
                            }
                            else
                            {
                                UpdateWorklistResponse response =
                                    service.UpdateWorklist(new UpdateWorklistRequest(_editedItemEntityRef, _editedItemDetail));
                                _editedItemEntityRef = response.WorklistAdminSummary.EntityRef;
                                _editedItemSummary = response.WorklistAdminSummary;
                            }
                        });

                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveWorklist, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.Exit(ApplicationComponentExitCode.Error);
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        public void OnWorklistTypeChanged()
        {
            try
            {
                // Update AvailableProcedureTypeGroups
                Platform.GetService<IWorklistAdminService>(
                    delegate(IWorklistAdminService service)
                        {
                            ListProcedureTypeGroupsForWorklistCategoryRequest request = new ListProcedureTypeGroupsForWorklistCategoryRequest(_editedItemDetail.WorklistType);
                            ListProcedureTypeGroupsForWorklistCategoryResponse response =
                                service.ListProcedureTypeGroupsForWorklistCategory(request);

                            _availableProcedureTypeGroups.Items.Clear();
                            _availableProcedureTypeGroups.Items.AddRange(response.ProcedureTypeGroups);

                            foreach(ProcedureTypeGroupSummary selectedItem in _selectedProcedureTypeGroups.Items)
                            {
                                _availableProcedureTypeGroups.Items.Remove(selectedItem);
                            }

                            EventsHelper.Fire(_availableItemsChanged, this, EventArgs.Empty);
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private RelativeTime ConvertTimePointToRelativeTime(WorklistAdminDetail.TimePoint ts)
        {
            // resolution 1440 minutes per day
            if (ts.Resolution == 1440)
                return new RelativeTimeInDays(ts.RelativeTime.Value.Days);
            else
                return new RelativeTimeInHours(ts.RelativeTime.Value.Hours);
        }

        private WorklistAdminDetail.TimePoint ConvertRelativeTimeToTimePoint(RelativeTime rt)
        {
            // in days, use a resolution of days (1440 mintues per day)
            if (rt is RelativeTimeInDays)
                return new WorklistAdminDetail.TimePoint(TimeSpan.FromDays(rt.Value), 1440);

            // in hours, use real-time resolution
            // (this was a User Experience decision - that it would be more intuitive to have a real-time window, rather than nearest-hour)
            if (rt is RelativeTimeInHours)
                return new WorklistAdminDetail.TimePoint(TimeSpan.FromHours(rt.Value), 0);

            // no other types are currently implemented
            throw new NotImplementedException();
        }

        private IList SlidingTimeChoices
        {
            get
            {
                 return _slidingScale == Hours ? _slidingHourChoices : _slidingDayChoices;
            }
        }

    }
}
