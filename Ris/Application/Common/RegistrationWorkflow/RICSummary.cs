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
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class RICSummary : DataContractBase
    {
        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public PersonNameDetail OrderingPractitioner;

        [DataMember]
        public string Insurance;

        [DataMember]
        public DateTime? ScheduledTime;

        [DataMember]
        public string OrderingFacility;

        [DataMember]
        public string Status;

        public static Comparison<RICSummary> ActiveScheduledTimeComparer
        {
            get
            {
                return new Comparison<RICSummary>(
                    delegate(RICSummary s1, RICSummary s2)
                    {
                        int compareResult = CompreMoreRecent(s1.ScheduledTime, s2.ScheduledTime);
                        return compareResult;
                    });
            }
        }

        /// <summary>
        /// Compare whether time1 is more recent than time2.
        /// Time of null is considered unscheduled and furthest into the future.
        /// Any time after today is considered more recent than the past.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns>0 if both time are equal, -1 if time1 is more recent and 1 if time2 is more recent</returns>
        public static int CompreMoreRecent(DateTime? time1, DateTime? time2)
        {
            if (time1 == null && time2 == null)
                return 0;

            DateTime today = Platform.Time.Date;
            bool timeOneMoreRecent = false;

            if (time1 == null)
            {
                if (time2.Value.CompareTo(today) < 0)
                    timeOneMoreRecent = true;  // time1 in the future, time2 in the past
            }
            else if (time2 == null)
            {
                if (time1.Value.CompareTo(today) < 0)
                    timeOneMoreRecent = false;  // time1 in the past, time2 in the future
            }
            else
            {
                if (time1.Value == time2.Value)
                    return 0;

                long timeOneSpan = time1.Value.Subtract(today).Ticks;
                long timeTwoSpan = time2.Value.Subtract(today).Ticks;

                if (timeOneSpan > 0 && timeTwoSpan > 0)
                {
                    // Both in the future
                    timeOneMoreRecent = timeOneSpan < timeTwoSpan;
                }
                else if (timeOneSpan < 0 && timeTwoSpan < 0)
                {
                    // Both in the past
                    timeOneMoreRecent = timeOneSpan > timeTwoSpan;
                }
                else
                {
                    timeOneMoreRecent = timeOneSpan > timeTwoSpan;
                }
            }

            return timeOneMoreRecent ? -1 : 1;
        }

    }
}
