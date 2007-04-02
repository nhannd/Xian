using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class OutputLUT : IReferenceCountable
	{
		private int _referenceCount = 0;
		private int[] _outputLUT;
		private string _key;

		public OutputLUT(string key, int numEntries)
		{
			_key = key;
			_outputLUT = new int[numEntries];
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public int[] LUT
		{
			get { return _outputLUT; }
		}

		#region IReferenceCountable Members

		public void IncrementReferenceCount()
		{
			_referenceCount++;
		}

		public void DecrementReferenceCount()
		{
			if (_referenceCount > 0)
				_referenceCount--;
		}

		public bool IsReferenceCountZero
		{
			get { return (_referenceCount == 0); }
		}

		public int ReferenceCount
		{
			get { return _referenceCount; }
		}

		#endregion
	}

	/// <summary>
	/// Allows <see cref="IComposableLUT"/> objects
	/// be composed together in a pipeline.
	/// </summary>
	public class LUTComposer
	{
		private LUTCollection _lutCollection;
		private int _numEntries;
		private OutputLUT _outputLUT;
		private int _minInputValue;
		private int _maxInputValue;
		private bool _recalculate = true;
		private bool _validated = false;

		private static OutputLUTPool _lutPool = new OutputLUTPool();
		private string _key = String.Empty;

		/// <summary>
		/// Initializes a new instance of <see cref="LUTComposer"/>.
		/// </summary>
		public LUTComposer()
		{
			this.LUTCollection.ItemAdded += new EventHandler<LUTEventArgs>(OnLUTAdded);
		}

		/// <summary>
		/// A collection of <see cref="IComposableLUT"/> objects.
		/// </summary>
		public LUTCollection LUTCollection
		{
			get 
			{ 
				if (_lutCollection == null)
					_lutCollection = new LUTCollection();

				return _lutCollection; 
			}
		}

		/// <summary>
		/// The output LUT of the pipeline.
		/// </summary>
		public int[] OutputLUT
		{
			get 
			{
				if (_recalculate)
				{
					if (!_validated)
					{
						SetInputRange();
						ValidateLUTCollection();
						_validated = true;
					}

					_lutPool.Return(_key);

					_key = GetKey();

					bool composeRequired;
					_outputLUT = _lutPool.Retrieve(_key, _numEntries, out composeRequired);

					if (composeRequired)
						Compose();

					_recalculate = false;
				}

				return _outputLUT.LUT; 
			}
		}

		private void Compose()
		{
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
					_outputLUT.LUT[i] = val;
				else
					_outputLUT.LUT[i + _numEntries] = val;
			}

			counter.Stop();

			string str = String.Format("Compose: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		private void SetInputRange()
		{
			IComposableLUT lut = this.LUTCollection[0];
			_minInputValue = lut.MinInputValue;
			_maxInputValue = lut.MaxInputValue;
			_numEntries = lut.Length;
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

		private string GetKey()
		{
			StringBuilder key = new StringBuilder();

			foreach (IComposableLUT lut in this.LUTCollection)
			{
				key.Append(lut.GetKey());
			}

			return key.ToString();
		}

		void OnLUTAdded(object sender, LUTEventArgs e)
		{
			e.Lut.LUTChanged += new EventHandler(OnLUTChanged);
			_validated = false;
		}

		void OnLUTChanged(object sender, EventArgs e)
		{
			_recalculate = true;
		}
	}
}
