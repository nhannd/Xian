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
		public abstract double this[int input] { get; }

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
		new int this[double input] { get; }

		new IVoiLut Clone();
	}

	[Cloneable(true)]
	public abstract class ComposableVoiLut : ComposableLut, IVoiLut
	{
		public abstract int this[double input] { get; }

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