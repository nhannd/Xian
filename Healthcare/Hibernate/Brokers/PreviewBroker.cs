#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class PreviewBroker : Broker, IPreviewBroker
    {
        private class QueryParameter
        {
            public QueryParameter(string name, object value)
            {
                this.Name = name;
                this.Value = value;
            }

            public string Name;
            public object Value;
        }

        private const string _hqlSelectOrder =  "select o from Order o";
        private const string _hqlSelectRP =     "select rp from RequestedProcedure rp";
        private const string _hqlSelectMPS =    "select mps from ModalityProcedureStep mps";
        private const string _hqlJoinRP =       " join mps.RequestedProcedure rp";
        private const string _hqlJoinOrder =    " join rp.Order o";
        private const string _hqlCommonJoin =   " join o.Patient p" +
                                                " join o.Visit v";
        private const string _hqlBaseCondition = " where p = :patient";

        public IList<Order> QueryOrderData(Patient patient)
        {
            string hqlQuery = _hqlSelectOrder + _hqlCommonJoin + _hqlBaseCondition;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("patient", patient));

            return GetList<Order>(hqlQuery, parameters);
        }

        public IList<RequestedProcedure> QueryRequestedProcedureData(Patient patient)
        {
            string hqlQuery = _hqlSelectRP + _hqlJoinOrder + _hqlCommonJoin + _hqlBaseCondition;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("patient", patient));

            return GetList<RequestedProcedure>(hqlQuery, parameters);
        }

        #region Query helpers

        private IList<TItem> GetList<TItem>(string hqlQuery, List<QueryParameter> parameters)
        {
            List<TItem> results = new List<TItem>();

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                TItem item = (TItem)tuple;
                results.Add(item);
            }

            return results;
        }

        private IList DoQuery(string hqlQuery, List<QueryParameter> parameters)
        {
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            foreach (QueryParameter param in parameters)
            {
                query.SetParameter(param.Name, param.Value);
            }

            return query.List();
        }

        #endregion

    }

}
