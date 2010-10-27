#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// Code taken directly and unmodified from http://www.windojitsu.com/code/floatcomparer.html

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using dbg = System.Diagnostics.Debug;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A utility class to facilitate comparison of floats.
	/// </summary>
	public class FloatComparer
	{
		/// <summary>
		/// Compares two floats with a specified tolerance.
		/// </summary>
		/// <param name="x">First float to compare.</param>
		/// <param name="y">Second float to compare.</param>
		/// <param name="tolerance">The tolerance expressed in terms
		/// of the number of floats above or below <paramref name="y"/>.
		/// </param>
		/// <returns>
		/// <list>
		/// <item> 0 if x - y &lt; tolerance</item>
		/// <item>+1 if x - y &gt; tolerance</item>
		/// <item>-1 if x - y &lt; -tolerance</item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Look <a href="http://www.windojitsu.com/code/floatcomparer.html">here</a>
		/// for details on floating point comparison.
		/// </remarks>
		public static int Compare(float x, float y, int tolerance)
		{
			int dummy;
			return Compare(x, y, tolerance, out dummy);
		}

		/// <summary>
		/// Compares two floats with a specified tolerance.
		/// </summary>
		/// <param name="x">First float to compare.</param>
		/// <param name="y">Second float to compare.</param>
		/// <param name="tolerance">The tolerance expressed in terms
		/// of the number of floats above or below <paramref name="y"/>.
		/// </param>
		/// <param name="difference">The number of floats between 
		/// <paramref name="x"/> and <paramref name="y"/>.</param>
		/// <returns>
		/// <list>
		/// <item> 0 if x - y &lt; tolerance</item>
		/// <item>+1 if x - y &gt; tolerance</item>
		/// <item>-1 if x - y &lt; -tolerance</item>
		/// </list>
		/// </returns>
		public static int Compare(float x, float y, int tolerance, out int difference)
		{
			// Make sure maxUlps is non-negative and small enough that the
			// default NAN won't compare as equal to anything.
			dbg.Assert(tolerance >= 0 && tolerance < 4 * 1024 * 1024);
			
			// Reinterpret float bits as sign-magnitude integers.
			int xi = BitReinterpreter.Convert(x);
			int yi = BitReinterpreter.Convert(y);

			// Convert from sign-magnitude to two's complement form, 
			// by subtracting from 0x80000000.
			if (xi < 0)
				xi = int.MinValue - xi;
			if (yi < 0)
				yi = int.MinValue - yi;

			// How many epsilons apart?
			difference = xi - yi;

			// Is the difference outside our tolerance?
			if (xi > yi + tolerance)
				return +1;
			if (xi < yi - tolerance)
				return -1;
			else
				return 0;
		}

		/// <summary>
		/// Returns a value indicating whether two values are equal
		/// within a specified absolute tolerance.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		public static bool AreEqual(float x, float y, float tolerance)
		{
			return Math.Abs(x - y) < tolerance;
		}

		/// <summary>
		/// Returns a value indicating whether two values are equal
		/// within a specified tolerance in units of number of floats.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// Uses <see cref="Compare(float,float,int)"/> to perform
		/// the comparison.  Assumes a tolerance of 100.
		/// </remarks>
		public static bool AreEqual(float x, float y)
		{
			int dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);

			if (result == 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns a value indicating whether x is greater than y.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// Uses <see cref="Compare(float,float,int)"/> to perform
		/// the comparison.  Assumes a tolerance of 100.
		/// </remarks>
		public static bool IsGreaterThan(float x, float y)
		{
			int dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);

			if (result == 1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns a value indicating whether x is less than y.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// Uses <see cref="Compare(float,float,int)"/> to perform
		/// the comparison.  Assumes a tolerance of 100.
		/// </remarks>
		public static bool IsLessThan(float x, float y)
		{
			int dummy;
			const int tolerance = 100;

			int result = Compare(x, y, tolerance, out dummy);

			if (result == -1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns a value indicating whether two points are equal.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		/// <remarks>
		/// Uses <see cref="Compare(float,float,int)"/> to perform
		/// the comparison.  Assumes a tolerance of 100.
		/// </remarks>
		public static bool AreEqual(PointF pt1, PointF pt2)
		{
			int dummy;
			const int tolerance = 100;

			int xResult = Compare(pt1.X, pt2.X, tolerance, out dummy);
			int yResult = Compare(pt1.Y, pt2.Y, tolerance, out dummy);

			if (xResult == 0 && yResult == 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns a value indicating whether two sizes are equal.
		/// </summary>
		/// <param name="size1"></param>
		/// <param name="size2"></param>
		/// <returns></returns>
		/// <remarks>
		/// Uses <see cref="Compare(float,float,int)"/> to perform
		/// the comparison.  Assumes a tolerance of 100.
		/// </remarks>
		public static bool AreEqual(SizeF size1, SizeF size2)
		{
			int dummy;
			const int tolerance = 100;

			int xResult = Compare(size1.Width, size2.Width, tolerance, out dummy);
			int yResult = Compare(size1.Height, size2.Height, tolerance, out dummy);

			if (xResult == 0 && yResult == 0)
				return true;
			else
				return false;
		}

		//
		// Implementation

		[StructLayout(LayoutKind.Explicit)]
		internal struct BitReinterpreter
		{
			public static int Convert(float f)
			{
				BitReinterpreter br = new BitReinterpreter(f);
				return br.i;
			}

			public static float Convert(int i)
			{
				BitReinterpreter br = new BitReinterpreter(i);
				return br.f;
			}

			[FieldOffset(0)]
			float f;
			[FieldOffset(0)]
			int i;

			private BitReinterpreter(float f)
			{ this.i = 0; this.f = f; }

			private BitReinterpreter(int i)
			{ this.f = 0; this.i = i; }
		}
	}
}