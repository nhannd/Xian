using System;
using System.Xml;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Enterprise.Core
{


	/// <summary>
	/// ExtendedPropertyValue component
	/// </summary>
	[Serializable]
	public class ExtendedPropertyValue : IEquatable<ExtendedPropertyValue>, ICloneable
	{
		private const int MaxSmallValueLength = 255;

		public string Value { get; private set; }
		public string SmallValue { get; private set; }


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
			return Value;
		}

		public bool GetBoolean()
		{
			return bool.Parse(Value);
		}

		public int GetInteger()
		{
			return int.Parse(Value);
		}

		public DateTime? GetDateTime()
		{
			return string.IsNullOrEmpty(Value) ? null : DateTimeUtils.ParseISO(Value);
		}

		public XmlDocument GetXml()
		{
			var xml = new XmlDocument();
			xml.LoadXml(Value);
			return xml;
		}

		#region IEquatable methods

		public bool Equals(ExtendedPropertyValue that)
		{
			return that != null && this.Value == that.Value;
		}

		#endregion

		#region Object overrides

		public override string ToString()
		{
			return Value;
		}

		public override bool Equals(object that)
		{
			return this.Equals(that as ExtendedPropertyValue);
		}

		public override int GetHashCode()
		{
			return Value == null ? 0 : Value.GetHashCode();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new ExtendedPropertyValue(Value);
		}

		#endregion

		private void SetInternal(string value)
		{
			Value = value;
			SmallValue = value == null ? null : (value.Length <= MaxSmallValueLength ? value : value.Substring(0, MaxSmallValueLength));
		}
	}
}