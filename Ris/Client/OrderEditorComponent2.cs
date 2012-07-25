#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public partial class OrderEditorComponent
	{
		public enum Mode
		{
			NewOrder,
			ModifyOrder,
			ReplaceOrder
		}

		/// <summary>
		/// Container for default values passed from caller to the component.
		/// </summary>
		public class DefaultValues
		{
			/// <summary>
			/// Specifies the default scheduled time.
			/// </summary>
			public DateTime? ScheduledTime { get; set; }

			/// <summary>
			/// Specifies the default duration.
			/// </summary>
			public int? ScheduledDuration { get; set; }

			/// <summary>
			/// Specifies the default modality.
			/// </summary>
			public EntityRef ModalityRef { get; set; }
		}

		public abstract class OperatingContext
		{
			protected OperatingContext(Mode mode)
			{
				Mode = mode;
			}

			/// <summary>
			/// Gets the mode implied by this operating context.
			/// </summary>
			public Mode Mode { get; private set; }

			/// <summary>
			/// Specifies default values to be applied.
			/// </summary>
			public DefaultValues Defaults { get; set; }

			/// <summary>
			/// Ensure the operating context is in a valid state prior to using it.
			/// </summary>
			internal virtual void Validate()
			{
				if(this.Defaults == null)
					this.Defaults = new DefaultValues();
			}

			/// <summary>
			/// Initialize the order editor component.
			/// </summary>
			/// <param name="component"></param>
			internal abstract void Initialize(OrderEditorComponent component);

			/// <summary>
			/// Applies default values to the specified procedure requisition.
			/// </summary>
			/// <param name="procedureRequisition"></param>
			/// <param name="component"></param>
			internal virtual void ApplyDefaults(ProcedureRequisition procedureRequisition, OrderEditorComponent component)
			{
				if(!procedureRequisition.CanModify)
					return;

				if(this.Defaults.ScheduledTime.HasValue)
				{
					procedureRequisition.ScheduledTime = this.Defaults.ScheduledTime.Value;
				}

				if(this.Defaults.ScheduledDuration.HasValue)
				{
					procedureRequisition.ScheduledDuration = this.Defaults.ScheduledDuration.Value;
				}

				if(this.Defaults.ModalityRef != null)
				{
					var modality = component._modalityChoices.FirstOrDefault(m => m.ModalityRef.Equals(this.Defaults.ModalityRef, true));
					procedureRequisition.Modality = modality;
				}
			}

			/// <summary>
			/// Submit the specified order requisition to the server.
			/// </summary>
			/// <param name="requisition"></param>
			/// <param name="component"></param>
			/// <returns></returns>
			internal abstract EntityRef Submit(OrderRequisition requisition, OrderEditorComponent component);

			/// <summary>
			/// Gets a value indicating whether the patient can be modified.
			/// </summary>
			internal virtual bool CanModifyPatient
			{
				get { return false; }
			}

			/// <summary>
			/// Gets a value indicating whether the diagnostic service can be modified.
			/// </summary>
			internal virtual bool CanModifyDiagnosticService
			{
				get { return true; }
			}
		}

		public class NewOrderOperatingContext : OperatingContext
		{
			public NewOrderOperatingContext()
				:base(Mode.NewOrder)
			{
			}

			/// <summary>
			/// Specifies the patient for which to create the new order. Optional.
			/// </summary>
			public PatientProfileSummary PatientProfile { get; set; }

			internal override void Initialize(OrderEditorComponent component)
			{
				if(this.PatientProfile != null)
				{
					component.UpdatePatientProfile(this.PatientProfile);
				}
			}

			internal override EntityRef Submit(OrderRequisition requisition, OrderEditorComponent component)
			{
				PlaceOrderResponse response = null;
				Platform.GetService<IOrderEntryService>(
					service => response = service.PlaceOrder(new PlaceOrderRequest(requisition))
				);

				component.Host.ShowMessageBox(
					string.Format(
						"Order {0} placed successfully.",
						AccessionFormat.Format(response.Order.AccessionNumber)),
					MessageBoxActions.Ok);
				return response.Order.OrderRef;
			}

			internal override bool CanModifyPatient
			{
				get
				{
					// if no patient was initially provided, then obviously we need to allow user to modify patient
					return this.PatientProfile == null;
				}
			}
		}

		public class ModifyOrderOperatingContext : OperatingContext
		{
			public ModifyOrderOperatingContext()
				:base(Mode.ModifyOrder)
			{
			}

			/// <summary>
			/// Specifies the order to modify. Required if <see cref="ProcedureRef"/> is not supplied.
			/// </summary>
			public EntityRef OrderRef { get; set; }

			/// <summary>
			/// Indirectly specifies the order to modify, by specifying a procedure. Required if <see cref="OrderRef"/> is not supplied.
			/// </summary>
			public EntityRef ProcedureRef { get; set; }

			/// <summary>
			/// Specifies a filter that determines if the defaults should be applied to a given procedure. 
			/// </summary>
			public Predicate<ProcedureRequisition> DefaultValueApplicabilityFilter { get; set; }

			internal override void Validate()
			{
				if(this.OrderRef == null && this.ProcedureRef == null)
					throw new InvalidOperationException("Either OrderRef or ProcedureRef must be specified.");
				if(this.DefaultValueApplicabilityFilter == null)
					this.DefaultValueApplicabilityFilter = requisition => true;	// apply to all

				base.Validate();
			}

			internal override void Initialize(OrderEditorComponent component)
			{
				// load the existing order
				Async.Request(component,
							  (IOrderEntryService service) =>
							  service.GetOrderRequisitionForEdit(new GetOrderRequisitionForEditRequest { OrderRef = this.OrderRef, ProcedureRef = this.ProcedureRef}),
							  response => component.OnOrderRequisitionLoaded(response.Requisition, response.IsCompleted));
			}

			internal override void ApplyDefaults(ProcedureRequisition procedureRequisition, OrderEditorComponent component)
			{
				// apply the defaults only if the requisition satisfies the filter
				if (this.DefaultValueApplicabilityFilter(procedureRequisition))
				{
					base.ApplyDefaults(procedureRequisition, component);
				}
			}

			internal override EntityRef Submit(OrderRequisition requisition, OrderEditorComponent component)
			{
				ModifyOrderResponse response = null;
				requisition.OrderRef = component.OrderRef;
				Platform.GetService<IOrderEntryService>(service =>
				{
					response = service.ModifyOrder(new ModifyOrderRequest(requisition));
				});
				return response.Order.OrderRef;
			}

			internal override bool CanModifyDiagnosticService
			{
				get { return false; }
			}
		}

		public class ReplaceOrderOperatingContext : OperatingContext
		{
			public ReplaceOrderOperatingContext()
				:base(Mode.ReplaceOrder)
			{
			}

			/// <summary>
			/// Specifies the order to modify. Required.
			/// </summary>
			public EntityRef OrderRef { get; set; }

			internal override void Validate()
			{
				if(this.OrderRef == null)
					throw new InvalidOperationException("OrderRef must be specified.");

				base.Validate();
			}

			internal override void Initialize(OrderEditorComponent component)
			{
				// load the existing order
				Async.Request(component,
							  (IOrderEntryService service) =>
							  service.GetOrderRequisitionForEdit(new GetOrderRequisitionForEditRequest { OrderRef = this.OrderRef }),
							  response =>
							  	{
							  		component.OnOrderRequisitionLoaded(response.Requisition, response.IsCompleted);
									// bug #3506: in replace mode, overwrite the procedures with clean one(s) based on diagnostic service
									component.UpdateDiagnosticService(response.Requisition.DiagnosticService);
							  		this.OrderRef = component.OrderRef;
							  	});
			}

			internal override EntityRef Submit(OrderRequisition requisition, OrderEditorComponent component)
			{
				ReplaceOrderResponse response = null;
				Platform.GetService<IOrderEntryService>(
					service => response = service.ReplaceOrder(new ReplaceOrderRequest(this.OrderRef, component.SelectedCancelReason, requisition))
				);

				component.Host.ShowMessageBox(
					string.Format("Order successfully replaced with new order {0}.", AccessionFormat.Format(response.Order.AccessionNumber)),
					MessageBoxActions.Ok);

				return response.Order.OrderRef;
			}
		}
	}
}
