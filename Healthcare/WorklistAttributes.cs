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

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class for worklist attribute classes.
    /// </summary>
    public abstract class WorklistAttribute : Attribute
    {

    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, indicates that the class is "static",
    /// in that it does not support creation of persistent instances.
    /// If this attribute is not applied, it is assumed that the class is not static.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StaticWorklistAttribute : WorklistAttribute
    {
        private readonly bool _isSingleton;

        public StaticWorklistAttribute(bool isSingleton)
        {
            _isSingleton = isSingleton;
        }

        public bool IsSingleton
        {
            get { return _isSingleton; }
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, declares the subclass of <see cref="ProcedureTypeGroup"/>
    /// that the worklist is based on.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistProcedureTypeGroupClassAttribute : WorklistAttribute
    {
        private readonly Type _procedureTypeGroupClass;

        public WorklistProcedureTypeGroupClassAttribute(Type procedureTypeGroupClass)
        {
            _procedureTypeGroupClass = procedureTypeGroupClass;
        }

        public Type ProcedureTypeGroupClass
        {
            get { return _procedureTypeGroupClass; }
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, indicates whether the class supports time filters.
    /// If this attribute is not applied, it is assumed that the class <b>does not</b> support time filtering.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistSupportsTimeFilterAttribute : WorklistAttribute
    {
        private readonly bool _supportsTimeFilter;

        public WorklistSupportsTimeFilterAttribute(bool supportsTimeFilter)
        {
            _supportsTimeFilter = supportsTimeFilter;
        }

        public bool SupportsTimeFilter
        {
            get { return _supportsTimeFilter; }
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, indicates whether the class supports time filters.
    /// If this attribute is not applied, it is assumed that the class <b>does not</b> support time filtering.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistSupportsReportingStaffRoleFilterAttribute : WorklistAttribute
    {
        private readonly bool _supportsReportingStaffRoleFilter;

        public WorklistSupportsReportingStaffRoleFilterAttribute(bool supportsReportingStaffRoleFilter)
        {
            _supportsReportingStaffRoleFilter = supportsReportingStaffRoleFilter;
        }

        public bool SupportsReportingStaffRoleFilter
        {
            get { return _supportsReportingStaffRoleFilter; }
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, declares the category in which the class belongs.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistCategoryAttribute : WorklistAttribute
    {
        private readonly string _category;

        public WorklistCategoryAttribute(string category)
        {
            _category = category;
        }

        /// <summary>
        /// Gets the category name, which may be a resource key.
        /// </summary>
        public string Category
        {
            get { return _category; }
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, declares a display name for the class.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistClassDisplayNameAttribute : WorklistAttribute
    {
        private readonly string _displayName;

        public WorklistClassDisplayNameAttribute(string displayName)
        {
            _displayName = displayName;
        }

        /// <summary>
        /// Gets the display name, which may be a resource key.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, declares a description for the class.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistClassDescriptionAttribute : WorklistAttribute
    {
        private readonly string _description;

        public WorklistClassDescriptionAttribute(string description)
        {
            _description = description;
        }

        /// <summary>
        /// Gets the description, which may be a resource key.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
    }
}
