#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;



namespace ClearCanvas.Healthcare.Owls
{
	using UpdateViewItemDelegate = UpdateViewItemDelegate<WorklistViewItemBase>;


	/// <summary>
	/// Base class for all worklist view items.
	/// </summary>
	// note: View items are not published in change sets, and not validated.
	[PublishInChangeSets(false)]
	[Validation(EnableValidation = false)]
	public partial class WorklistViewItemBase
	{

		private static readonly Dictionary<WorklistItemField, UpdateViewItemDelegate> _fieldMappings
			= new Dictionary<WorklistItemField, UpdateViewItemDelegate>();

		/// <summary>
		/// Class constructor.
		/// </summary>
		static WorklistViewItemBase()
		{
			_fieldMappings.Add(WorklistItemField.PatientProfile,
				(item, value, updateReferences) => item.SetPatientProfileInfo((PatientProfile)value));

			_fieldMappings.Add(WorklistItemField.Visit,
				(item, value, updateReferences) => item.SetVisitInfo((Visit)value));

			_fieldMappings.Add(WorklistItemField.Order,
				(item, value, updateReferences) => item.SetOrderInfo((Order)value, updateReferences));

			_fieldMappings.Add(WorklistItemField.Procedure,
				(item, value, updateReferences) => item.SetProcedureInfo((Procedure)value, updateReferences));
		}

		/// <summary>
		/// Gets the tuple mapping for the specified projection.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public TupleMappingDelegate GetTupleMapping(WorklistItemProjection projection)
		{
			var mapping = new UpdateViewItemDelegate[projection.Fields.Count];
			for (var i = 0; i < projection.Fields.Count; i++)
			{
				mapping[i] = GetFieldUpdater(projection.Fields[i]);
			}

			return (viewItem, tuple, persistenceContext) =>
				((WorklistViewItemBase) viewItem).InitializeFromTuple(tuple, mapping, persistenceContext);
		}

		public bool IsPatientProfileAligned()
		{
			return _patientProfile.Mrn.AssigningAuthority.Equals(_procedure.PerformingFacility.InformationAuthority);
		}

		/// <summary>
		/// Sets the patient profile component of this view item.
		/// </summary>
		/// <param name="profile"></param>
		public virtual void SetPatientProfileInfo(PatientProfile profile)
		{
			_patientProfile = new WorklistViewItemPatientProfileInfo(
				profile,
				profile.Version,
				profile.Patient,
				profile.Patient.Version,
				new WorklistViewItemPatientIdentifierInfo(profile.Mrn.Id, profile.Mrn.AssigningAuthority),
				new WorklistViewItemHealthcardNumberInfo(profile.Healthcard.Id, profile.Healthcard.AssigningAuthority),
				new WorklistViewItemPatientNameInfo(profile.Name.FamilyName, profile.Name.GivenName, profile.Name.MiddleName));
		}

		/// <summary>
		/// Sets the visit component of this view item.
		/// </summary>
		/// <param name="visit"></param>
		public virtual void SetVisitInfo(Visit visit)
		{
			_visit = new WorklistViewItemVisitInfo(
				visit,
				visit.Version,
				visit.PatientClass,
				visit.CurrentLocation);
		}

		/// <summary>
		/// Sets the order component of this view item.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="updateReferences"></param>
		public virtual void SetOrderInfo(Order order, bool updateReferences)
		{
			_order = new WorklistViewItemOrderInfo(
				order,
				order.Version,
				order.AccessionNumber,
				order.Priority,
				order.SchedulingRequestTime,
				order.ScheduledStartTime,
				order.StartTime,
				order.EndTime,
				new WorklistViewItemDiagnosticServiceInfo(order.DiagnosticService, order.DiagnosticService.Id, order.DiagnosticService.Name),
				order.OrderingPractitioner);
			
			if(updateReferences)
			{
				SetVisitInfo(order.Visit);
				SetPatientProfileInfo(order.Patient.GetProfile(_procedure.PerformingFacility));
			}
		}

		/// <summary>
		/// Sets the procedure component of this view item.
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="updateReferences"></param>
		public virtual void SetProcedureInfo(Procedure procedure, bool updateReferences)
		{
			_procedure = new WorklistViewItemProcedureInfo(
				procedure,
				procedure.Version,
				procedure.PerformingFacility,
				procedure.PerformingDepartment,
				new WorklistViewItemProcedureTypeInfo(procedure.Type, procedure.Type.Id, procedure.Type.Name),
				procedure.Portable,
				procedure.Laterality,
				procedure.ScheduledStartTime,
				procedure.StartTime,
				procedure.EndTime,
				procedure.Status,
				procedure.DowntimeRecoveryMode);

			if (updateReferences)
			{
				SetOrderInfo(procedure.Order, true);
			}
		}

		protected virtual UpdateViewItemDelegate GetFieldUpdater(WorklistItemField field)
		{
			// return the mapping for the field, or a no-op mapping
			UpdateViewItemDelegate mapping;
			return _fieldMappings.TryGetValue(field, out mapping) ? mapping : delegate { };
		}

		/// <summary>
		/// Initializes this view item from the specified data tuple.
		/// </summary>
		private void InitializeFromTuple(object[] tuple, UpdateViewItemDelegate[] mapping, IPersistenceContext persistenceContext)
		{
			for (var i = 0; i < tuple.Length; i++)
			{
				// the tuple will consist entirely of entityRefs (as defined by ViewSourceProjection)
				// before we can use them, we need to map each ref to the actual entity
				var value = tuple[i];
				if (value != null)
					value = persistenceContext.Load((EntityRef)value, EntityLoadFlags.Proxy);

				// apply mapping i to value
				// don't need to update direct references when initializing from tuple
				mapping[i](this, value, false);
			}
		}
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}