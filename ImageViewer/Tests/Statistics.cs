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
using ClearCanvas.ImageViewer.Mathematics;

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

namespace ClearCanvas.ImageViewer.Tests
{
	internal class Statistics
	{
		public readonly double Mean;
		public readonly double StandardDeviation;
		public readonly double MaxValue;
		public readonly double MinValue;

		public Statistics(IEnumerable<double> data)
		{
			this.Mean = CalculateMean(data);
			this.StandardDeviation = CalculateStandardDeviation(this.Mean, data);
			this.MaxValue = double.MaxValue;
			this.MinValue = double.MinValue;
		}

		public Statistics(IEnumerable<int> data)
		{
			this.Mean = CalculateMean(Enumerate(data));
			this.StandardDeviation = CalculateStandardDeviation(this.Mean, Enumerate(data));
			this.MaxValue = int.MaxValue;
			this.MinValue = int.MinValue;
		}

		public bool IsEqualTo(double value)
		{
			if (FloatComparer.AreEqual((float) this.Mean, (float) value))
			{
				if (FloatComparer.AreEqual((float) this.StandardDeviation, 0))
					return true;
			}
			return false;
		}

		public bool IsEqualTo(double value, double tolerance)
		{
			if (FloatComparer.AreEqual((float) this.Mean, (float) value, (float) tolerance))
			{
				if (FloatComparer.AreEqual((float) this.StandardDeviation, 0, (float) tolerance))
					return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("\u03BC={0:f3}  \u03C3={1:f3}", this.Mean, this.StandardDeviation);
		}

		private static IEnumerable<double> Enumerate(IEnumerable<int> data)
		{
			foreach (int value in data)
				yield return value;
		}

		private static double CalculateMean(IEnumerable<double> data)
		{
			double sum = 0;
			int count = 0;

			foreach (double value in data)
			{
				count++;
				sum += value;
			}

			if (count == 0)
				return 0;

			return sum/count;
		}

		private static double CalculateStandardDeviation(double mean, IEnumerable<double> data)
		{
			double sum = 0;
			int count = 0;

			foreach (double value in data)
			{
				count++;
				double deviation = value - mean;
				sum += deviation*deviation;
			}

			if (count == 0)
				return 0;

			return Math.Sqrt(sum/count);
		}
	}
}

#endif