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

using System;
using System.Collections.Generic;
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
