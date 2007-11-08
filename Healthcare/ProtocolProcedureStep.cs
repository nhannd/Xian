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

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public class ProtocolProcedureStep : ProcedureStep
    {
        private Protocol _protocol;

        #region Constructors

        public ProtocolProcedureStep(Protocol protocol)
        {
            _protocol = protocol;
        }

        /// <summary>
        /// Default no-args constructor required by nHibernate
        /// </summary>
        protected ProtocolProcedureStep()
        {
        }

        #endregion

        public Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        #region ProcedureStep overrides

        public override string Name
        {
            get { return "Protocol"; }
        }

        #endregion

        public override void Suspend()
        {
            if(this.State == ActivityStatus.SU && this.Protocol.Status == ProtocolStatus.SU)
            {
                // Assume Suspended to Rejected transition, which is okay
                return;
            }
            base.Suspend();
        }

        public virtual bool CanAccept
        {
            get { return this.State == ActivityStatus.SC || this.State == ActivityStatus.IP || (this.State == ActivityStatus.SU && this.Protocol.Status == ProtocolStatus.SU); }
        }

        public virtual bool CanReject
        {
            get { return this.State == ActivityStatus.SC || this.State == ActivityStatus.IP || (this.State == ActivityStatus.SU && this.Protocol.Status == ProtocolStatus.SU); }
        }

        public virtual bool CanSuspend
        {
            get { return this.State == ActivityStatus.SC || this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSave
        {
            get { return this.State == ActivityStatus.SC || this.State == ActivityStatus.IP || (this.State == ActivityStatus.SU && this.Protocol.Status == ProtocolStatus.SU); }
        }

        public virtual bool IsRejected
        {
            get { return this.State == ActivityStatus.SU && this.Protocol.Status == ProtocolStatus.RJ; }
        }

        public virtual bool IsSuspended
        {
            get { return this.State == ActivityStatus.SU && this.Protocol.Status == ProtocolStatus.SU; }
        }

        #region Object overrides

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
