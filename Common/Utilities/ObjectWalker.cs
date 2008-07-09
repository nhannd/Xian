#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Defines a callback interface used by the <see cref="ObjectWalker"/> class.
    /// </summary>
    public interface IObjectMemberContext
    {
        /// <summary>
        /// Gets the object instance being walked, or null if a type is being walked.
        /// </summary>
        object Object { get;}

        /// <summary>
        /// Gets the property or field that the walker is currently at.
        /// </summary>
        MemberInfo Member { get;}

        /// <summary>
        /// Gets the type of the property or field that the walker is currently at.
        /// </summary>
        Type MemberType { get; }

        /// <summary>
        /// Gets or sets the value of the property or field that the walker is currently at,
        /// assuming an object instance is being walked.
        /// </summary>
        object MemberValue { get; set; }
    }

    /// <summary>
    /// Utility class for walking the properties and/or fields of an object.
    /// </summary>
    /// <remarks>
    /// By default, the public properties and fields of the object will be included in the walk.
    /// Set the properties of the <see cref="ObjectWalker"/> instance to optionally include
    /// private fields and/or properties, or to optionally exclude public fields/properties. 
    /// </remarks>
    public class ObjectWalker
    {
        /// <summary>
        /// Defines a delegate that acts as a member filter.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public delegate bool MemberFilterDelegate(MemberInfo property);

        /// <summary>
        /// Defines a delegate that acts as a member handler.
        /// </summary>
        /// <param name="context"></param>
        public delegate void MemberHandlerDelegate(IObjectMemberContext context);

        #region IObjectWalkerContext implementations

        class PropertyContext : IObjectMemberContext
        {
            private readonly PropertyInfo _property;
            private readonly object _obj;

            public PropertyContext(object obj, PropertyInfo property)
            {
                _property = property;
                _obj = obj;
            }

            public object Object
            {
                get { return _obj; }
            }

            public Type MemberType
            {
                get { return _property.PropertyType; }
            }

            public MemberInfo Member
            {
                get { return _property;}
            }

            public object MemberValue
            {
                get { return _property.GetValue(_obj, BindingFlags.Public|BindingFlags.NonPublic, null, null, null);}
                set { _property.SetValue(_obj, value, BindingFlags.Public | BindingFlags.NonPublic, null, null, null); }
            }
        }

        class FieldContext : IObjectMemberContext
        {
            private readonly FieldInfo _member;
            private readonly object _obj;

            public FieldContext(object obj, FieldInfo member)
            {
                _member = member;
                _obj = obj;
            }

            public object Object
            {
                get { return _obj; }
            }

            public Type MemberType
            {
                get { return _member.FieldType; }
            }

            public MemberInfo Member
            {
                get { return _member;}
            }

            public object MemberValue
            {
                get { return _member.GetValue(_obj);}
                set { _member.SetValue(_obj, value);}
            }
        }

        #endregion

        #region Private fields

        private bool _includeNonPublicFields;
        private bool _includePublicFields;
        private bool _includeNonPublicProperties;
        private bool _includePublicProperties;

        private MemberFilterDelegate _memberFilter;
        private MemberHandlerDelegate _memberHandler;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectWalker()
        {
            // includ public fields and properties by default
            _includePublicFields = true;
            _includePublicProperties = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="memberHandler"></param>
        public ObjectWalker(MemberHandlerDelegate memberHandler)
            : this(memberHandler, null)
        {
            _memberHandler = memberHandler;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="memberHandler"></param>
        /// <param name="memberFilter"></param>
        public ObjectWalker(MemberHandlerDelegate memberHandler, MemberFilterDelegate memberFilter)
            : this()
        {
            _memberHandler = memberHandler;
            _memberFilter = memberFilter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to include non-public fields in the walk.
        /// </summary>
        public bool IncludeNonPublicFields
        {
            get { return _includeNonPublicFields; }
            set { _includeNonPublicFields = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include public fields in the walk.
        /// </summary>
        public bool IncludePublicFields
        {
            get { return _includePublicFields; }
            set { _includePublicFields = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include non-public properties in the walk.
        /// </summary>
        public bool IncludeNonPublicProperties
        {
            get { return _includeNonPublicProperties; }
            set { _includeNonPublicProperties = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include public properties in the walk.
        /// </summary>
        public bool IncludePublicProperties
        {
            get { return _includePublicProperties; }
            set { _includePublicProperties = value; }
        }

        /// <summary>
        /// Gets or sets a delegate that filters which members will be passed to the <see cref="MemberHandler"/>.
        /// </summary>
        public MemberFilterDelegate MemberFilter
        {
            get { return _memberFilter; }
            set { _memberFilter = value; }
        }

        /// <summary>
        /// Gets or sets a delegate that is invoked for each member property or field that is walked.
        /// </summary>
        public MemberHandlerDelegate MemberHandler
        {
            get { return _memberHandler; }
            set { _memberHandler = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Walks properties and/or fields of the specified object.
        /// </summary>
        /// <param name="obj"></param>
        public void Walk(object obj)
        {
            Platform.CheckForNullReference(obj, "obj");

            WalkHelper(obj.GetType(), obj);
        }

        /// <summary>
        /// Walks properties and/or fields of the specified type.
        /// </summary>
        /// <param name="type"></param>
        public void Walk(Type type)
        {
            WalkHelper(type, null);
        }

        #endregion

        private void WalkHelper(Type type, object instance)
        {
            if (_memberHandler == null)
                throw new InvalidOperationException("MemberHandler not set.");

            // walk properties
            if (_includePublicProperties || _includeNonPublicProperties)
            {
                BindingFlags bindingFlags = BindingFlags.Instance;
                if (_includePublicProperties)
                    bindingFlags |= BindingFlags.Public;
                if (_includeNonPublicProperties)
                    bindingFlags |= BindingFlags.NonPublic;
                foreach (PropertyInfo property in type.GetProperties(bindingFlags))
                {
                    if (_memberFilter == null || _memberFilter(property))
                    {
                        _memberHandler(new PropertyContext(instance, property));
                    }
                }
            }

            // walk fields
            if (_includePublicFields || _includeNonPublicFields)
            {
                BindingFlags bindingFlags = BindingFlags.Instance;
                if (_includePublicFields)
                    bindingFlags |= BindingFlags.Public;
                if (_includeNonPublicFields)
                    bindingFlags |= BindingFlags.NonPublic;
                foreach (FieldInfo field in type.GetFields(bindingFlags))
                {
                    if (_memberFilter == null || _memberFilter(field))
                    {
                        _memberHandler(new FieldContext(instance, field));
                    }
                }
            }
        }
    }
}
