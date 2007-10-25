#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implements the DICOM concept of a Modality LUT.
	/// </summary>
	/// <remarks>
	/// For efficiency's sake, <see cref="ModalityLutLinear"/> is implemented as a <see cref="GeneratedDataLut"/>
	/// although it could also be a purely calculated Lut.  See the comments for <see cref="IModalityLut"/>
	/// for more information.
	/// </remarks>
	internal sealed class ModalityLutLinear : GeneratedDataLut, IModalityLut, IEquatable<ModalityLutLinear>
	{
		private readonly int _bitsStored;
		private readonly bool _isSigned;
		private double _rescaleSlope;
		private double _rescaleIntercept;

		/// <summary>
		/// Initializes a new instance of ModalityLUTLinear with the specified parameters.
		/// </summary>
		/// <param name="bitsStored">Indicates the number of bits stored of the associated pixel data.</param>
		/// <param name="isSigned">Indicates whether or not the associated pixel data is signed.</param>
		/// <param name="rescaleSlope">The rescale slope.</param>
		/// <param name="rescaleIntercept">The rescale intercept.</param>
		public ModalityLutLinear(
			int bitsStored,
			bool isSigned, 
			double rescaleSlope,
			double rescaleIntercept)
			: base()
		{
			DicomValidator.ValidateBitsStored(bitsStored);

			_bitsStored = bitsStored;
			_isSigned = isSigned;
			this.RescaleSlope = rescaleSlope;
			this.RescaleIntercept = rescaleIntercept;

			Initialize();
		}

		private void Initialize()
		{
			if (this.IsSigned)
			{
				base.MinInputValue = -(1 << (this.BitsStored - 1));
				base.MaxInputValue = (1 << (this.BitsStored - 1)) - 1;
			}
			else
			{
				base.MinInputValue = 0;
				base.MaxInputValue = (1 << this.BitsStored) - 1;
			}

			int minMax1 = (int)(this.RescaleSlope*this.MinInputValue + this.RescaleIntercept);
			int minMax2 = (int) (this.RescaleSlope*this.MaxInputValue + this.RescaleIntercept);

			base.MinOutputValue = (int)Math.Min(minMax1, minMax2);
			base.MaxOutputValue = (int)Math.Max(minMax1, minMax2);
		}

		private int BitsStored
		{
			get { return _bitsStored; }
		}

		private bool IsSigned
		{
			get { return _isSigned; }
		}

		private double RescaleSlope
		{
			get { return _rescaleSlope; }
			set
			{
				if (value <= double.Epsilon || double.IsNaN(value))
					_rescaleSlope = 1;
				else
					_rescaleSlope = value;
			}
		}

		private double RescaleIntercept
		{
			get { return _rescaleIntercept; }
			set
			{
				if (double.IsNaN(value))
					_rescaleIntercept = 0;
				else
					_rescaleIntercept = value;
			}
		}

		protected override void Create()
		{
			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				base[i] = (int) (this.RescaleSlope * i + this.RescaleIntercept);
			}
		}

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3:F2}",
				this.BitsStored,
				this.IsSigned.ToString(),
				this.RescaleSlope,
				this.RescaleIntercept);
		}

		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionModalityLutLinear, _rescaleSlope, _rescaleIntercept);
		}

		#region IEquatable<IModalityLut> Members

		public bool Equals(IModalityLut other)
		{
			if (this == other)
				return true;

			if (other is ModalityLutLinear)
				return this.Equals(other as ModalityLutLinear);

			return false;
		}

		#endregion

		#region IEquatable<ModalityLutLinear> Members

		public bool Equals(ModalityLutLinear other)
		{
			return
				this.BitsStored == other.BitsStored && this.IsSigned == other.IsSigned && this.RescaleSlope == other.RescaleSlope &&
				this.RescaleIntercept == other.RescaleIntercept;
		}

		#endregion
	}
}