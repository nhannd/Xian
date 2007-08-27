using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Reflection;

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

        public EnumValue Find(Type enumValueClass, string code)
        {
            return this.Context.LoadEnumValue(enumValueClass, code, true);
        }

        public TEnumValue Find<TEnumValue>(string code)
            where TEnumValue : EnumValue
        {
            return (TEnumValue)Find(typeof(TEnumValue), code);
        }

        public EnumValue AddValue(Type enumValueClass, string code, string value, string description)
        {
            EnumValue ev = (EnumValue)Activator.CreateInstance(enumValueClass, true);
            UpdateValue(ev, code, value, description);

            this.Context.Session.Save(ev);

            return ev;
        }

        public EnumValue UpdateValue(Type enumValueClass, string code, string value, string description)
        {
            EnumValue ev = this.Context.LoadEnumValue(enumValueClass, code, false); 
            UpdateValue(ev, code, value, description);

            return ev;
        }

        public void RemoveValue(Type enumValueClass, string code)
        {
           EnumValue ev = this.Context.LoadEnumValue(enumValueClass, code, true);
           this.Context.Session.Delete(ev);
        }

        #endregion

        private IList LoadTable(Type enumValueClass)
        {
            HqlQuery q = new HqlQuery(string.Format("from {0}", enumValueClass.FullName));
            return ExecuteHql(q);
        }

        private void UpdateValue(EnumValue ev, string code, string value, string description)
        {
            MethodInfo setCodeMethod = typeof(EnumValue).GetProperty("Code").GetSetMethod(true);
            setCodeMethod.Invoke(ev, new object[] { code });

            MethodInfo setValueMethod = typeof(EnumValue).GetProperty("Value").GetSetMethod(true);
            setValueMethod.Invoke(ev, new object[] { value });

            MethodInfo setDescriptionMethod = typeof(EnumValue).GetProperty("Description").GetSetMethod(true);
            setDescriptionMethod.Invoke(ev, new object[] { description });
        }
    }
}
