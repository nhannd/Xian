#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;
using NHibernate.Engine;
using NHibernate.UserTypes;

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

        public object Replace(object original, object target, ISessionImplementor session, object owner)
        {
            return DeepCopy(original);
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
            if (Object.ReferenceEquals(x, y))
                return true;
            if (x == null || y == null)
                return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            DateTimeRange value = (DateTimeRange) x;
            return value.GetHashCode();
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

        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            var from = (DateTime?)NHibernateUtil.DateTime.NullSafeGet(dr, names[0], session, owner);
            var until = (DateTime?)NHibernateUtil.DateTime.NullSafeGet(dr, names[1], session, owner);
            return new DateTimeRange(from, until);
        }


      public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            var dtr = (DateTimeRange)value;
            NHibernateUtil.DateTime.NullSafeSet(cmd, dtr == null ? null : dtr.From, index, settable, session);
            NHibernateUtil.DateTime.NullSafeSet(cmd, dtr == null ? null : dtr.Until, index + 1, settable, session);
        }

        #endregion
    }
}
