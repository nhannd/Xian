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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    public abstract class ProtocolProcedureStep : ProcedureStep
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

        public override string Name
        {
            get { return "Protocol"; }
        }

        public override bool IsPreStep
        {
            get
            {
                // occurs prior to the actual procedure
                return true;
            }
        }

		public override List<Procedure> GetLinkedProcedures()
		{
			if (_protocol != null)
			{
				return CollectionUtils.Select(_protocol.Procedures,
					delegate(Procedure p) { return !Equals(p, this.Procedure); });
			}
			else
			{
				return new List<Procedure>();
			}
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// can't have relatives if no protocol
			if (this.Protocol == null)
				return false;

			// relatives must be protocol steps
			if (!step.Is<ProtocolProcedureStep>())
				return false;

			// check if tied to same protocol
			ProtocolProcedureStep that = step.As<ProtocolProcedureStep>();
			return that.Protocol != null && Equals(this.Protocol, that.Protocol);
		}
	}
}
