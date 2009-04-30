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
using System.Xml;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using ClearCanvas.Enterprise.Core.Modelling;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public class EntitySerializer<TEntity>
        where TEntity : Entity
    {
        #region Public Methods

        public void WriteEntity(TEntity entity, XmlWriter writer)
        {
            writer.WriteStartElement(entity.GetClass().Name);

            // if it's a subclass, rather than an instance of TEntity, then write the class name out as an attribute
            if (entity.GetClass().IsSubclassOf(typeof(TEntity)))
            {
                writer.WriteAttributeString("class", entity.GetClass().FullName);
            }

            WriteObjectProperties(entity, writer);
            writer.WriteEndElement();
        }

        #endregion

        #region Protected Overridables

        protected virtual void WriteProperty(object obj, PropertyInfo property, object value, XmlWriter writer)
        {
            if (IsEntity(property.PropertyType) || IsEntityCollection(property.PropertyType))
            {
                // do nothing ... no general way to write an entity or entity collection
                return;
            }

            WriteSingleValuedProperty(property, value, writer, delegate(object item) { WriteValue(item.GetType(), item, writer); });
        }

        #endregion

        #region Protected Helpers

        protected void WriteSingleValuedProperty(PropertyInfo property, object value, XmlWriter writer, Action<object> writeItemCallback)
        {
            writer.WriteStartElement(property.Name);
            writeItemCallback(value);
            writer.WriteEndElement();
        }

        protected void WriteMultiValuedProperty(PropertyInfo property, object value, XmlWriter writer, Action<object> writeItemCallback)
        {
            writer.WriteStartElement(property.Name);
            WriteCollection(value, writer, writeItemCallback);
            writer.WriteEndElement();
        }

        protected void WriteObjectProperties(object obj, XmlWriter writer)
        {
            ObjectWalker walker = new ObjectWalker(
                delegate(IObjectMemberContext ctx)
                {
                    WriteProperty(ctx.Object, (PropertyInfo)ctx.Member, ctx.MemberValue, writer);
                },
                delegate(MemberInfo member)
                {
                    return AttributeUtils.HasAttribute<PersistentPropertyAttribute>(member);
                });
            walker.IncludePublicFields = false;
            walker.Walk(obj);
        }

        #endregion

        #region Private Methods

        private void WriteCollection(object value, XmlWriter writer, Action<object> writeItemCallback)
        {
            foreach (object item in (IEnumerable)value)
            {
                writer.WriteStartElement("Item");
                writeItemCallback(item);
                writer.WriteEndElement();
            }
        }

        private void WriteValue(Type type, object value, XmlWriter writer)
        {
            if (value == null)
                return;

            if (IsEnum(type))
                WriteEnum(value, writer);
            else if (IsValueObject(type))
                WriteValueObject(value, writer);
            else if (IsCollection(type))    // collection of values
                WriteCollection(value, writer, delegate(object item) { WriteValue(item.GetType(), item, writer); });
            else if (IsPrimitive(type))
                WritePrimitive(value, writer);
            else
                throw new NotSupportedException();
        }

        private bool IsEntity(Type type)
        {
            return typeof(Entity).IsAssignableFrom(type);
        }

        private bool IsValueObject(Type type)
        {
            return typeof(ValueObject).IsAssignableFrom(type);
        }

        private bool IsEnum(Type type)
        {
            return type.IsEnum || typeof(EnumValue).IsAssignableFrom(type);
        }

        private bool IsCollection(Type type)
        {
            bool b = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
            return b;
        }

        private bool IsEntityCollection(Type type)
        {
            return IsCollection(type) && type.GetGenericArguments()[0].IsSubclassOf(typeof(Entity));
        }

        private bool IsPrimitive(Type type)
        {
            return true;
        }

        private void WriteEnum(object value, XmlWriter writer)
        {
            writer.WriteValue(value);
        }

        private void WritePrimitive(object value, XmlWriter writer)
        {
            writer.WriteValue(value);
        }

        private void WriteValueObject(object value, XmlWriter writer)
        {
            WriteObjectProperties(value, writer);
        }

	    #endregion
    }
}
