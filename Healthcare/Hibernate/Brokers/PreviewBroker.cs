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

        public IList<ModalityProcedureStep> QueryModalityProcedureStepData(Patient patient)
        {
            string hqlQuery = _hqlSelectMPS + _hqlJoinRP + _hqlJoinOrder + _hqlCommonJoin + _hqlBaseCondition;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("patient", patient));

            return GetList<ModalityProcedureStep>(hqlQuery, parameters);
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
