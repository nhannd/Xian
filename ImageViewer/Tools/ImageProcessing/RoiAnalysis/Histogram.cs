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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	public class Histogram
	{
		private int _numBins;
		private int _binWidth;
		private int _min;
		private int _max;

		private int[] _data;
		private int[] _binLabels;
		private int[] _bins;

		private bool _generate;
	
		public Histogram(int min, int max, int numBins, int[] data)
		{
			Platform.CheckPositive(numBins, "numBins");
			Platform.CheckForNullReference(data, "data");

			this.Min = min;
			this.Max = max;
			this.NumBins = numBins;
			_data = data;
		}

		public int NumBins
		{
			get { return _numBins; }
			set 
			{
				_numBins = value;
				_generate = true;
			}
		}

		public int Min
		{
			get { return _min; }
			set
			{
				_min = value;
				_generate = true;
			}
		}
		public int Max
		{
			get { return _max; }
			set
			{
				_max = value;
				_generate = true;
			}
		}

		public int[] Data
		{
			get { return _data; }
			set
			{
				_data = value;
				_generate = true;
			}
		}

		public int[] BinLabels
		{
			get
			{
				GenerateHistogram();

				int length = _binLabels.Length - 1;
				int[] labels = new int[length];
				Array.Copy(_binLabels, labels, length);
				return labels;
			}
		}

		public int[] Bins
		{
			get 
			{
				GenerateHistogram();
				return _bins; 
			}
		}

		//public void Calculate

		private void GenerateHistogram()
		{
			if (!_generate)
				return;

			_bins = new int[_numBins];

			GenerateBinLabels();

			for (int i = 0; i < _data.Length; i++)
			{
				for (int j = 0; j < _binLabels.Length - 1; j++)
				{
					if (_data[i] >= _binLabels[j] &&
						_data[i] <= _binLabels[j + 1])
					{
						_bins[j]++;
						break;
					}
				}
			}

			_generate = false;
		}

		private void GenerateBinLabels()
		{
			int range = _max - _min;
			_binWidth = range / _numBins;

			_binLabels = new int[_numBins + 1];

			for (int i = 0; i < _binLabels.Length; i++)
				_binLabels[i] = _min + i * _binWidth;
		}
	}
}
