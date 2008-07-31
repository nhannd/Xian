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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow;
using System;

namespace ClearCanvas.Ris.Application.Services
{
    public class WorklistTextQueryHelper<TDomainItem, TSummary>
        : TextQueryHelper<TDomainItem, WorklistItemSearchCriteria, TSummary>
        where TDomainItem : WorklistItemBase
        where TSummary : DataContractBase
    {
    	private readonly Type _procedureStepClass;
    	private readonly bool _downtimeRecoveryMode;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="summaryAssembler"></param>
		/// <param name="specificityCallback"></param>
		/// <param name="queryCallback"></param>
		/// <param name="procedureStepClass"></param>
		/// <param name="downtimeRecoveryMode"></param>
		public WorklistTextQueryHelper(
			Converter<TDomainItem, TSummary> summaryAssembler,
			TestCriteriaSpecificityDelegate specificityCallback,
			DoQueryDelegate queryCallback,
			Type procedureStepClass,
			bool downtimeRecoveryMode)
			: base(null, summaryAssembler, specificityCallback, queryCallback)
		{
			_procedureStepClass = procedureStepClass;
			_downtimeRecoveryMode = downtimeRecoveryMode;
		}


        protected override WorklistItemSearchCriteria[] BuildCriteria(string query)
        {
            // this will hold all criteria
            List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

            // build criteria against names
            PersonName[] names = ParsePersonNames(query);
            criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
                delegate(PersonName n)
                {
                    WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
                    sc.PatientProfile.Name.FamilyName.StartsWith(n.FamilyName);
                    if (n.GivenName != null)
                        sc.PatientProfile.Name.GivenName.StartsWith(n.GivenName);
                    return sc;
                }));

            // build criteria against Mrn identifiers
            string[] ids = ParseIdentifiers(query);
            criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
                delegate(string word)
                {
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
                    c.PatientProfile.Mrn.Id.StartsWith(word);
                    return c;
                }));

            // build criteria against Healthcard identifiers
            criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
                delegate(string word)
                {
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
                    c.PatientProfile.Healthcard.Id.StartsWith(word);
                    return c;
                }));

            // build criteria against Accession Number
            criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
                delegate(string word)
                {
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
                    c.Order.AccessionNumber.StartsWith(word);
                    return c;
                }));

			// add constraint for downtime vs live procedures
			criteria.ForEach(delegate (WorklistItemSearchCriteria c) { c.Procedure.DowntimeRecoveryMode.EqualTo(_downtimeRecoveryMode); });

            return criteria.ToArray();
        }
    }
}