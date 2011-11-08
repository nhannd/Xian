#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract base implementation for a lookup table in the standard grayscale image display pipeline that selects range from manufacturer-independent values for display.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IVoiLut"/>
	[Cloneable(true)]
	public abstract class ComposableVoiLut : ComposableLutBase, IVoiLut
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
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
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
}