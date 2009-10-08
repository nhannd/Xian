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

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Order entity
	/// </summary>
	public partial class Order
	{
		#region Static Factory methods

		/// <summary>
		/// Factory method to create a new order.
		/// </summary>
		public static Order NewOrder(OrderCreationArgs args)
		{
			// validate required members are set
			Platform.CheckMemberIsSet(args.Patient, "Patient");
			Platform.CheckMemberIsSet(args.Visit, "Visit");
			Platform.CheckMemberIsSet(args.AccessionNumber, "AccessionNumber");
			Platform.CheckMemberIsSet(args.DiagnosticService, "DiagnosticService");
			Platform.CheckMemberIsSet(args.ReasonForStudy, "ReasonForStudy");
			Platform.CheckMemberIsSet(args.OrderingFacility, "OrderingFacility");
			Platform.CheckMemberIsSet(args.OrderingPractitioner, "OrderingPractitioner");


			// create the order
			var order = new Order
			{
				Patient = args.Patient,
				Visit = args.Visit,
				AccessionNumber = args.AccessionNumber,
				DiagnosticService = args.DiagnosticService,
				ReasonForStudy = args.ReasonForStudy,
				OrderingFacility = args.OrderingFacility,
				OrderingPractitioner = args.OrderingPractitioner,
				Priority = args.Priority,
				SchedulingRequestTime = args.SchedulingRequestTime,
				EnteredTime = args.EnteredTime,
				EnteredBy = args.EnteredBy,
				EnteredComment = args.EnteredComment
			};

			if (args.Procedures == null || args.Procedures.Count == 0)
			{
				// create procedures according to the diagnostic service plan
				args.Procedures = CollectionUtils.Map<ProcedureType, Procedure>(
					args.DiagnosticService.ProcedureTypes,
					type => new Procedure(type)
								{
									PerformingFacility = args.PerformingFacility ?? args.OrderingFacility
								});
			}


			// associate all procedures with the order
			foreach (var procedure in args.Procedures)
			{
				order.AddProcedure(procedure);
			}

			// add recipients
			if (args.ResultRecipients != null)
			{
				foreach (var recipient in args.ResultRecipients)
				{
					order.ResultRecipients.Add(recipient);
				}
			}

			var recipientsContainsOrderingPractitioner = CollectionUtils.Contains(
				order.ResultRecipients,
				r => r.PractitionerContactPoint.Practitioner.Equals(args.OrderingPractitioner));

			// if the result recipients collection does not contain the ordering practitioner, add it by force, using the default contact point
			if (!recipientsContainsOrderingPractitioner)
			{
				// find the default
				var defaultContactPoint =
					CollectionUtils.SelectFirst(args.OrderingPractitioner.ContactPoints, cp => cp.IsDefaultContactPoint)
					// if no default, use first available
					?? CollectionUtils.FirstElement(args.OrderingPractitioner.ContactPoints);

				if (defaultContactPoint != null)
				{
					order.ResultRecipients.Add(new ResultRecipient(defaultContactPoint, ResultCommunicationMode.ANY));
				}
			}

			return order;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets a value indicating whether this order is in a terminal state.
		/// </summary>
		public virtual bool IsTerminated
		{
			get
			{
				return _status == OrderStatus.CM || _status == OrderStatus.CA || _status == OrderStatus.DC || _status == OrderStatus.RP;
			}
		}

		/// <summary>
		/// Gets a value indicating whether all active (i.e. non terminated) procedures in this order are performed.
		/// </summary>
		public virtual bool AreAllActiveProceduresPerformed
		{
			get
			{
				return CollectionUtils.TrueForAll(
					CollectionUtils.Select(this.Procedures, p => !p.IsTerminated),
					p => p.IsPerformed);
			}
		}

		#endregion

		#region Public operations

		/// <summary>
		/// Adds the specified procedure to this order.
		/// </summary>
		/// <param name="procedure"></param>
		public virtual void AddProcedure(Procedure procedure)
		{
			if (procedure.Order != null || procedure.Status != ProcedureStatus.SC)
				throw new ArgumentException("Only new Procedure objects may be added to an order.");
			if (this.IsTerminated)
				throw new WorkflowException(string.Format("Cannot add procedure to order with status {0}.", _status));

			procedure.Order = this;

			// generate an index for the procedure
			var highestIndex = CollectionUtils.Max(
				CollectionUtils.Map<Procedure, int>(_procedures, p => int.Parse(p.Index)), 0);
			procedure.Index = (highestIndex + 1).ToString();

			// add to collection
			_procedures.Add(procedure);

			// update scheduling information
			UpdateScheduling();
		}

		/// <summary>
		/// Removes the specified procedure from this order.
		/// </summary>
		/// <param name="procedure"></param>
		public virtual void RemoveProcedure(Procedure procedure)
		{
			if (!_procedures.Contains(procedure))
				throw new ArgumentException("Specified procedure does not exist for this order.");
			if (procedure.Status != ProcedureStatus.SC)
				throw new WorkflowException("Only procedures in the SC status can be removed from an order.");

			_procedures.Remove(procedure);
			procedure.Order = null;
		}

		/// <summary>
		/// Schedules all procedures in this order for the specified start time.
		/// </summary>
		/// <param name="startTime"></param>
		public virtual void Schedule(DateTime? startTime)
		{
			foreach (var procedure in _procedures)
			{
				procedure.Schedule(startTime);
			}
		}

		/// <summary>
		/// Cancels the order.
		/// </summary>
		/// <param name="cancelInfo"></param>
		public virtual void Cancel(OrderCancelInfo cancelInfo)
		{
			if (this.Status != OrderStatus.SC)
				throw new WorkflowException("Only orders in the SC status can be canceled");

			_cancelInfo = cancelInfo;

			// update the status prior to cancelling the procedures
			// (otherwise cancelling the procedures will cause them to try and update the order status)
			//SetStatus(OrderStatus.CA);

			// cancel/discontinue all procedures
			foreach (var procedure in _procedures)
			{
				// given that the order is still in SC, all procedures must be either
				// SC or CA - and only those in SC need to be cancelled
				if (procedure.Status == ProcedureStatus.SC)
					procedure.Cancel();
			}

			// if the order was replaced, change the status to RP
			if (cancelInfo.ReplacementOrder != null)
				SetStatus(OrderStatus.RP);

			// need to update the end-time again, after cacnelling procedures
			UpdateEndTime();
		}

		/// <summary>
		/// Discontinues the order.
		/// </summary>
		public virtual void Discontinue(OrderCancelInfo cancelInfo)
		{
			if (this.Status != OrderStatus.IP)
				throw new WorkflowException("Only orders in the IP status can be discontinued");

			_cancelInfo = cancelInfo;

			// update the status prior to cancelling the procedures
			// (otherwise cancelling the procedures will cause them to try and update the order status)
			SetStatus(OrderStatus.DC);

			// cancel or discontinue any non-terminated procedures
			foreach (var procedure in _procedures)
			{
				if (procedure.Status == ProcedureStatus.SC)
					procedure.Cancel();
				else if (procedure.Status == ProcedureStatus.IP)
					procedure.Discontinue();
			}

			// need to update the end-time again, after discontinuing procedures
			UpdateEndTime();
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.  Calling this method on a
		/// <see cref="Order"/> will also shift all child objects (Procedures, ProcedureSteps, 
		/// Reports, ProcedureCheckIn, Protocol).  The <see cref="Visit"/> is not shifted, because
		/// it is not considered a child of the order.
		/// </para>
		/// <para>
		/// Typically this method is called after the order is performed and the report(s) is 
		/// created and published, so that the entire order and all resulting documentation is
		/// shifted in time by the same amount.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_enteredTime = _enteredTime.AddMinutes(minutes);
			_schedulingRequestTime = _schedulingRequestTime.HasValue ? _schedulingRequestTime.Value.AddMinutes(minutes) : _schedulingRequestTime;
			_scheduledStartTime = _scheduledStartTime.HasValue ? _scheduledStartTime.Value.AddMinutes(minutes) : _scheduledStartTime;
			_startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
			_endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;

			foreach (var procedure in _procedures)
			{
				procedure.TimeShift(minutes);
			}
		}

		#endregion

		#region Helper methods

		/// <summary>
		/// Called by a child procedure to tell the order to update its scheduling information.
		/// </summary>
		protected internal virtual void UpdateScheduling()
		{
			// set the scheduled start time to the earliest non-null scheduled start time of any child procedure
			_scheduledStartTime = MinMaxHelper.MinValue(
				_procedures,
				delegate { return true; },
				procedure => procedure.ScheduledStartTime,
				null);
		}

		/// <summary>
		/// Called by a child procedure to tell the order to update its status.  Only
		/// certain status updates can be inferred deterministically from child statuses.  If no
		/// status can be inferred, the status does not change.
		/// </summary>
		protected internal virtual void UpdateStatus()
		{
			// if the order has not yet terminated, it may need to be auto-terminated
			if (!IsTerminated)
			{
				// if all rp are cancelled, the order is cancelled
				if (CollectionUtils.TrueForAll(_procedures, procedure => procedure.Status == ProcedureStatus.CA))
				{
					SetStatus(OrderStatus.CA);
				}
				else
					// if all rp are cancelled or discontinued, the order is discontinued
					if (CollectionUtils.TrueForAll(_procedures, procedure => procedure.Status == ProcedureStatus.CA || procedure.Status == ProcedureStatus.DC))
					{
						SetStatus(OrderStatus.DC);
					}
					else
						// if all rp are cancelled, discontinued or completed, then the order is completed
						if (CollectionUtils.TrueForAll(_procedures, procedure => procedure.IsTerminated))
						{
							SetStatus(OrderStatus.CM);
						}
			}

			// if the order is still scheduled, it may need to be auto-started
			if (_status == OrderStatus.SC)
			{
				if (CollectionUtils.Contains(_procedures, procedure => procedure.Status == ProcedureStatus.IP || procedure.Status == ProcedureStatus.CM))
				{
					SetStatus(OrderStatus.IP);
				}
			}
		}

		private void SetStatus(OrderStatus status)
		{
			if (_status == status)
				return;

			_status = status;

			if (_status == OrderStatus.IP)
				UpdateStartTime();

			if (this.IsTerminated)
				UpdateEndTime();
		}

		private void UpdateStartTime()
		{
			// compute the earliest procedure start time
			_startTime = MinMaxHelper.MinValue(
				_procedures,
				delegate { return true; },
				procedure => procedure.StartTime,
				null);
		}

		private void UpdateEndTime()
		{
			// compute the latest procedure end time
			_endTime = MinMaxHelper.MaxValue(
				_procedures,
				delegate { return true; },
				procedure => procedure.EndTime,
				null);
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		#endregion
	}
}
