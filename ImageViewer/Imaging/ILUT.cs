#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IModalityLut : IComposableLut
	{
		new int MinInputValue { get; set; }
		new int MaxInputValue { get; set; }

		double this[int input] { get; }

		new IModalityLut Clone();
	}

	[Cloneable(true)]
	public abstract class ComposableModalityLut : ComposableLut, IModalityLut
	{
		public abstract int MinInputValue { get; set; }
		public abstract int MaxInputValue { get; set; }
		public abstract double MinOutputValue { get; protected set; }
		public abstract double MaxOutputValue { get; protected set; }
		public abstract double this[int input] { get; }

		protected override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) value; }
		}

		protected override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) value; }
		}

		protected override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
			set { MinOutputValue = value; }
		}

		protected override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
			set { MaxOutputValue = value; }
		}

		protected override sealed double Lookup(double input)
		{
			return this[(int) Math.Round(input)];
		}

		public new IModalityLut Clone()
		{
			return (IModalityLut) base.Clone();
		}
	}

	public interface IVoiLut : IComposableLut
	{
		new int MinOutputValue { get; }
		new int MaxOutputValue { get; }

		new int this[double input] { get; }

		new IVoiLut Clone();
	}

	[Cloneable(true)]
	public abstract class ComposableVoiLut : ComposableLut, IVoiLut
	{
		public abstract double MinInputValue { get; set; }
		public abstract double MaxInputValue { get; set; }
		public abstract int MinOutputValue { get; protected set; }
		public abstract int MaxOutputValue { get; protected set; }
		public abstract int this[double input] { get; }

		protected override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = value; }
		}

		protected override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = value; }
		}

		protected override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
			set { MinOutputValue = (int) value; }
		}

		protected override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
			set { MaxOutputValue = (int) value; }
		}

		protected override sealed double Lookup(double input)
		{
			return this[input];
		}

		public new IVoiLut Clone()
		{
			return (IVoiLut) base.Clone();
		}
	}

	[Cloneable(true)]
	public abstract class DataVoiLut : DataLut, IVoiLut
	{
		double IComposableLut.MinInputValue
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) Math.Round(value); }
		}

		double IComposableLut.MaxInputValue
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) Math.Round(value); }
		}

		double IComposableLut.MinOutputValue
		{
			get { return MinOutputValue; }
		}

		double IComposableLut.MaxOutputValue
		{
			get { return MaxOutputValue; }
		}

		int IVoiLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		double IComposableLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		public new IVoiLut Clone()
		{
			return (IVoiLut) base.Clone();
		}

		IComposableLut IComposableLut.Clone()
		{
			return Clone();
		}
	}

	[Cloneable]
	internal sealed class PETLinearNormalizationLut : ComposableLut
	{
		[CloneIgnore]
		private readonly double _inverseSlope;

		[CloneIgnore]
		private readonly double _inverseIntercept;

		/// <summary>
		/// Initializes a new instance of <see cref="PETLinearNormalizationLut"/>.
		/// </summary>
		/// <param name="rescaleSlope">Original frame rescale slope to be inverted.</param>
		/// <param name="rescaleIntercept">Original frame rescale intercept to be inverted.</param>
		public PETLinearNormalizationLut(double rescaleSlope, double rescaleIntercept)
		{
			_inverseSlope = 1/rescaleSlope;
			_inverseIntercept = -rescaleIntercept/rescaleSlope;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private PETLinearNormalizationLut(PETLinearNormalizationLut source, ICloningContext context)
		{
			_inverseSlope = source._inverseSlope;
			_inverseIntercept = source._inverseIntercept;
		}

		protected override double MinInputValueCore { get; set; }

		protected override double MaxInputValueCore { get; set; }

		protected override double MinOutputValueCore
		{
			get { return Lookup(MinInputValueCore); }
			set { throw new NotSupportedException(); }
		}

		protected override double MaxOutputValueCore
		{
			get { return Lookup(MaxInputValueCore); }
			set { throw new NotSupportedException(); }
		}

		protected override double Lookup(double input)
		{
			return input*_inverseSlope + _inverseIntercept;
		}

		public override string GetKey()
		{
			return string.Format(@"PETNorm_{0}_{1}", _inverseSlope, _inverseIntercept);
		}

		public override string GetDescription()
		{
			return string.Format(@"PET Normalization Function m={0} b={1}", _inverseSlope, _inverseIntercept);
		}
	}
}