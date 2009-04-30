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
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a member of an enumeration.
	/// </summary>
	[DataContract]
	public class EnumerationMemberInfo : ElementInfo
	{
		private string _code;
		private string _value;
		private string _description;
		private float _displayOrder;
		private bool _deactivated;

		internal EnumerationMemberInfo()
		{
		}


		internal EnumerationMemberInfo(string code, string value, string description, float displayOrder, bool deactivated)
		{
			_code = code;
			_value = value;
			_description = description;
			_displayOrder = displayOrder;
			_deactivated = deactivated;
		}

		[DataMember]
		public string Code
		{
			get { return _code; }
			private set { _code = value; }
		}

		[DataMember]
		public string Value
		{
			get { return _value; }
			private set { _value = value; }
		}

		[DataMember]
		public string Description
		{
			get { return _description; }
			private set { _description = value; }
		}

		[DataMember]
		public float DisplayOrder
		{
			get { return _displayOrder; }
			private set { _displayOrder = value; }
		}

		[DataMember]
		public bool Deactivated
		{
			get { return _deactivated; }
			private set { _deactivated = value; }
		}

		public override string Identity
		{
			get { return _code; }
		}
	}

	/// <summary>
	/// Describes an enumeration that is defined as part of a relational model.
	/// </summary>
	[DataContract]
	public class EnumerationInfo : ElementInfo
	{
		private string _enumerationClass;
		private List<EnumerationMemberInfo> _members;
		private bool _isHard;
		private string _table;

		internal EnumerationInfo()
		{

		}

		internal EnumerationInfo(string enumerationClass, string table, bool isHard, List<EnumerationMemberInfo> members)
		{
			_enumerationClass = enumerationClass;
			_members = members;
			_isHard = isHard;
			_table = table;
		}

		/// <summary>
		/// Gets the .NET class name of the enumeration.
		/// </summary>
		[DataMember]
		public string EnumerationClass
		{
			get { return _enumerationClass; }
			private set { _enumerationClass = value; }
		}

		/// <summary>
		/// Gets the name of the table that holds the enumeration values.
		/// </summary>
		[DataMember]
		public string Table
		{
			get { return _table; }
			private set { _table = value; }
		}

		/// <summary>
		/// Gets the set of member values of the enumeration.
		/// </summary>
		[DataMember]
		public List<EnumerationMemberInfo> Members
		{
			get { return _members; }
			private set { _members = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this enumeration is 'hard' vs. 'soft'.
		/// </summary>
		[DataMember]
		public bool IsHard
		{
			get { return _isHard; }
			private set { _isHard = value; }
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
		{
			get { return _enumerationClass; }
		}
	}
}
