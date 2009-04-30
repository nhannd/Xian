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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Allows <see cref="IComposableLut"/> objects to be composed together in a pipeline.
	/// </summary>
	public class LutComposer : IComposedLut, IDisposable
	{
		#region Private Fields

		private LutCollection _lutCollection;
		private int[] _composedLut;
		private bool _recalculate = true;
		private bool _validated = false;

		private ComposedLutPool _lutPool;
		private string _key = String.Empty;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		public LutComposer()
		{
			this.LutCollection.ItemAdded += new EventHandler<ListEventArgs<IComposableLut>>(OnLutAdded);
			this.LutCollection.ItemChanging += new EventHandler<ListEventArgs<IComposableLut>>(OnLutChanging);
			this.LutCollection.ItemChanged += new EventHandler<ListEventArgs<IComposableLut>>(OnLutChanged);
		}

		#region Public Properties

		/// <summary>
		/// A collection of <see cref="IComposableLut"/> objects.
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

		#endregion

		#region Private Members
		#region Methods

		private unsafe void Compose(int[] data)
		{

#if DEBUG
			CodeClock counter = new CodeClock();
			counter.Start();
#endif
			fixed (int* composedLutData = data)
			{
				int* pLutData = composedLutData;
				int min = MinInputValue;
				int max = MaxInputValue + 1;
				int lutCount = LutCollection.Count;

				for (int i = min; i < max; ++i)
				{
					int val = i;

					for (int j = 0; j < lutCount; ++j)
						val = this.LutCollection[j][val];

					*pLutData = val;
					++pLutData;
				}
			}
#if DEBUG
			counter.Stop();

			string str = String.Format("Compose: {0}\n", counter.ToString());
			Trace.Write(str);
#endif
		}

		private void ValidateLutCollection()
		{
			// Make sure we have at least one LUT
			if (this.LutCollection.Count == 0)
				throw new InvalidOperationException(SR.ExceptionLUTNotAdded);

			// Check for null LUTs
			foreach (IComposableLut lut in this.LutCollection)
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
				IComposableLut curLut = this.LutCollection[i];
				IComposableLut prevLut = this.LutCollection[i - 1];

				if (prevLut.MinOutputValue != curLut.MinInputValue ||
					prevLut.MaxOutputValue != curLut.MaxInputValue)
					throw new InvalidOperationException(SR.ExceptionLUTInputOutputRange);
			}
		}

		private IEnumerable<string> GetKeys()
		{
			foreach (IComposableLut lut in this.LutCollection)
				yield return lut.GetKey();
		}

		private string GetKey()
		{
			return StringUtilities.Combine(GetKeys(), "/");
		}

		private void SyncMinMaxValues()
		{
			for (int i = 1; i < this.LutCollection.Count; ++i)
			{
				IComposableLut curLut = this.LutCollection[i];
				IComposableLut prevLut = this.LutCollection[i - 1];

				curLut.MinInputValue = prevLut.MinOutputValue;
				curLut.MaxInputValue = prevLut.MaxOutputValue;
			}
		}

		#region Event Handlers

		private void OnLutChanging(object sender, ListEventArgs<IComposableLut> e)
		{
			e.Item.LutChanged -= new EventHandler(OnLutValuesChanged);
		}

		private void OnLutChanged(object sender, ListEventArgs<IComposableLut> e)
		{
			_recalculate = true;
			SyncMinMaxValues();
			OnLutAdded(sender, e);
		}

		private void OnLutAdded(object sender, ListEventArgs<IComposableLut> e)
		{
			e.Item.LutChanged += new EventHandler(OnLutValuesChanged);
			SyncMinMaxValues();
			_validated = false;
		}

		private void OnLutValuesChanged(object sender, EventArgs e)
		{
			_recalculate = true;
			SyncMinMaxValues();
		}

		#endregion
		#endregion

		#region Properties

		private ComposedLutPool LutPool
		{
			get
			{
				if (_lutPool == null)
					_lutPool = ComposedLutPool.NewInstance;

				return _lutPool;
			}
		}

		/// <summary>
		/// The output LUT of the pipeline.
		/// </summary>
		private int[] ComposedLut
		{
			get 
			{
				if (_recalculate)
				{
					if (!_validated)
					{
						ValidateLutCollection();
						_validated = true;
					}

					this.LutPool.Return(_key);

					_key = GetKey();

					_composedLut = this.LutPool.Retrieve(_key, Length, Compose);
					_recalculate = false;
				}

				return _composedLut; 
			}
		}

		private int Length
		{
			get { return MaxInputValue - MinInputValue + 1; }
		}

		private IComposableLut FirstLut
		{
			get
			{
				ValidateLutCollection();
				return LutCollection[0];
			}
		}

		private IComposableLut LastLut
		{
			get
			{
				ValidateLutCollection();
				return LutCollection[LutCollection.Count - 1];
			}
		}

		#endregion
		#endregion

		#region IComposedLut Members

		/// <summary>
		/// Gets the composed lut data.
		/// </summary>
		/// <remarks>
		/// This property should be considered readonly and is only 
		/// provided for fast (unsafe) iteration over the array.
		/// </remarks>
		public int[] Data
		{
			get
			{
				return ComposedLut;
			}
		}

		#endregion

		#region ILut Members

		/// <summary>
		/// Gets the minimum input value.
		/// </summary>
		public int MinInputValue
		{
			get { return FirstLut.MinInputValue; }
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		public int MaxInputValue
		{
			get { return FirstLut.MaxInputValue; }
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public int MinOutputValue
		{
			get { return LastLut.MinOutputValue; }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public int MaxOutputValue
		{
			get { return LastLut.MaxOutputValue; }
		}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		public int this[int index]
		{
			get
			{
				if (index <= MinInputValue)
					return this.ComposedLut[0];
				else if (index >= MaxInputValue)
					return this.ComposedLut[Length - 1];

				return this.ComposedLut[index - MinInputValue];
			}
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				this.LutCollection.ItemAdded -= new EventHandler<ListEventArgs<IComposableLut>>(OnLutAdded);
				this.LutCollection.ItemChanging -= new EventHandler<ListEventArgs<IComposableLut>>(OnLutChanging);
				this.LutCollection.ItemChanged -= new EventHandler<ListEventArgs<IComposableLut>>(OnLutChanged);

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
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lutCollection != null)
					_lutCollection.Clear();

				if (_lutPool != null)
					_lutPool.Dispose();

				_composedLut = null;
			}
		}

		#endregion
		#endregion
	}
}
