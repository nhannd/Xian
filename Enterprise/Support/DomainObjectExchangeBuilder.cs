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
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Collections;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Support
{
    class DomainObjectExchangeBuilder
    {
        public static IList<IFieldExchange> CreateFieldExchangers(Type domainClass, Type infoClass)
        {
            if (infoClass.GetCustomAttributes(typeof(DataContractAttribute), false).Length == 0)
                throw new Exception("Missing DataContract attribute");  //TODO fix this

            List<IFieldExchange> exchanges = new List<IFieldExchange>();
            foreach (PropertyInfo infoClassProperty in infoClass.GetProperties())
            {
                if (infoClassProperty.GetCustomAttributes(typeof(DataMemberAttribute), true).Length > 0)
                {
                    PropertyInfo domainClassProperty = domainClass.GetProperty(infoClassProperty.Name);
                    exchanges.Add(CreateFieldExchange(domainClassProperty, infoClassProperty));
                }
            }
            return exchanges;
        }

        private static IFieldExchange CreateFieldExchange(PropertyInfo domainClassProperty, PropertyInfo infoClassProperty)
        {
            GetFieldValueDelegate domainFieldGetter = CreateGetFieldValueDelegate(domainClassProperty);
            GetFieldValueDelegate infoFieldGetter = CreateGetFieldValueDelegate(infoClassProperty);
            SetFieldValueDelegate domainFieldSetter = CreateSetFieldValueDelegate(domainClassProperty);
            SetFieldValueDelegate infoFieldSetter = CreateSetFieldValueDelegate(infoClassProperty);

            Type fieldType = infoClassProperty.PropertyType;
            if (fieldType.IsSubclassOf(typeof(ValueObjectInfo)))
            {
                IInfoExchange conversion = GetDomainObjectConversion(domainClassProperty.PropertyType, infoClassProperty.PropertyType);
                return new ValueFieldExchange(domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, conversion);
            }
            else if (fieldType.IsSubclassOf(typeof(EntityInfo)))
            {
                IInfoExchange conversion = GetDomainObjectConversion(domainClassProperty.PropertyType, infoClassProperty.PropertyType);
                return new EntityFieldExchange(domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, conversion);
            }
            else if (CollectionUtils.Contains<Type>(fieldType.GetInterfaces(), delegate(Type t) { return t.Equals(typeof(IList)); }))
            {
                Type infoElementType = GetCollectionElementType(fieldType);
                Type domainElementType = GetAssociatedDomainClass(infoElementType);
                IInfoExchange conversion = GetDomainObjectConversion(domainElementType, infoElementType);

                Type typedCollectionFieldExchange = typeof(CollectionFieldExchange<>).MakeGenericType(new Type[] { infoElementType });
                return (IFieldExchange)Activator.CreateInstance(typedCollectionFieldExchange, domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, conversion);
            }
            else
            {
                // assume it is a primitive field
                return new ValueFieldExchange(domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, new PrimitiveTypeInfoExchange());
            }
        }

        private static GetFieldValueDelegate CreateGetFieldValueDelegate(PropertyInfo property)
        {
            return (GetFieldValueDelegate)Delegate.CreateDelegate(typeof(GetFieldValueDelegate), property.GetGetMethod());
        }

        private static SetFieldValueDelegate CreateSetFieldValueDelegate(PropertyInfo property)
        {
            return property.GetSetMethod() == null ? null :
                (SetFieldValueDelegate)Delegate.CreateDelegate(typeof(SetFieldValueDelegate), property.GetSetMethod());
        }

        private static IInfoExchange GetDomainObjectConversion(Type domainClass, Type infoClass)
        {
            Type typedConversion = typeof(DomainObjectInfoExchange<,>).MakeGenericType(new Type[] { domainClass, infoClass });
            return (IInfoExchange)Activator.CreateInstance(typedConversion);
        }

        private static Type GetCollectionElementType(Type collectionType)
        {
            if (!collectionType.IsGenericType)
                throw new Exception("can't determine element type");    // TODO fix this

            Type[] genericArgs = collectionType.GetGenericArguments();
            if(genericArgs.Length == 0)
                throw new Exception("can't determine element type");    // TODO fix this

            return genericArgs[0];
        }

        public static Type GetAssociatedDomainClass(Type infoClass)
        {
            AssociateDomainClassAttribute assocDomainClassAttr = CollectionUtils.FirstElement<AssociateDomainClassAttribute>(
                infoClass.GetCustomAttributes(typeof(AssociateDomainClassAttribute), false));
            if (assocDomainClassAttr == null)
                throw new Exception("Missing assoc domain class attr");     // TODO fix this
            return assocDomainClassAttr.DomainClass;
        }
    }
}
