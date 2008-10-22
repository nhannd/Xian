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

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Protocol entity
	/// </summary>
	public partial class Protocol : ClearCanvas.Enterprise.Core.Entity
	{
		public Protocol(Procedure procedure)
			: this()
		{
			_procedures.Add(procedure);
			procedure.Protocol = this;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public virtual void Accept()
		{
			_status = ProtocolStatus.PR;
		}

		public virtual void Reject(ProtocolRejectReasonEnum reason)
		{
			_status = ProtocolStatus.RJ;
			_rejectReason = reason;
		}

		public virtual void Resolve()
		{
			_status = ProtocolStatus.PN;
			_rejectReason = null;
		}

		public virtual void SubmitForApproval()
		{
			_status = ProtocolStatus.AA;
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		protected internal virtual void TimeShift(int minutes)
		{
			// no times to shift
		}

		/// <summary>
		/// Links a <see cref="Procedure"/> to this report, meaning that the protocol covers
		/// this radiology procedure.
		/// </summary>
		/// <param name="procedure"></param>
		protected internal virtual void LinkProcedure(Procedure procedure)
		{
			if (_procedures.Contains(procedure))
				throw new WorkflowException("The procedure is already associated with this protocol.");

			// does the procedure already have a non-new protocol?
			Protocol otherProtocol = procedure.Protocol;
			if (otherProtocol.IsNew() == false && !this.Equals(otherProtocol))
				throw new WorkflowException("Cannot link this procedure because it already has an active protocol.");

			_procedures.Add(procedure);
			procedure.Protocol = this;
		}

		protected internal virtual bool IsNew()
		{
			if (_status != ProtocolStatus.PN)
				return false;
			if (_author != null)
				return false;
			if (!_codes.IsEmpty)
				return false;
			return true;
		}
	}
}