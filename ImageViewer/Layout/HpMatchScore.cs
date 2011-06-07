#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Layout
{
	public class HpMatchScore
	{
		private readonly int _value;

		public HpMatchScore(int value)
		{
			_value = value;
		}

		public bool IsMatch
		{
			get { return _value > 0; }
		}

		public int Value
		{
			get { return _value; }
		}
	}
}
