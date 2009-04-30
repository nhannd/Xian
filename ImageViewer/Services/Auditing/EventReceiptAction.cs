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

using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Represents the action taken by the application entity upon receiving a transfer of DICOM instances.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventReceiptAction"/> has a 1-to-1 mapping with a <see cref="EventIdentificationTypeEventActionCode"/>,
	/// but <see cref="AuditHelper"/> uses <see cref="EventReceiptAction"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventReceiptAction
	{
		/// <summary>
		/// The device does not already have these instances, and hence created new ones.
		/// </summary>
		public static readonly EventReceiptAction CreateNew = new EventReceiptAction(EventIdentificationTypeEventActionCode.C);

		/// <summary>
		/// The device already has these instances, has determined them to be no different from the arriving ones, and hence did not perform any action.
		/// </summary>
		public static readonly EventReceiptAction KeepExisting = new EventReceiptAction(EventIdentificationTypeEventActionCode.R);

		/// <summary>
		/// The device already has these instances, has determined them to be different from the arriving ones, and hence updated the existing ones.
		/// </summary>
		public static readonly EventReceiptAction UpdateExisting = new EventReceiptAction(EventIdentificationTypeEventActionCode.U);

		/// <summary>
		/// The action that the receiving device took is unknown.
		/// </summary>
		public static readonly EventReceiptAction ActionUnknown = new EventReceiptAction(EventIdentificationTypeEventActionCode.E);

		private readonly EventIdentificationTypeEventActionCode _action;

		private EventReceiptAction(EventIdentificationTypeEventActionCode action)
		{
			_action = action;
		}

		public static implicit operator EventIdentificationTypeEventActionCode(EventReceiptAction operand)
		{
			return operand._action;
		}
	}
}