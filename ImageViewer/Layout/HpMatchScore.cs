#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout
{
	public class HpMatchScore
	{
		public static HpMatchScore Sum(IEnumerable<HpMatchScore> scores)
		{
			var sum = 0;
			foreach (var score in scores)
			{
				sum += score.Value;
			}
			return new HpMatchScore(sum);
		}


		private readonly int _value;

		public HpMatchScore(int value)
		{
			_value = value;
		}

		public bool IsMatch
		{
			get { return _value >= 0; }
		}

		public int Value
		{
			get { return _value; }
		}
	}
}
