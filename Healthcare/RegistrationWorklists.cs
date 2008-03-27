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

using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
    public abstract class RegistrationWorklist : Worklist
    {
        public override IList GetWorklistItems(IWorklistQueryContext wqc)
        {
            return (IList)wqc.GetBroker<IRegistrationWorklistItemBroker>().GetWorklistItems(this, wqc);
        }

        public override int GetWorklistItemCount(IWorklistQueryContext wqc)
        {
            return wqc.GetBroker<IRegistrationWorklistItemBroker>().CountWorklistItems(this, wqc);
        }

        public override Type ProcedureStepType
        {
            get { return null; }
        }
    }

    /// <summary>
    /// RegistrationToBeScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationToBeScheduledWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationToBeScheduledWorklist : RegistrationWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.Order.Status.In(new OrderStatus[] { OrderStatus.SC });

            // only unscheduled items should appear in this list
            criteria.Procedure.ScheduledStartTime.IsNull();
            ApplyTimeRange(criteria.Order.SchedulingRequestTime, null);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name="RegistrationScheduledWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationScheduledWorklist : RegistrationWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.Order.Status.EqualTo(OrderStatus.SC);
            criteria.ProcedureCheckIn.CheckInTime.IsNull();     // exclude anything already checked-in
            ApplyTimeRange(criteria.Procedure.ScheduledStartTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationCheckedInWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationCheckedInWorklist : RegistrationWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.Order.Status.EqualTo(OrderStatus.SC);
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull();  // include only items that have been checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull();
            ApplyTimeRange(criteria.ProcedureCheckIn.CheckInTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationInProgressWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationInProgressWorklist : RegistrationWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.Order.Status.EqualTo(OrderStatus.IP);
            criteria.ProcedureCheckIn.CheckOutTime.IsNull();    // exclude any item already checked-out
            ApplyTimeRange(criteria.Procedure.StartTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationCompletedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationCompletedWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationCompletedWorklist : RegistrationWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            // "completed" in this context just means the patient has checked-out
            // the order may still be in progress
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.Order.Status.In(new OrderStatus[] { OrderStatus.IP, OrderStatus.CM });
            criteria.ProcedureCheckIn.CheckOutTime.IsNotNull();
            ApplyTimeRange(criteria.ProcedureCheckIn.CheckOutTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// RegistrationCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationCancelledWorklist")]
    [WorklistSupportsTimeFilter(true)]
    public class RegistrationCancelledWorklist : RegistrationWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
            criteria.Order.Status.In(new OrderStatus[] { OrderStatus.DC, OrderStatus.CA });

            // apply filter to the end-time (time procedure was was cancelled)
            ApplyTimeRange(criteria.Procedure.EndTime, WorklistTimeRange.Today);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}