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

namespace ClearCanvas.ImageViewer.Externals.General
{
	public interface IArgumentHint : IDisposable
	{
		ArgumentHintValue this[string key] { get; }
	}

	public struct ArgumentHintValue : IEquatable<ArgumentHintValue>, IComparable<ArgumentHintValue>
	{
		private readonly string[] _values;

		public ArgumentHintValue(string value)
		{
			_values = null;
			if (value != null)
			{
				_values = new string[] {value};
			}
		}

		public ArgumentHintValue(string[] values)
		{
			_values = null;
			if (values != null && values.Length > 0)
			{
				_values = new string[values.Length];
				values.CopyTo(_values, 0);
			}
		}

		public ArgumentHintValue(IEnumerable<string> values)
		{
			_values = null;
			if (values != null)
			{
				List<string> valueList = new List<string>(values);
				if (valueList.Count > 0)
					_values = valueList.ToArray();
			}
		}

		public bool IsMultiValued
		{
			get { return _values != null && _values.Length > 1; }
		}

		public bool IsNull
		{
			get { return _values == null; }
		}

		public int Count
		{
			get
			{
				if (_values == null)
					return 0;
				return _values.Length;
			}
		}

		public override int GetHashCode()
		{
			int hashCode = 0x1925C138;
			if (_values != null)
			{
				foreach (string value in _values)
				{
					if (value != null)
						hashCode ^= value.GetHashCode();
					else
						hashCode ^= 0x229A3E06;
				}
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj is ArgumentHintValue)
				return this.Equals((ArgumentHintValue) obj);
			return false;
		}

		public bool Equals(ArgumentHintValue other)
		{
			if (ReferenceEquals(_values, other._values) || (_values == null && other._values == null))
				return true;
			if (_values == null || other._values == null)
				return false;

			int small = Math.Min(_values.Length, other._values.Length);
			for (int n = 0; n < small; n++)
			{
				int result = string.Compare(_values[n], other._values[n]);
				if (result != 0)
					return false;
			}

			return _values.Length.Equals(other._values.Length);
		}

		public int CompareTo(ArgumentHintValue other)
		{
			if (ReferenceEquals(_values, other._values) || (_values == null && other._values == null))
				return 0;
			if (_values == null)
				return -1;
			if (other._values == null)
				return +1;

			int small = Math.Min(_values.Length, other._values.Length);
			for (int n = 0; n < small; n++)
			{
				int result = string.Compare(_values[n], other._values[n]);
				if (result != 0)
					return result;
			}

			return _values.Length.CompareTo(other._values.Length);
		}

		public override string ToString()
		{
			if (this.IsNull)
				return string.Empty;
			if (this.IsMultiValued)
				return _values.ToString();
			return _values[0];
		}

		public string ToString(string multiValueSeparator)
		{
			if (this.IsNull)
				return string.Empty;
			return string.Join(multiValueSeparator, _values);
		}

		public static implicit operator string(ArgumentHintValue value)
		{
			if (value._values == null)
				return null;
			return value._values[0];
		}

		public static implicit operator string[](ArgumentHintValue value)
		{
			return value._values;
		}

		public static readonly ArgumentHintValue Empty = new ArgumentHintValue();
	}
}