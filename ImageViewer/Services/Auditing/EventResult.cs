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
	/// Represents the result of a particular auditable event.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventResult"/> has a 1-to-1 mapping with a <see cref="EventIdentificationTypeEventOutcomeIndicator"/>,
	/// but <see cref="AuditHelper"/> uses <see cref="EventResult"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventResult
	{
		/// <summary>
		/// The auditable event completed successfully.
		/// </summary>
		public static readonly EventResult Success = new EventResult(EventIdentificationTypeEventOutcomeIndicator.Success);

		/// <summary>
		/// The auditable event finished with minor errors.
		/// </summary>
		public static readonly EventResult MinorFailure = new EventResult(EventIdentificationTypeEventOutcomeIndicator.MinorFailureActionRestarted);

		/// <summary>
		/// The auditable event finished with major errors.
		/// </summary>
		public static readonly EventResult MajorFailure = new EventResult(EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable);

		/// <summary>
		/// The auditable event finished with serious errors.
		/// </summary>
		public static readonly EventResult SeriousFailure = new EventResult(EventIdentificationTypeEventOutcomeIndicator.SeriousFailureActionTerminated);

		private readonly EventIdentificationTypeEventOutcomeIndicator _outcome;

		private EventResult(EventIdentificationTypeEventOutcomeIndicator outcome)
		{
			_outcome = outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventIdentificationTypeEventOutcomeIndicator"/>.
		/// </summary>
		public static implicit operator EventIdentificationTypeEventOutcomeIndicator(EventResult operand)
		{
			return operand._outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventResult"/>.
		/// </summary>
		public static implicit operator EventResult(EventIdentificationTypeEventOutcomeIndicator operand)
		{
			switch (operand)
			{
				case EventIdentificationTypeEventOutcomeIndicator.Success:
					return Success;
				case EventIdentificationTypeEventOutcomeIndicator.MinorFailureActionRestarted:
					return MinorFailure;
				case EventIdentificationTypeEventOutcomeIndicator.SeriousFailureActionTerminated:
					return SeriousFailure;
				case EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable:
					return MajorFailure;
				default:
					return null;
			}
		}
	}
}