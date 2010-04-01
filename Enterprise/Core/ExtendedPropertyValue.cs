using System;
using System.Collections;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Enterprise.Core
{


	/// <summary>
	/// ExtendedPropertyValue component
	/// </summary>
	[Serializable]
	public partial class ExtendedPropertyValue : IEquatable<ExtendedPropertyValue>, ICloneable
	{
		private const int MaxSmallValueLength = 255;

		private string _value;
		private string _smallValue;

		public string Value { get { return _value; } }
		public string SmallValue { get { return _smallValue; } }


		public ExtendedPropertyValue(string value)
		{
			SetInternal(value);
		}

		public ExtendedPropertyValue(bool value)
		{
			SetInternal(value.ToString());
		}

		public ExtendedPropertyValue(int value)
		{
			SetInternal(value.ToString());
		}

		public ExtendedPropertyValue(DateTime? value)
		{
			SetInternal(value.HasValue ? DateTimeUtils.FormatISO(value.Value) : null);
		}

		public ExtendedPropertyValue(XmlDocument value)
		{
			// TODO: implement this using proper formatting, etc
			throw new NotImplementedException();
		}

		public string GetString()
		{
			return _value;
		}

		public bool GetBoolean()
		{
			return bool.Parse(_value);
		}

		public int GetInteger()
		{
			return int.Parse(_value);
		}

		public DateTime? GetDateTime()
		{
			return string.IsNullOrEmpty(_value) ? null : DateTimeUtils.ParseISO(_value);
		}

		public XmlDocument GetXml()
		{
			var xml = new XmlDocument();
			xml.LoadXml(_value);
			return xml;
		}

		#region IEquatable methods

		public bool Equals(ExtendedPropertyValue that)
		{
			return that != null && this._value == that._value;
		}

		#endregion

		#region Object overrides

		public override string ToString()
		{
			return _value;
		}

		public override bool Equals(object that)
		{
			return this.Equals(that as ExtendedPropertyValue);
		}

		public override int GetHashCode()
		{
			return _value == null ? 0 : _value.GetHashCode();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new ExtendedPropertyValue(_value);
		}

		#endregion

		private void SetInternal(string value)
		{
			_value = value;
			_smallValue = value.Length <= MaxSmallValueLength ? value : value.Substring(0, MaxSmallValueLength);
		}
	}
}