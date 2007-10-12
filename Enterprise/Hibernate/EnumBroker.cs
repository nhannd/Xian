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
