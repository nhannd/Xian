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
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Abstract base-class for worklist filters.
	/// </summary>
	public abstract class WorklistFilter : ValueObject
	{
		private bool _isEnabled;

		/// <summary>
		/// Gets or sets a value indicating whether this filter is enabled or not.
		/// </summary>
		/// <remarks>
		/// This property is significant in that it may allow the worklist broker to make optimizations
		/// when building the query by not loading the filter values for disabled filters.
		/// </remarks>
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set { _isEnabled = value; }
		}
	}

	/// <summary>
	/// Abstract base-class for single-valued worklist filters.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class WorklistSingleValuedFilter<T> : WorklistFilter, IEquatable<WorklistSingleValuedFilter<T>>
	{
		private T _value;

		/// <summary>
		/// Gets or sets the value of this filter.
		/// </summary>
		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#region Object overrides

		public override object Clone()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Equals(WorklistSingleValuedFilter<T> that)
		{
			if (that == null) return false;
			return Equals(_value, that._value) && Equals(this.IsEnabled, that.IsEnabled);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as WorklistSingleValuedFilter<T>);
		}

		public override int GetHashCode()
		{
			return (_value != null ? _value.GetHashCode() : 0) + 29 * IsEnabled.GetHashCode();
		}

		#endregion
	}

	/// <summary>
	/// Abstract base-class for multi-valued filters.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class WorklistMultiValuedFilter<T> : WorklistFilter, IEquatable<WorklistMultiValuedFilter<T>>
	{
		private ISet<T> _values;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistMultiValuedFilter()
		{
			_values = new HashedSet<T>();
		}

		/// <summary>
		/// Gets the set of values for this filter.
		/// </summary>
		public ISet<T> Values
		{
			get { return _values; }
			// private setter for NHibernate compatibility
			private set { _values = value; }
		}

		#region Object overrides

		public override object Clone()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Equals(WorklistMultiValuedFilter<T> that)
		{
			if (that == null) return false;
			return Equals(IsEnabled, that.IsEnabled) && Equals(_values, that._values);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as WorklistMultiValuedFilter<T>);
		}

		public override int GetHashCode()
		{
			return _values.GetHashCode() + 29 * IsEnabled.GetHashCode();
		}

		#endregion
	}

	/// <summary>
	/// Defines a filter that limits worklist items to procedures that fall into a specified
	/// set of <see cref="ProcedureTypeGroup"/>s.
	/// </summary>
	public class WorklistProcedureTypeGroupFilter : WorklistMultiValuedFilter<ProcedureTypeGroup>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items to procedures that are to performed
	/// at specified <see cref="Facility"/>s, or at the current working facility.
	/// </summary>
	public class WorklistFacilityFilter : WorklistMultiValuedFilter<Facility>
	{
		private bool _includeWorkingFacility;

		/// <summary>
		/// Gets or sets a value indicating whether the current working facility should be included
		/// in the worklist query.
		/// </summary>
		public bool IncludeWorkingFacility
		{
			get { return _includeWorkingFacility; }
			set { _includeWorkingFacility = value; }
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those visits that fall into a specified
	/// set of <see cref="PatientClassEnum"/> values.
	/// </summary>
	public class WorklistPatientClassFilter : WorklistMultiValuedFilter<PatientClassEnum>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those visits where the patient location falls into a specified
	/// set of <see cref="Location"/> values.
	/// </summary>
	public class WorklistPatientLocationFilter : WorklistMultiValuedFilter<Location>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those orders that fall into a specified
	/// set of <see cref="OrderPriorityEnum"/> values.
	/// </summary>
	public class WorklistOrderPriorityFilter : WorklistMultiValuedFilter<OrderPriorityEnum>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items based on a specified 
	/// set of <see cref="ExternalPractitioner"/> values.
	/// </summary>
	public class WorklistPractitionerFilter : WorklistMultiValuedFilter<ExternalPractitioner>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those procedures are either portable
	/// or non-portable, according to the <see cref="Procedure.Portable"/> property.
	/// </summary>
	public class WorklistPortableFilter : WorklistSingleValuedFilter<bool>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those that fall into a specified
	/// time-range.
	/// </summary>
	public class WorklistTimeFilter : WorklistSingleValuedFilter<WorklistTimeRange>
	{
	}

	/// <summary>
	/// Defines a filter that limits worklist items based on a specified 
	/// set of <see cref="Staff"/> values.
	/// </summary>
	public class WorklistStaffFilter : WorklistMultiValuedFilter<Staff>
	{
		private bool _includeCurrentStaff;

		/// <summary>
		/// Gets or sets a value indicating whether the current staff should be included
		/// in the worklist query.
		/// </summary>
		public bool IncludeCurrentStaff
		{
			get { return _includeCurrentStaff; }
			set { _includeCurrentStaff = value; }
		}
	}
}
