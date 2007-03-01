using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Data.Hibernate.Hql;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Data.Hibernate
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
