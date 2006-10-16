using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate
{
    /// <summary>
    /// Provides an NHibernate mapping of the <see cref="DateTimeRange"/> class
    /// </summary>
    public class DateTimeRangeHbm : ICompositeUserType
    {
        #region ICompositeUserType Members

        public object Assemble(object cached, NHibernate.Engine.ISessionImplementor session, object owner)
        {
            return DeepCopy(cached);
        }

        public object Disassemble(object value, NHibernate.Engine.ISessionImplementor session)
        {
            return DeepCopy(value);
        }

        public object DeepCopy(object value)
        {
            if (value == null)
                return null;

            DateTimeRange original = (DateTimeRange)value;  // throws InvalidCast... if wrong type of object
            return new DateTimeRange(original.From, original.Until);
        }

        public new bool Equals(object x, object y)
        {
            return (x as DateTimeRange) == (y as DateTimeRange);
        }

        public string[] PropertyNames
        {
            get
            {
                return new string[] { "From", "Until" };
            }
        }

        public NHibernate.Type.IType[] PropertyTypes
        {
            get
            {
                return new NHibernate.Type.IType[] { NHibernateUtil.DateTime, NHibernateUtil.DateTime };
            }
        }

        public Type ReturnedClass
        {
            get { return typeof(DateTimeRange); }
        }

        public object GetPropertyValue(object component, int property)
        {
            switch (property)
            {
                case 0:
                    return ((DateTimeRange)component).From;
                case 1:
                    return ((DateTimeRange)component).Until;
            }
            return null;
        }

        public void SetPropertyValue(object component, int property, object value)
        {
            switch (property)
            {
                case 0:
                    ((DateTimeRange)component).From = (DateTime?)value;
                    break;
                case 1:
                    ((DateTimeRange)component).Until = (DateTime?)value;
                    break;
            }
        }


        public bool IsMutable
        {
            get { return true; }
        }

        public object NullSafeGet(System.Data.IDataReader dr, string[] names, NHibernate.Engine.ISessionImplementor session, object owner)
        {
            DateTime? from = (DateTime?)NHibernateUtil.DateTime.NullSafeGet(dr, names[0], session, owner);
            DateTime? until = (DateTime?)NHibernateUtil.DateTime.NullSafeGet(dr, names[1], session, owner);

            //return (from == null && until == null) ? null : new DateTimeRange(from, until);
            return new DateTimeRange(from, until);
        }

        public void NullSafeSet(System.Data.IDbCommand cmd, object value, int index, NHibernate.Engine.ISessionImplementor session)
        {
            DateTimeRange dtr = (DateTimeRange)value;

            NHibernateUtil.DateTime.NullSafeSet(cmd, dtr == null ? null : dtr.From, index, session);
            NHibernateUtil.DateTime.NullSafeSet(cmd, dtr == null ? null : dtr.Until, index + 1, session);
        }


        #endregion
    }
}
