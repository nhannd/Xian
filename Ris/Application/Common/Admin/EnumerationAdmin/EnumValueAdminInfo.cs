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
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
	[DataContract]
	public class EnumValueAdminInfo : DataContractBase, ICloneable, IEquatable<EnumValueAdminInfo>
	{
		public EnumValueAdminInfo(string code, string value, string description, bool deactivated)
		{
			this.Code = code;
			this.Value = value;
			this.Description = description;
			this.Deactivated = deactivated;
		}

		public EnumValueAdminInfo()
		{
		}

		[DataMember]
		public string Code;

		[DataMember]
		public string Value;

		[DataMember]
		public string Description;

		[DataMember]
		public bool Deactivated;


		#region ICloneable Members

		public object Clone()
		{
			return new EnumValueAdminInfo(this.Code, this.Value, this.Description, this.Deactivated);
		}

		#endregion

		#region IEquatable<EnumValueAdminInfo>

		public bool Equals(EnumValueAdminInfo enumValueInfo)
		{
			if (ReferenceEquals(this, enumValueInfo)) return true;
			if (enumValueInfo == null) return false;
			if (!Equals(Code, enumValueInfo.Code)) return false;
			return true;
		}

		#endregion

		#region Object overrides

		public override bool Equals(object obj)
		{
			return Equals(obj as EnumValueAdminInfo);
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode();
		}

		/// <summary>
		/// Return the display value
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Value;
		}

		#endregion
	}
}
