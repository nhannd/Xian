#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Represents the result of a HP match operation.
	/// </summary>
	public struct HpMatchResult : IEquatable<HpMatchResult>
	{
		/// <summary>
		/// The <see cref="HpMatchResult"/> value representing a non-match.
		/// </summary>
		public static HpMatchResult Negative = new HpMatchResult(false, 0);

		/// <summary>
		/// The <see cref="HpMatchResult"/> value, representing a match with a quality of zero.
		/// </summary>
		public static HpMatchResult Zero = new HpMatchResult(true, 0);

		/// <summary>
		/// The <see cref="HpMatchResult"/> value, representing a match with a quality of one.
		/// </summary>
		public static HpMatchResult Positive = new HpMatchResult(true, 1);

		/// <summary>
		/// Computes a result representing the sum of the specified sequence of results.
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		public static HpMatchResult Sum(IEnumerable<HpMatchResult> results)
		{
			var sum = Zero;
			foreach (var score in results)
			{
				sum = sum + score;
			}
			return sum;
		}


		private readonly int _quality;
		private readonly bool _match;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="match"></param>
		/// <param name="quality"></param>
		private HpMatchResult(bool match, int quality)
		{
			_match = match;
			_quality = quality;
		}

		/// <summary>
		/// Gets a value indicating whether this result represents a match or not.
		/// </summary>
		public bool IsMatch
		{
			get { return _match; }
		}

		/// <summary>
		/// Gets a value indicating the quality of this match (assuming that <see cref="IsMatch"/> returns true.
		/// </summary>
		/// <exception cref="InvalidOperationException">If this property is invoked when <see cref="IsMatch"/> returns false.</exception>
		public int Quality
		{
			get
			{
				if(!_match)
					throw new InvalidOperationException("A non-match does not have a quality value.");
				return _quality;
			}
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.
		///                 </param>
		public bool Equals(HpMatchResult other)
		{
			// all non-matches are equal, regardless of score
			return _match == false ? other._match == false : _quality == other._quality;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		/// <param name="obj">Another object to compare to. 
		///                 </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof(HpMatchResult)) return false;
			return Equals((HpMatchResult)obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				return _match == false ? _match.GetHashCode() : (_quality * 397) ^ _match.GetHashCode();
			}
		}

		/// <summary>
		/// Determines if two <see cref="HpMatchResult"/> have the same value.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(HpMatchResult left, HpMatchResult right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Determines if two <see cref="HpMatchResult"/> have a different value.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(HpMatchResult left, HpMatchResult right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Computes an <see cref="HpMatchResult"/> representing the sum of two <see cref="HpMatchResult"/>.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static HpMatchResult operator +(HpMatchResult x, HpMatchResult y)
		{
			return x.IsMatch && y.IsMatch ? new HpMatchResult(true, x.Quality + y.Quality) : Negative;
		}
	}
}
