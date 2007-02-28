using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IEnumBroker"/>.
    /// </summary>
    public abstract class CodedValueBroker<TCodedValue> : Broker, ICodedValueBroker<TCodedValue>
        where TCodedValue : CodedValue
    {
        #region ICodedValueBroker<TCodedValue> Members

        public CodedValueDictionary<TCodedValue> LoadDictionary()
        {
            HqlQuery q = new HqlQuery(string.Format("from {0}", typeof(TCodedValue).Name));
            return new CodedValueDictionary<TCodedValue>(MakeTypeSafe<TCodedValue>(ExecuteHql(q)));
        }

        #endregion
    }
}
