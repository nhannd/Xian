#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using System.Linq;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Validation rules for <see cref="Order"/> and <see cref="Procedure"/> entities.
	/// </summary>
	internal static class OrderRules
	{
		internal static TestResult VisitAndPerformingFacilitiesHaveSameInformationAuthority(Order order)
		{
			// all non-defunct procedures for the order must have the same performing information authority as the visit
			var hasSameInformationAuthority = CollectionUtils.TrueForAll(NonDefunctProcedures(order),
				p => Equals(p.PerformingFacility.InformationAuthority, p.Order.Visit.VisitNumber.AssigningAuthority));
			return new TestResult(hasSameInformationAuthority, SR.MessageValidateInformationAuthorityForVisitAndPerformingFacilities);
		}

		internal static TestResult AllNonDefunctProceduresHaveSamePerformingFacility(Order order)
		{
			// all non-defunct procedures for the order must have the same performing facility
			var procedures = NonDefunctProcedures(order);
			var facility = CollectionUtils.FirstElement(CollectionUtils.Map(procedures, (Procedure p) => p.PerformingFacility));
			var hasSameFacility = CollectionUtils.TrueForAll(procedures, p => Equals(p.PerformingFacility, facility));
			return new TestResult(hasSameFacility, SR.MessageValidateOrderPerformingFacilities);
		}

		internal static TestResult AllNonDefunctProceduresHaveSamePerformingDepartment(Order order)
		{
			// all non-defunct procedures for the order must have the same performing department
			var procedures = NonDefunctProcedures(order);
			var department = CollectionUtils.FirstElement(CollectionUtils.Map(procedures, (Procedure p) => p.PerformingDepartment));
			var hasSameDepartment = CollectionUtils.TrueForAll(procedures, p => Equals(p.PerformingDepartment, department));
			return new TestResult(hasSameDepartment, SR.MessageValidateOrderPerformingDepartments);
		}

		internal static TestResult PerformingDepartmentAlignsWithPerformingFacility(Procedure procedure)
		{
			// performing department must be associated with performing facility
			var performingDepartmentIsInPerformingFacility = procedure.PerformingDepartment == null
				|| procedure.PerformingFacility.Equals(procedure.PerformingDepartment.Facility);
			return new TestResult(performingDepartmentIsInPerformingFacility, SR.MessageValidateProcedurePerformingFacilityAndDepartment);
		}

		internal static TestResult ModalitiesAlignWithPerformingFacility(Procedure procedure)
		{
			// modality facilities must match performing facility
			var valid = procedure.ModalityProcedureSteps.All(
				mps => ModalityAlignsWithPerformingFacility(mps).Success);

			return new TestResult(valid, SR.MessageValidateProcedurePerformingFacilityAndModalities);
		}

		internal static TestResult ModalityAlignsWithPerformingFacility(ModalityProcedureStep mps)
		{
			// modality facility must match performing facility
			var valid = mps.Modality.Facility == null || mps.Modality.Facility.Equals(mps.Procedure.PerformingFacility);

			return new TestResult(valid, SR.MessageValidateProcedurePerformingFacilityAndModalities);
		}

		internal static TestResult PatientProfileExistsForPerformingFacility(Procedure procedure)
		{
			// patient must have a profile at the performing facility
			var patientProfileExists = procedure.PatientProfile != null;
			return new TestResult(patientProfileExists, SR.MessageValidateProcedurePatientProfile);
		}

		private static List<Procedure> NonDefunctProcedures(Order order)
		{
			return order.GetProcedures(p => !p.IsDefunct);
		}
	}
}
