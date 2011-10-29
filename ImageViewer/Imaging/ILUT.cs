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

		internal override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) value; }
		}

		internal override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) value; }
		}

		internal override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
			set { MinOutputValue = value; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
			set { MaxOutputValue = value; }
		}

		internal override sealed double Lookup(double input)
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

		internal override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = value; }
		}

		internal override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = value; }
		}

		internal override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
			set { MinOutputValue = (int) value; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
			set { MaxOutputValue = (int) value; }
		}

		internal override sealed double Lookup(double input)
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
}