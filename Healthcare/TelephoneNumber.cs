#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// TelephoneNumber component
	/// </summary>
	public partial class TelephoneNumber : IFormattable
	{
		private void CustomInitialize()
		{
		}

		public bool IsCurrent
		{
			get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
		}

		/// <summary>
		/// Equivalence comparison which ignores ValidRange
		/// </summary>
		/// <param name="that">The TelephoneNumber to compare to</param>
		/// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
		public bool IsSameNumber(TelephoneNumber that)
		{
			return (that != null) &&
				((this._countryCode == default(string)) ? (that._countryCode == default(string)) : this._countryCode.Equals(that._countryCode)) &&
				((this._areaCode == default(string)) ? (that._areaCode == default(string)) : this._areaCode.Equals(that._areaCode)) &&
				((this._number == default(string)) ? (that._number == default(string)) : this._number.Equals(that._number)) &&
				((this._extension == default(string)) ? (that._extension == default(string)) : this._extension.Equals(that._extension)) &&
				((this._use == default(TelephoneUse)) ? (that._use == default(TelephoneUse)) : this._use.Equals(that._use)) &&
				((this._equipment == default(TelephoneEquipment)) ? (that._equipment == default(TelephoneEquipment)) : this._equipment.Equals(that._equipment)) &&
				true;
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			// TODO interpret the format string according to custom-defined format characters
			var sb = new StringBuilder();
			sb.AppendFormat("{0} ", _countryCode);
			if (!String.IsNullOrEmpty(_areaCode))
			{
				sb.AppendFormat("({0}) ", _areaCode);
			}
			if (!String.IsNullOrEmpty(_number) && _number.Length == 7)
			{
				sb.AppendFormat("{0}-{1}", _number.Substring(0, 3), _number.Substring(3,4));
			}
			else
			{
				sb.AppendFormat("{0}", _number);
			}
			if (!String.IsNullOrEmpty(_extension))
			{
				sb.Append(" x");
				sb.Append(_extension);
			}
			return sb.ToString();
		}

		#endregion

		public override string ToString()
		{
			return this.ToString(null, null);
		}
	}
}