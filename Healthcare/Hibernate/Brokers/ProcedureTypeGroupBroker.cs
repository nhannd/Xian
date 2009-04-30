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

// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// NHibernate implementation of <see cref="IProcedureTypeGroupBroker"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
	public partial class ProcedureTypeGroupBroker : IProcedureTypeGroupBroker
	{
        #region IProcedureTypeGroupBroker Members

        public IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass)
        {
        	return Find(criteria, subClass, null);
        }

		public IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass, SearchResultPage page)
		{
			HqlQuery query = new HqlQuery(string.Format("from {0} x", subClass.Name));

			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("x", criteria));
			query.Sorts.AddRange(HqlSort.FromSearchCriteria("x", criteria));
			query.Page = page;

			return ExecuteHql<ProcedureTypeGroup>(query);
		}

		public ProcedureTypeGroup FindOne(ProcedureTypeGroupSearchCriteria criteria, Type subClass)
        {
            IList<ProcedureTypeGroup> groups = Find(criteria, subClass);

            if(groups.Count == 0)
                throw new EntityNotFoundException(null);

            return CollectionUtils.FirstElement(groups);
        }

        #endregion
    }
}