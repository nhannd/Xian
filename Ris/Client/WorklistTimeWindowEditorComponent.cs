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
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistTimeWindowEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistTimeWindowEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistTimeWindowEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistTimeWindowEditorComponentViewExtensionPoint))]
    public class WorklistTimeWindowEditorComponent : ApplicationComponent
    {
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

        private static readonly object Hours = new DummyItem(SR.DummyItemHours);
        private static readonly object Days = new DummyItem(SR.DummyItemDays);

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

        private WorklistAdminDetail _worklistDetail;

		private bool _isFixedTimeWindow;
		private bool _startTimeChecked;
        private bool _endTimeChecked;
        private RelativeTime _slidingStartTime;
        private RelativeTime _slidingEndTime;
        private DateTime _fixedStartTime;
        private DateTime _fixedEndTime;
        private object _slidingScale;
       
        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistTimeWindowEditorComponent(WorklistAdminDetail detail)
        {
            _worklistDetail = detail;
        }

        public override void Start()
        {
            // init both fixed and sliding times to "now"
            _fixedStartTime = _fixedEndTime = Platform.Time;
            _slidingStartTime = _slidingEndTime = new RelativeTimeInDays(0);

            if (_worklistDetail.StartTime != null)
            {
                _startTimeChecked = true;
                if (_worklistDetail.StartTime.FixedTime != null)
                {
                    _fixedStartTime = _worklistDetail.StartTime.FixedTime.Value;
                    _isFixedTimeWindow = true;
                }

                if (_worklistDetail.StartTime.RelativeTime != null)
                {
                    _slidingStartTime = ConvertTimePointToRelativeTime(_worklistDetail.StartTime);
                }
            }

            if (_worklistDetail.EndTime != null)
            {
                _endTimeChecked = true;
                if (_worklistDetail.EndTime.FixedTime != null)
                {
                    _fixedEndTime = _worklistDetail.EndTime.FixedTime.Value;
                    _isFixedTimeWindow = true;
                }

                if (_worklistDetail.EndTime.RelativeTime != null)
                {
                    _slidingEndTime = ConvertTimePointToRelativeTime(_worklistDetail.EndTime);
                }
            }

            if (_slidingStartTime is RelativeTimeInHours || _slidingEndTime is RelativeTimeInHours)
                _slidingScale = Hours;
            else
                _slidingScale = Days;

            this.Validation.Add(new ValidationRule("SlidingEndTime",
                delegate
                {
					// this rule only applies if user has selected a sliding time window
					// only need to validate the time difference if both Start and End are specified
					if(!_isFixedTimeWindow && _startTimeChecked && _endTimeChecked)
					{
						int i = SlidingEndTime.CompareTo(SlidingStartTime);

						// if the scale is Hours, then the end-time must be greater than start-time
						// if the scale is Days, then the end-time must be greater than or equal to start-time
						bool ok = _slidingScale == Hours ? (i > 0) : (i > -1);
						return new ValidationResult(ok, _slidingScale == Hours ? SR.MessageEndTimeMustBeGreaterThanStartTime
							: SR.MessageEndTimeMustBeGreaterOrEqualStartTime);
					}
					else
					{
						// rule not applicable
						return new ValidationResult(true, "");
					}
                }));

            base.Start();
        }

        #region Presentation Model

        public bool IsFixedTimeWindow
        {
            get { return _isFixedTimeWindow; }
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
                if (value != _startTimeChecked)
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
                if (_fixedStartTime != value)
                {
                    _fixedStartTime = value;
                    this.Modified = true;
                }
            }
        }

        [ValidateGreaterThan("FixedStartTime", Inclusive = true, Message = "MessageEndTimeMustBeGreaterOrEqualStartTime")]
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
                if (value != _slidingScale)
                {
                    _slidingScale = value;

                    // if validation is enabled, temporarily disable it, otherwise we get exceptions from transient comparisons
                    bool validationVisible = this.ValidationVisible;
                    ShowValidation(false);

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

                    // re-enable validation if enabled
                    ShowValidation(validationVisible);
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
                if (!Equals(value, _slidingStartTime))
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

        #endregion

        internal void SaveData()
        {
            if (_startTimeChecked)
            {
                if (_isFixedTimeWindow)
                    _worklistDetail.StartTime = new WorklistAdminDetail.TimePoint(_fixedStartTime, 1440);
                else
                    _worklistDetail.StartTime = ConvertRelativeTimeToTimePoint(_slidingStartTime);
            }
            else
                _worklistDetail.StartTime = null;

            if (_endTimeChecked)
            {
                if (_isFixedTimeWindow)
                    _worklistDetail.EndTime = new WorklistAdminDetail.TimePoint(_fixedEndTime, 1440);
                else
                    _worklistDetail.EndTime = ConvertRelativeTimeToTimePoint(_slidingEndTime);
            }
            else
                _worklistDetail.EndTime = null;
        }

        private static RelativeTime ConvertTimePointToRelativeTime(WorklistAdminDetail.TimePoint ts)
        {
            // resolution 1440 minutes per day
            if (ts.Resolution == 1440)
                return new RelativeTimeInDays(ts.RelativeTime.Value.Days);
            else
                return new RelativeTimeInHours(ts.RelativeTime.Value.Hours);
        }

        private static WorklistAdminDetail.TimePoint ConvertRelativeTimeToTimePoint(RelativeTime rt)
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
