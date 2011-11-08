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
	/// <summary>
	/// Abstract base implementation for a lookup table in the standard grayscale image display pipeline that transforms stored pixel values to manufacturer-independent values.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IModalityLut"/>
	[Cloneable(true)]
	public abstract class ComposableModalityLut : ComposableLutBase, IModalityLut
	{
		public abstract int MinInputValue { get; set; }
		public abstract int MaxInputValue { get; set; }
		public abstract double MinOutputValue { get; protected set; }
		public abstract double MaxOutputValue { get; protected set; }
		public abstract double this[int input] { get; }

		internal override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = (int) Math.Round(value); }
		}

		internal override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = (int) Math.Round(value); }
		}

		internal override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
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
}