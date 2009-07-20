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
using System.Text.RegularExpressions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public struct DicomAge : IComparable<DicomAge>, IComparable, IEquatable<DicomAge>
	{
		private static readonly Regex _parserPattern = new Regex("^(\\d{3})([DWMY])$", RegexOptions.Compiled);
		public static readonly DicomAge MaxValue = new DicomAge(999, DicomAgeUnits.Years);
		public static readonly DicomAge MinValue = new DicomAge(0, DicomAgeUnits.Days);
		public static readonly DicomAge Zero = new DicomAge(0, DicomAgeUnits.Days);
		public const double DaysPerYear = 365 + 97/400d; // average days per year (factoring in leap days over a 400 year period)
		public const double DaysPerMonth = DaysPerYear/12; // average days per month (factoring in leap days over a 400 year period)
		public const double DaysPerWeek = 7.0; // exactly 7 days per week

		private readonly int _count;
		private readonly DicomAgeUnits _units;

		public DicomAge(int count, DicomAgeUnits units)
		{
			Platform.CheckArgumentRange(count, 0, 999, "count");

			_count = count;
			_units = units;
		}

		public int Count
		{
			get { return _count; }
		}

		public DicomAgeUnits Units
		{
			get { return _units; }
		}

		public double TotalDays
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count;
					case DicomAgeUnits.Weeks:
						return _count/DaysPerWeek;
					case DicomAgeUnits.Months:
						return _count/DaysPerMonth;
					case DicomAgeUnits.Years:
					default:
						return _count/DaysPerYear;
				}
			}
		}

		public double TotalWeeks
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count*DaysPerWeek;
					case DicomAgeUnits.Weeks:
						return _count;
					case DicomAgeUnits.Months:
						return _count*DaysPerWeek/DaysPerMonth;
					case DicomAgeUnits.Years:
					default:
						return _count*DaysPerWeek/DaysPerYear;
				}
			}
		}

		public double TotalMonths
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count*DaysPerMonth;
					case DicomAgeUnits.Weeks:
						return _count*DaysPerMonth/DaysPerWeek;
					case DicomAgeUnits.Months:
						return _count;
					case DicomAgeUnits.Years:
					default:
						return _count*DaysPerMonth/DaysPerYear;
				}
			}
		}

		public double TotalYears
		{
			get
			{
				switch (_units)
				{
					case DicomAgeUnits.Days:
						return _count*DaysPerYear;
					case DicomAgeUnits.Weeks:
						return _count*DaysPerYear/DaysPerWeek;
					case DicomAgeUnits.Months:
						return _count*DaysPerYear/DaysPerMonth;
					case DicomAgeUnits.Years:
					default:
						return _count;
				}
			}
		}

		#region Conversion Members

		public static implicit operator TimeSpan (DicomAge x)
		{
			return new TimeSpan((int) x.TotalDays, 0, 0, 0);
		}

		public static implicit operator DicomAge (TimeSpan x)
		{
			return new DicomAge(x.Days, DicomAgeUnits.Days);
		}

		#endregion

		#region Hashing Members

		public override int GetHashCode()
		{
			return -0x15A760B5 ^ _count.GetHashCode() ^ _units.GetHashCode();
		}

		#endregion

		#region Comparison Members

		public int CompareTo(DicomAge other)
		{
			return this.TotalDays.CompareTo(other.TotalDays);
		}

		public int CompareTo(object obj)
		{
			if (obj is DicomAge)
				return this.CompareTo((DicomAge) obj);
			throw new ArgumentException("Parameter must be a DicomAge.", "obj");
		}

		public bool Equals(DicomAge other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is DicomAge)
				return this.Equals((DicomAge) obj);
			return false;
		}

		#endregion

		#region Formatting Members

		public string ToString(string format)
		{
			switch(format.ToLowerInvariant())
			{
				case "d":
					return this.ToString(DicomAgeUnits.Days);
				case "w":
					return this.ToString(DicomAgeUnits.Weeks);
				case "m":
					return this.ToString(DicomAgeUnits.Months);
				case "y":
					return this.ToString(DicomAgeUnits.Years);
				default:
					throw new FormatException("Invalid format specifier.");
			}
		}

		public string ToString(DicomAgeUnits units)
		{
			switch (units)
			{
				case DicomAgeUnits.Days:
					return string.Format("{0:000}D", (int) this.TotalDays);
				case DicomAgeUnits.Weeks:
					return string.Format("{0:000}W", (int) this.TotalWeeks);
				case DicomAgeUnits.Months:
					return string.Format("{0:000}M", (int) this.TotalMonths);
				default:
				case DicomAgeUnits.Years:
					return string.Format("{0:000}Y", (int) this.TotalYears);
			}
		}

		public override string ToString()
		{
			return this.ToString(_units);
		}

		#endregion

		#region Parsing Members

		public static DicomAge Parse(string s)
		{
			DicomAge age;
			if (TryParse(s, out age))
				return age;
			throw new FormatException("Parameter was not in the expected format.");
		}

		public static bool TryParse(string s, out DicomAge dicomAge)
		{
			if (s != null)
				s = s.Trim();

			dicomAge = Zero;

			Match m = _parserPattern.Match(s);
			if (!m.Success)
				return false;

			int value;
			int.TryParse(m.Groups[1].Value, out value);
			switch (m.Groups[2].Value)
			{
				case "D":
					dicomAge = new DicomAge(value, DicomAgeUnits.Days);
					break;
				case "W":
					dicomAge = new DicomAge(value, DicomAgeUnits.Weeks);
					break;
				case "M":
					dicomAge = new DicomAge(value, DicomAgeUnits.Months);
					break;
				case "Y":
				default:
					dicomAge = new DicomAge(value, DicomAgeUnits.Years);
					break;
			}
			return true;
		}

		#endregion

		#region Comparison Operators

		public static bool operator ==(DicomAge x, DicomAge y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(DicomAge x, DicomAge y)
		{
			return !x.Equals(y);
		}

		public static bool operator >(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) > 0;
		}

		public static bool operator <(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) < 0;
		}

		public static bool operator >=(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) >= 0;
		}

		public static bool operator <=(DicomAge x, DicomAge y)
		{
			return x.CompareTo(y) <= 0;
		}

		#endregion
	}

	public enum DicomAgeUnits
	{
		Days,
		Weeks,
		Months,
		Years
	}
}