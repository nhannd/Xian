// Code taken directly and unmodified from http://www.windojitsu.com/code/floatcomparer.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using dbg = System.Diagnostics.Debug;

namespace ClearCanvas.Common
{
	public class FloatComparer : System.Collections.IComparer
	{
		int System.Collections.IComparer.Compare(object x, object y)
		{
			// Handle special cases (nulls, wrong types, etc).
			if (x == null && y == null) return 0;

			if (x == null)
				if (y is float) return -1;
				else throw new ArgumentException("Object must be of type Single.", "y");
			if (y == null)
				if (x is float) return +1;
				else throw new ArgumentException("Object must be of type Single.", "x");

			// Compare the floats' distance in units of least-precise bits.
			const int tolerance = 5; // a pretty robust default
			return Compare((float)x, (float)y, tolerance);
		}

		public static int Compare(float x, float y, int tolerance)
		{
			int dummy;
			return Compare(x, y, tolerance, out dummy);
		}

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

		public static bool AreEqual(float x, float y, float tolerance)
		{
			return Math.Abs(x - y) < tolerance;
		}

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