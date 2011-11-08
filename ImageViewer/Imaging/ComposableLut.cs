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
	/// Abstract base implementation for a lookup table in the standard grayscale image display pipeline that performs any additional transformation prior to selecting the VOI range.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IComposableLut"/>
	[Cloneable(true)]
	public abstract class ComposableLut : ComposableLutBase
	{
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public abstract double MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		public abstract double MaxInputValue { get; set; }

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		public abstract double MinOutputValue { get; protected set; }

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		public abstract double MaxOutputValue { get; protected set; }

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		public abstract double this[double input] { get; }

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
	}
}