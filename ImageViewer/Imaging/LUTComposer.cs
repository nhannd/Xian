using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Allows <see cref="ILUT"/> objects
	/// be composed together in a pipeline.
	/// </summary>
	public class LutComposer : IDisposable
	{
		private LutCollection _lutCollection;
		private int _numEntries;
		private OutputLut _outputLut;
		private int _minInputValue;
		private int _maxInputValue;
		private bool _recalculate = true;
		private bool _validated = false;

		private OutputLutPool _lutPool;
		private string _key = String.Empty;

		/// <summary>
		/// Initializes a new instance of <see cref="LUTComposer"/>.
		/// </summary>
		public LutComposer()
		{
			this.LutCollection.ItemAdded += new EventHandler<LutEventArgs>(OnLutAdded);
			this.LutCollection.ItemChanging += new EventHandler<LutEventArgs>(OnLutChanging); 
			this.LutCollection.ItemChanged += new EventHandler<LutEventArgs>(OnLutChanged);
		}

		/// <summary>
		/// A collection of <see cref="ILUT"/> objects.
		/// </summary>
		public LutCollection LutCollection
		{
			get 
			{ 
				if (_lutCollection == null)
					_lutCollection = new LutCollection();

				return _lutCollection; 
			}
		}

		/// <summary>
		/// The output LUT of the pipeline.
		/// </summary>
		public int[] OutputLut
		{
			get 
			{
				if (_recalculate)
				{
					if (!_validated)
					{
						SetInputRange();
						ValidateLutCollection();
						_validated = true;
					}

					this.LutPool.Return(_key);

					_key = GetKey();

					bool composeRequired;
					_outputLut = this.LutPool.Retrieve(_key, _numEntries, out composeRequired);

					if (composeRequired)
						Compose();

					_recalculate = false;
				}

				return _outputLut.Lut; 
			}
		}

		private OutputLutPool LutPool
		{
			get
			{
				if (_lutPool == null)
					_lutPool = OutputLutPool.NewInstance;

				return _lutPool;
			}
		}

		private void Compose()
		{

#if DEBUG
			CodeClock counter = new CodeClock();
			counter.Start();
#endif
			int val;

			for (int i = _minInputValue; i <= _maxInputValue; i++)
			{
				val = i;

				for (int j = 0; j < this.LutCollection.Count; j++)
				{
					ILut lut = this.LutCollection[j];
					val = lut[val];
				}

				if (i >= 0)
					_outputLut.Lut[i] = val;
				else
					_outputLut.Lut[i + _numEntries] = val;
			}

#if DEBUG
			counter.Stop();

			string str = String.Format("Compose: {0}\n", counter.ToString());
			Trace.Write(str);
#endif
		}

		private void SetInputRange()
		{
			ILut lut = this.LutCollection[0];
			_minInputValue = lut.MinInputValue;
			_maxInputValue = lut.MaxInputValue;
			_numEntries = _maxInputValue - _minInputValue + 1;
		}

		private void ValidateLutCollection()
		{
			// Make sure we have at least one LUT
			if (this.LutCollection.Count == 0)
				throw new InvalidOperationException(SR.ExceptionLUTNotAdded);

			// Check for null LUTs
			foreach (ILut lut in this.LutCollection)
			{
				if (lut == null)
					throw new InvalidOperationException(SR.ExceptionLUTNotAdded);
			}

			// If we only have one LUT then no further validation is required
			if (this.LutCollection.Count == 1)
				return;

			// Verify that the input range of the nth LUT is equal to the output
			// range of the n-1th LUT.
			for (int i = 1; i < this.LutCollection.Count; i++)
			{
				ILut curLut = this.LutCollection[i];
				ILut prevLut = this.LutCollection[i - 1];

				if (prevLut.MinOutputValue != curLut.MinInputValue ||
					prevLut.MaxOutputValue != curLut.MaxInputValue)
					throw new InvalidOperationException(SR.ExceptionLUTInputOutputRange);
			}

			//Verify that the last LUT is a PresentationLUT
			int lastLUT = this.LutCollection.Count - 1;

			if (!(this.LutCollection[lastLUT] is IPresentationLut))
			    throw new InvalidOperationException("Last LUT in pipeline must be an IPresentationLUT");
		}

		private string GetKey()
		{
			StringBuilder key = new StringBuilder();

			foreach (ILut lut in this.LutCollection)
			{
				key.Append(lut.GetKey());
			}

			return key.ToString();
		}

		void OnLutChanging(object sender, LutEventArgs e)
		{
			e.Lut.LutChanged -= new EventHandler(OnLutValuesChanged);
		}

		void OnLutChanged(object sender, LutEventArgs e)
		{
			_recalculate = true;
			OnLutAdded(sender, e);
		}

		void OnLutAdded(object sender, LutEventArgs e)
		{
			e.Lut.LutChanged += new EventHandler(OnLutValuesChanged);
			_validated = false;
		}

		void OnLutValuesChanged(object sender, EventArgs e)
		{
			_recalculate = true;
		}

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lutPool != null)
					_lutPool.Dispose();

				_outputLut = null;
			}
		}

		#endregion
	}
}
