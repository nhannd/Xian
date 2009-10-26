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
using System.Text;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="CheckInOrderComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CheckInOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CheckInOrderComponent class
	/// </summary>
	[AssociateView(typeof(CheckInOrderComponentViewExtensionPoint))]
	public class CheckInOrderComponent : ApplicationComponent
	{
		private readonly RegistrationWorklistItem _worklistItem;
		private CheckInOrderTable _checkInOrderTable;
		private DateTime _checkInTime;

		/// <summary>
		/// Constructor
		/// </summary>
		public CheckInOrderComponent(RegistrationWorklistItem item)
		{
			_worklistItem = item;
		}

		public override void Start()
		{
			_checkInOrderTable = new CheckInOrderTable();
			_checkInTime = Platform.Time;

			Platform.GetService(
				delegate(IRegistrationWorkflowService service)
				{
					var response = service.ListProceduresForCheckIn(new ListProceduresForCheckInRequest(_worklistItem.OrderRef));
					_checkInOrderTable.Items.AddRange(
						CollectionUtils.Map(response.Procedures,
								delegate(ProcedureSummary item)
								{
									var entry = new CheckInOrderTableEntry(item);
									entry.CheckedChanged += OrderCheckedStateChangedEventHandler;
									return entry;
								}));
				});

			base.Start();
		}

		#region Presentation Model

		public ITable OrderTable
		{
			get { return _checkInOrderTable; }
		}

		public DateTime CheckInTime
		{
			get { return _checkInTime; }
			set { _checkInTime = value; }
		}

		public bool CheckInTimeVisible
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode; }
		}

		public bool AcceptEnabled
		{
			get { return CollectionUtils.Contains(_checkInOrderTable.Items, entry => entry.Checked); }
		}

		#endregion

		public void Accept()
		{
			var earlyProcedures = new List<ProcedureSummary>();
			var lateProcedures = new List<ProcedureSummary>();
			var checkedProcedureRefs = new List<EntityRef>();

			// Get the list of Order EntityRef from the table
			foreach (var entry in _checkInOrderTable.Items)
			{
				if (!entry.Checked)
					continue;

				checkedProcedureRefs.Add(entry.Procedure.ProcedureRef);

				string checkInValidationMessage;
				var result = CheckInSettings.Validate(entry.Procedure.ScheduledStartTime, _checkInTime, out checkInValidationMessage);
				switch (result)
				{
					case CheckInSettings.ValidateResult.CheckingInTooEarly:
						earlyProcedures.Add(entry.Procedure);
						break;
					case CheckInSettings.ValidateResult.CheckingInTooLate:
						lateProcedures.Add(entry.Procedure);
						break;
					default:
						break;
				}
			}

			if (earlyProcedures.Count > 0 || lateProcedures.Count > 0)
			{
				var earlyThreshold = TimeSpan.FromMinutes(CheckInSettings.Default.EarlyCheckInWarningThreshold);
				var lateThreshold = TimeSpan.FromMinutes(CheckInSettings.Default.LateCheckInWarningThreshold);


				var messageBuilder = new StringBuilder();
				messageBuilder.AppendFormat(SR.MessageCheckInProceduresTooLateOrTooEarly,
					TimeSpanFormat.FormatDescriptive(earlyThreshold),
					TimeSpanFormat.FormatDescriptive(lateThreshold));
				messageBuilder.AppendLine();
				messageBuilder.AppendLine();
				CollectionUtils.ForEach(earlyProcedures, procedure => messageBuilder.AppendLine(ProcedureFormat.Format(procedure)));
				CollectionUtils.ForEach(lateProcedures, procedure => messageBuilder.AppendLine(ProcedureFormat.Format(procedure)));
				messageBuilder.AppendLine();
				messageBuilder.Append(SR.MessageConfirmCheckInProcedures);

				if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.YesNo))
				{
					return;
				}
			}

			try
			{
				Platform.GetService( (IRegistrationWorkflowService service) => service.CheckInProcedure(
					new CheckInProcedureRequest(checkedProcedureRefs, 
						DowntimeRecovery.InDowntimeRecoveryMode ? (DateTime?) _checkInTime : null)));

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		private void OrderCheckedStateChangedEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("AcceptEnabled");
		}
	}
}
