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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.Common.Utilities;

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
            // load all values in asc order
            return LoadTable<EnumValue>(enumValueClass);
        }

        public IList<TEnumValue> Load<TEnumValue>()
            where TEnumValue : EnumValue
        {
            // load all values in asc order
            return LoadTable<TEnumValue>(typeof(TEnumValue));
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
            // find the insertAfter value so we can determine a value for "display order"
            // we are inserting after the last value in the set, so use order by desc
            HqlQuery q = new HqlQuery(string.Format("from {0} order by DisplayOrder desc", enumValueClass.FullName));
            q.Page = new SearchResultPage(0, 1);    // only need 1 value
            EnumValue insertAfterValue = CollectionUtils.FirstElement<EnumValue>(ExecuteHql<EnumValue>(q));

            float displayOrder = ComputeDisplayOrderValue(insertAfterValue, null);

            EnumValue ev = (EnumValue)Activator.CreateInstance(enumValueClass, true);
            UpdateValue(ev, code, value, description, displayOrder);

            this.Context.Session.Save(ev);

            return ev;
        }

        public EnumValue UpdateValue(Type enumValueClass, string code, string value, string description)
        {
            EnumValue ev = this.Context.LoadEnumValue(enumValueClass, code, false); 
            UpdateValue(ev, code, value, description, ev.DisplayOrder);

            return ev;
        }

        public void RemoveValue(Type enumValueClass, string code)
        {
           EnumValue ev = this.Context.LoadEnumValue(enumValueClass, code, true);
           this.Context.Session.Delete(ev);
        }

        public void MoveValue(Type enumValueClass, string code, string insertAfterCode)
        {
            Platform.CheckForNullReference(code, "code");

            // check for no-op
            if(code.Equals(insertAfterCode))
                return;

            // load the insertAfter value
            // null means insert at the beginning
            EnumValue insertAfterValue = insertAfterCode == null ? null : 
                this.Context.LoadEnumValue(enumValueClass, insertAfterCode, true);

            // find the insertBefore value (value immediately following the insertAfter value)
            HqlQuery q = new HqlQuery(string.Format("from {0} order by DisplayOrder asc", enumValueClass.FullName));
            q.Conditions.Add(new HqlCondition("DisplayOrder > ?",
                new object[] { insertAfterValue == null ? 0 : insertAfterValue.DisplayOrder }));
            q.Page = new SearchResultPage(0, 1);    // only need 1 value
            EnumValue insertBeforeValue = CollectionUtils.FirstElement<EnumValue>(ExecuteHql<EnumValue>(q));

            // load the value to move
            EnumValue valueToMove = this.Context.LoadEnumValue(enumValueClass, code, true);

            // check for no-op
            if(valueToMove.Equals(insertBeforeValue))
                return;

            // assign new displayOrder value
            float displayOrder = ComputeDisplayOrderValue(insertAfterValue, insertBeforeValue);
            SetEnumValueProperty(valueToMove, "DisplayOrder", displayOrder);
        }

        #endregion

        private IList<T> LoadTable<T>(Type enumValueClass)
        {
            // load all values in asc order
            HqlQuery q = new HqlQuery(string.Format("from {0} order by DisplayOrder_ asc", enumValueClass.FullName));
            return ExecuteHql<T>(q);
        }

        private void UpdateValue(EnumValue ev, string code, string value, string description, float displayOrder)
        {
            SetEnumValueProperty(ev, "Code", code);
            SetEnumValueProperty(ev, "Value", value);
            SetEnumValueProperty(ev, "Description", description);
            SetEnumValueProperty(ev, "DisplayOrder", displayOrder);
        }

        private void SetEnumValueProperty(EnumValue ev, string property, object value)
        {
            MethodInfo setter = typeof(EnumValue).GetProperty(property).GetSetMethod(true);
            setter.Invoke(ev, new object[] { value });
        }

        public float ComputeDisplayOrderValue(EnumValue lower, EnumValue upper)
        {
            float l = lower == null ? 0 : lower.DisplayOrder;
            return upper == null ? l + 1 : (l + upper.DisplayOrder)/2;
        }
    }
}
