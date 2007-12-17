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

using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using NHibernate;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class WorklistBroker : EntityBroker<Worklist, WorklistSearchCriteria>, IWorklistBroker
    {
        #region Private Fields

        private readonly string _selectHql = "select w from Worklist w join w.Users u"
                                             + " where u = :user ";

        #endregion

        #region IWorklistBroker Members

        public IList FindWorklists(User currentUser, IList classNames)
        {
            return DoQuery(currentUser, ConstructWorklistHql(classNames));
        }

        public Worklist FindWorklist(string name, string type)
        {
            IQuery query = this.Context.CreateHibernateQuery("select w from Worklist w where w.Name = :name and w.class = " + type);
            query.SetParameter("name", name);
            return CollectionUtils.FirstElement<Worklist>(query.List());
        }

        public bool NameExistsForType(string name, string type)
        {
            IQuery query = this.Context.CreateHibernateQuery("select w from Worklist w where w.Name = :name and w.class = " + type);
            query.SetParameter("name", name);
            return query.List().Count > 0;
        }

        #endregion

        #region Private Methods

        private string ConstructWorklistHql(IList classNames)
        {
            // Construct a worklist hql that looks like the following
            // " and (w.class = className1 or w.class = className2)"

            string worklistCondition = string.Join(" or ",
                CollectionUtils.Map<string, string>(classNames,
                delegate(string className)
                {
                    return string.Format("w.class = {0}", className);
                }).ToArray());

            return string.Concat(" and (", worklistCondition, ")");
        }

        private IList DoQuery(User currentUser, string subCriteria)
        {
            IQuery query = this.Context.CreateHibernateQuery(_selectHql + subCriteria);
            query.SetParameter("user", currentUser);
            return query.List();
        }

        #endregion
    }
}
