#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Iod
{
	public struct CIELabColor
	{
		private ushort _l;
		private ushort _a;
		private ushort _b;

		public CIELabColor(ushort l, ushort a, ushort b)
		{
			_l = l;
			_a = a;
			_b = b;
		}

		public ushort L
		{
			get { return _l; }
			set { _l = value; }
		}

		public ushort A
		{
			get { return _a; }
			set { _a = value; }
		}

		public ushort B
		{
			get { return _b; }
			set { _b = value; }
		}

		public ushort[] ToArray()
		{
			return new ushort[] {_l, _a, _b};
		}
	}
}
