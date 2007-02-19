using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class LUTComposer
	{
		private LUTCollection _lutCollection;
		private int _numEntries;
		private int[] _outputLUT;
		private int _minInputValue;
		private int _maxInputValue;

		public LUTComposer()
		{
		}

		public LUTCollection LUTCollection
		{
			get 
			{ 
				if (_lutCollection == null)
					_lutCollection = new LUTCollection();

				return _lutCollection; 
			}
		}

		public int[] OutputLUT
		{
			get { return _outputLUT; }
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

				for (int j = 0; j < this.LUTCollection.Count; j++)
				{
					IComposableLUT lut = this.LUTCollection[j];
					val = lut[val];
				}

				if (i >= 0)
					_outputLUT[i] = val;
				else
					_outputLUT[i + _numEntries] = val;
			}

			counter.Stop();

			string str = String.Format("Compose: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		private void CreateLUT()
		{
			IComposableLUT lut = this.LUTCollection[0];

			// If the output LUT hasn't been created or the first LUT in the
			// collection has changed, create a new output LUT
			if (lut.NumEntries != _numEntries)
			{
				_minInputValue = lut.MinInputValue;
				_maxInputValue = lut.MaxInputValue;
				_numEntries = lut.NumEntries;
				_outputLUT = new int[_numEntries];
			}
		}

		private void ValidateLUTCollection()
		{
			// Make sure we have at least one LUT
			if (this.LUTCollection.Count == 0)
				throw new InvalidOperationException(SR.ExceptionLUTNotAdded);

			// Check for null LUTs
			foreach (IComposableLUT lut in this.LUTCollection)
			{
				if (lut == null)
					throw new InvalidOperationException(SR.ExceptionLUTNotAdded);
			}

			// If we only have one LUT then no further validation is required
			if (this.LUTCollection.Count == 1)
				return;

			// Verify that the input range of the nth LUT is equal to the output
			// range of the n-1th LUT.
			for (int i = 1; i < this.LUTCollection.Count; i++)
			{
				IComposableLUT curLUT = this.LUTCollection[i];
				IComposableLUT prevLUT = this.LUTCollection[i - 1];

				if (prevLUT.MinOutputValue != curLUT.MinInputValue ||
					prevLUT.MaxOutputValue != curLUT.MaxInputValue)
					throw new InvalidOperationException(SR.ExceptionLUTInputOutputRange);
			}

			// Verify that the last LUT is a PresentationLUT
			int lastLUT = this.LUTCollection.Count - 1;

			if (!(this.LUTCollection[lastLUT] is PresentationLUT))
				throw new InvalidOperationException("Last LUT in pipeline must be a PresentationLUT");
		}
	}
}
