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
		private bool _recalculate = true;
		private ComposedLutCache.ICachedLut _cachedLut;
		private int _minInputValue = int.MinValue;
		private int _maxInputValue = int.MaxValue;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		public LutComposer()
		{
			LutCollection.ItemAdded += OnLutAdded;
			LutCollection.ItemChanging += OnLutChanging;
			LutCollection.ItemChanged += OnLutChanged;
			LutCollection.ItemRemoved += OnLutRemoved;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		/// <param name="minInputValue">The smallest input value that can be used to perform a lookup in the composed table.</param>
		/// <param name="maxInputValue">The largest input value that can be used to perform a lookup in the composed table.</param>
		public LutComposer(int minInputValue, int maxInputValue) : this()
		{
			_minInputValue = minInputValue;
			_maxInputValue = maxInputValue;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="LutComposer"/>.
		/// </summary>
		/// <param name="inputBits">The number of bits used by the input values to the composed lookup table.</param>
		/// <param name="inputIsSigned">A value indicating whether or not the input values are signed.</param>
		public LutComposer(int inputBits, bool inputIsSigned) : this()
		{
			if (inputIsSigned)
			{
				_minInputValue = -(1 << (inputBits - 1));
				_maxInputValue = (1 << (inputBits - 1)) - 1;
			}
			else
			{
				_minInputValue = 0;
				_maxInputValue = (1 << inputBits) - 1;
			}
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

		private void OnLutChanged()
		{
			SyncMinMaxValues();
			_recalculate = true;
		}

		private void SyncMinMaxValues()
		{
			if (LutCollection.Count > 0)
			{
				IComposableLut firstLut = LutCollection[0];
				firstLut.MinInputValue = _minInputValue;
				firstLut.MaxInputValue = _maxInputValue;
			}

			LutCollection.SyncMinMaxValues();
		}

		private void DisposeCachedLut()
		{
			if (_cachedLut != null)
			{
				_cachedLut.Dispose();
				_cachedLut = null;
			}
		}

		#region Event Handlers

		private void OnLutChanging(object sender, ListEventArgs<IComposableLut> e)
		{
			e.Item.LutChanged -= OnLutValuesChanged;
		}

		private void OnLutChanged(object sender, ListEventArgs<IComposableLut> e)
		{
			e.Item.LutChanged += OnLutValuesChanged;
			OnLutChanged();
		}

		private void OnLutRemoved(object sender, ListEventArgs<IComposableLut> e)
		{
			e.Item.LutChanged -= OnLutValuesChanged;
			OnLutChanged();
		}

		private void OnLutAdded(object sender, ListEventArgs<IComposableLut> e)
		{
			e.Item.LutChanged += OnLutValuesChanged;
			OnLutChanged();
		}

		private void OnLutValuesChanged(object sender, EventArgs e)
		{
			OnLutChanged();
		}

		#endregion

		#region Properties
		/// <summary>
		/// The output LUT of the pipeline.
		/// </summary>
		private IComposedLut ComposedLut
		{
			get
			{
				if (_recalculate)
				{
					DisposeCachedLut();
					_recalculate = false;
				}

				if (_cachedLut == null)
					_cachedLut = ComposedLutCache.GetLut(LutCollection);

				return _cachedLut;
			}
		}

		private IComposableLut LastLut
		{
			get
			{
				LutCollection.Validate();
				return LutCollection[LutCollection.Count - 1];
			}
		}

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
			get { return ComposedLut.Data; }
		}

		#endregion

		#region ILut Members

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		public int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (_minInputValue != value)
				{
					_minInputValue = value;
					OnLutChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		public int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (_maxInputValue != value)
				{
					_maxInputValue = value;
					OnLutChanged();
				}
			}
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
			get { return ComposedLut[index]; }
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
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Debug, e);
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
				LutCollection.ItemAdded -= OnLutAdded;
				LutCollection.ItemChanging -= OnLutChanging;
				LutCollection.ItemChanged -= OnLutChanged;
				LutCollection.ItemRemoved -= OnLutRemoved;

				DisposeCachedLut();

				if (_lutCollection != null)
					_lutCollection.Clear();
			}
		}

		#endregion
		#endregion
	}
}
