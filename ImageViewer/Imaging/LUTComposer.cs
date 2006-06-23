using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Summary description for LUTComposer.
	/// </summary>
	public class LUTComposer
	{
		private IList<IGrayscaleLUT> _lutCollection = new List<IGrayscaleLUT>();
		private int _numEntries;
		private byte[] _outputLUT;
		private int _minInputValue;
		private int _maxInputValue;
		private bool _invert;

		public LUTComposer()
		{
		}

		public IList<IGrayscaleLUT> LUTCollection
		{
			get { return _lutCollection; }
		}

		public byte[] OutputLUT
		{
			get { return _outputLUT; }
		}

		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		public void Compose()
		{
			ValidateLUTCollection();
			CreateLUT();

            CodeClock counter = new CodeClock();
			counter.Start();

			int val;

			for (int i = _minInputValue; i <= _maxInputValue; i++)
			{
				val = i;

				for (int j = 0; j < _lutCollection.Count; j++)
				{
					IGrayscaleLUT lut = _lutCollection[j];
					val = lut[val];
				}

				if (i >= 0)
				{
					if (_invert)
					{
						_outputLUT[i] = byte.MaxValue;
						_outputLUT[i] -= (byte)val;
					}
					else
					{
						_outputLUT[i] = (byte)val;
					}
				}
				else
				{
					if (_invert)
					{
						_outputLUT[i + _numEntries] = byte.MaxValue;
						_outputLUT[i + _numEntries] -= (byte)val;
					}
					else
					{
						_outputLUT[i + _numEntries] = (byte)val;
					}
				}
			}

			counter.Stop();

			string str = String.Format("Compose: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		private void CreateLUT()
		{
			IGrayscaleLUT lut = _lutCollection[0];

			// If the output LUT hasn't been created or the first LUT in the
			// collection has changed, create a new output LUT
			if (lut.NumEntries != _numEntries)
			{
				_minInputValue = lut.MinInputValue;
				_maxInputValue = lut.MaxInputValue;
				_numEntries = lut.NumEntries;
				_outputLUT = new byte[_numEntries];
			}
		}

		private bool IsLastLUTOutputRangeCorrect()
		{
			int lastIndex = _lutCollection.Count - 1;
			IGrayscaleLUT lastLUT = (IGrayscaleLUT)_lutCollection[lastIndex];

			if (lastLUT.MinOutputValue >= byte.MinValue && lastLUT.MaxOutputValue <= byte.MaxValue)
				return true;
			else
				return false;
		}

		private void ValidateLUTCollection()
		{
			// Make sure we have at least one LUT
			if (_lutCollection.Count == 0)
				throw new InvalidOperationException(SR.ExceptionLUTNotAdded);

			// Check for null LUTs
			foreach (IGrayscaleLUT lut in _lutCollection)
			{
				if (lut == null)
					throw new InvalidOperationException(SR.ExceptionLUTNotAdded);
			}

			// Make sure output range of last LUT in pipeline is 8-bits
			if (!IsLastLUTOutputRangeCorrect())
				throw new InvalidOperationException(SR.ExceptionLUTLastOutputRange);

			// If we only have one LUT then no further validation is required
			if (_lutCollection.Count == 1)
				return;

			// Verify that the input range of the nth LUT is equal to the output
			// range of the n-1th LUT.
			for (int i = 1; i < _lutCollection.Count; i++)
			{
				IGrayscaleLUT curLUT = _lutCollection[i];
				IGrayscaleLUT prevLUT = _lutCollection[i - 1];

				if (prevLUT.MinOutputValue != curLUT.MinInputValue ||
					prevLUT.MaxOutputValue != curLUT.MaxInputValue)
					throw new InvalidOperationException(SR.ExceptionLUTInputOutputRange);
			}
		}

	}
}
