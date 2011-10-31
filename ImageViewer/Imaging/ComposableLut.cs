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
	[Cloneable(true)]
	public abstract class ComposableLut : ComposableLutBase
	{
		public abstract double MinInputValue { get; set; }
		public abstract double MaxInputValue { get; set; }
		public abstract double MinOutputValue { get; protected set; }
		public abstract double MaxOutputValue { get; protected set; }
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
			set { MinOutputValue = value; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
			set { MaxOutputValue = value; }
		}

		internal override sealed double Lookup(double input)
		{
			return this[input];
		}
	}
}