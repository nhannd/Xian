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

using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// RegistrationCompletedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name="RegistrationCompletedWorklist")]
    public partial class RegistrationCompletedWorklist : ClearCanvas.Healthcare.Worklist
    {
        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        private RegistrationWorklistItemSearchCriteria[] QueryConditions
        {
            get
            {
                RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
                criteria.Order.Status.In( new OrderStatus[] {OrderStatus.IP, OrderStatus.CM} );
                criteria.RequestedProcedure.ScheduledStartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                criteria.ProcedureCheckIn.CheckOutTime.IsNotNull();
                return new RegistrationWorklistItemSearchCriteria[] { criteria };
            }
        }

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IRegistrationWorklistBroker>(context).GetWorklist(QueryConditions, this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetWorklistCount(QueryConditions, this);
        }

        public override string NameSuffix
        {
            get { return " - Completed"; }
        }

        #region Object overrides

        public override bool Equals(object that)
        {
            // TODO: implement a test for business-key equality
            return base.Equals(that);
        }

        public override int GetHashCode()
        {
            // TODO: implement a hash-code based on the business-key used in the Equals() method
            return base.GetHashCode();
        }

        #endregion
    }
}