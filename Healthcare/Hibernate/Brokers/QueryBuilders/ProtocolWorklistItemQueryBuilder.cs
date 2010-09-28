#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Specialization of <see cref="WorklistItemQueryBuilder"/> for protocol worklists.
	/// </summary>
	public class ProtocolWorklistItemQueryBuilder : WorklistItemQueryBuilder
	{
		private static readonly HqlCondition ConditionMostRecentProtocolAssignmentStepIfRejected = new HqlCondition(
			"((pr.Status not in (?)) or (ps.EndTime = (select max(ps2.EndTime) from ProcedureStep ps2 where ps.Protocol = ps2.Protocol)))", ProtocolStatus.RJ);

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			base.AddRootQuery(query, args);

			var from = query.Froms[0];	// this would be added by the base.AddWorklistRootQuery call

			// join protocol object, because may have criteria on this object
			from.Joins.Add(HqlConstants.JoinProtocol);

			// check if we need to apply the "most recent step" condition
			// this is essentially a workaround to avoid showing duplicates in some worklist results
			// we can only apply this workaround when there is exactly one ps class specified
			// fortunately, there are no use cases yet where more than one ps class is specified
			// that require the workaround
			if (args.ProcedureStepClasses.Length == 1)
			{
				var psClass = CollectionUtils.FirstElement(args.ProcedureStepClasses);
				// the proc step class may be set to the more general "ProtocolProcedureStep" so
				// we need to check for both
				if (psClass == typeof(ProtocolAssignmentStep) || psClass == typeof(ProtocolProcedureStep))
				{
					// when querying for Rejected protocols, only show the most recent ProtocolAssignmentStep
					// There may be many ProtocolAssignmentStep if a protocol is rejected, resubmitted and rejected again.
					// For rejected protocols, the condition "pr.Status not in ('RJ')" is always false.  So the max(EndTime) condition is used
					// For non-rejected protocols, the first condition is always true, and the max(EndTime) condition is never used.
					query.Conditions.Add(ConditionMostRecentProtocolAssignmentStepIfRejected);
				}
			}
		}
	}
}
