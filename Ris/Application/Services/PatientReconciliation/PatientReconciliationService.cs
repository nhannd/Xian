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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.PatientReconciliation
{
	[ServiceImplementsContract(typeof(IPatientReconciliationService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class PatientReconciliationService : ApplicationServiceBase, IPatientReconciliationService
	{
		#region IPatientReconciliationService Members

		[ReadOperation]
		public ListPatientReconciliationMatchesResponse ListPatientReconciliationMatches(ListPatientReconciliationMatchesRequest request)
		{
			var targetProfile = this.PersistenceContext.GetBroker<IPatientProfileBroker>().Load(request.PatientProfileRef);

			var strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
			var matches = strategy.FindReconciliationMatches(targetProfile, this.PersistenceContext);

			var profileAssembler = new PatientProfileAssembler();
			var rcAssembler = new ReconciliationCandidateAssembler();
			var response = new ListPatientReconciliationMatchesResponse
				{
					ReconciledProfiles =
						CollectionUtils.Map<PatientProfile, PatientProfileSummary, List<PatientProfileSummary>>(
						targetProfile.Patient.Profiles,
						profile => profileAssembler.CreatePatientProfileSummary(profile, this.PersistenceContext)),
					MatchCandidates =
						CollectionUtils.Map<PatientProfileMatch, ReconciliationCandidate, List<ReconciliationCandidate>>(
						matches,
						match => rcAssembler.CreateReconciliationCandidate(match, this.PersistenceContext))
				};

			return response;
		}

		[ReadOperation]
		public LoadPatientProfileDiffResponse LoadPatientProfileDiff(LoadPatientProfileDiffRequest request)
		{
			var broker = this.PersistenceContext.GetBroker<IPatientProfileBroker>();

			// load profiles to compare
			var leftProfile = broker.Load(request.LeftProfileRef);
			var rightProfile = broker.Load(request.RightProfileRef);

			// ask model to compute discrepancies
			var results = PatientProfileDiscrepancyTest.GetDiscrepancies(leftProfile, rightProfile, PatientProfileDiscrepancy.All);

			// build response
			var assembler = new PatientProfileDiffAssembler();
			var diff = assembler.CreatePatientProfileDiff(leftProfile, rightProfile, results);

			return new LoadPatientProfileDiffResponse(diff);
		}

		[ReadOperation]
		public ListProfilesForPatientsResponse ListProfilesForPatients(ListProfilesForPatientsRequest request)
		{
			var assembler = new PatientProfileAssembler();
			var summaries = new List<PatientProfileSummary>();
			foreach (var patientRef in request.PatientRefs)
			{
				var patient = this.PersistenceContext.Load<Patient>(patientRef);
				foreach (var profile in patient.Profiles)
				{
					summaries.Add(assembler.CreatePatientProfileSummary(profile, this.PersistenceContext));
				}
			}

			return new ListProfilesForPatientsResponse(summaries);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Patient.Reconcile)]
		public ReconcilePatientsResponse ReconcilePatients(ReconcilePatientsRequest request)
		{
			var patients = CollectionUtils.Map<EntityRef, Patient, List<Patient>>(
				request.PatientRefs,
				patientRef => this.PersistenceContext.Load<Patient>(patientRef, EntityLoadFlags.CheckVersion));

			if (patients.Count < 2)
				throw new RequestValidationException(SR.ExceptionReconciliationRequiresAtLeast2Patients);

			var op = new Operations.ReconcilePatient();
			op.Execute(patients, new PersistentWorkflow(this.PersistenceContext));

			return new ReconcilePatientsResponse();
		}

		#endregion
	}
}
