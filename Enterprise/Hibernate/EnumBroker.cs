using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IEnumBroker"/>.
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class EnumBroker : Broker, IEnumBroker
    {
        #region IEnumBroker Members

        public IList<EnumValue> Load(Type enumValueClass)
        {
            return MakeTypeSafe<EnumValue>(LoadTable(enumValueClass));
        }

        public IList<TEnumValue> Load<TEnumValue>()
            where TEnumValue : EnumValue
        {
            return MakeTypeSafe<TEnumValue>(LoadTable(typeof(TEnumValue)));
        }

        public EnumValue Lookup(Type enumValueClass, string code)
        {
            return this.Context.LoadEnumValue(enumValueClass, code);
        }

        public TEnumValue Lookup<TEnumValue>(string code)
            where TEnumValue : EnumValue
        {
            return (TEnumValue)Lookup(typeof(TEnumValue), code);
        }

        #endregion

        private IList LoadTable(Type enumValueClass)
        {
            HqlQuery q = new HqlQuery(string.Format("from {0}", enumValueClass.FullName));
            return ExecuteHql(q);
        }
    }
}
