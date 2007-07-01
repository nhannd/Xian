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
