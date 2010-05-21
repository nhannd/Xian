#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public partial class InterpolatedColorMap : ColorMap
	{
		private readonly IList<KeyValuePair<float, Color>> _fixedNodes;
		private readonly string _name;

		public InterpolatedColorMap(string name, Color lowColor, Color highColor)
			: this(name, new KeyValuePair<float, Color>(0, lowColor), new KeyValuePair<float, Color>(1, highColor)) {}

		public InterpolatedColorMap(string name, params KeyValuePair<float, Color>[] fixedNodes)
			: this(name, (IEnumerable<KeyValuePair<float, Color>>) fixedNodes) {}

		public InterpolatedColorMap(string name, IEnumerable<KeyValuePair<float, Color>> fixedNodes)
		{
			List<KeyValuePair<float, Color>> list = new List<KeyValuePair<float, Color>>(fixedNodes);
			Platform.CheckTrue(list.Count >= 2, "There must be at least 2 fixed colour nodes.");

			list.Sort((x, y) => x.Key.CompareTo(y.Key));
			Platform.CheckTrue(list[0].Key >= 0, "Fixed colour nodes must be between 0% and 100% of full scale.");
			Platform.CheckTrue(FloatComparer.AreEqual(list[0].Key, 0), "A fixed colour node must be defined for 0% of full scale.");
			Platform.CheckTrue(list[list.Count - 1].Key <= 1, "Fixed colour nodes must be between 0% and 100% of full scale.");
			Platform.CheckTrue(FloatComparer.AreEqual(list[list.Count - 1].Key, 1), "A fixed colour node must be defined for 100% of full scale.");

			_fixedNodes = list.AsReadOnly();
			_name = name;
		}

		protected override void Create()
		{
			int valueIndex = 0;
			int valueFullScale = this.Length - 1;
			int min = MinInputValue;
			int max = MaxInputValue;

			KeyValuePair<float, Color> bucketStart = _fixedNodes[0];
			KeyValuePair<float, Color> bucketEnd = _fixedNodes[1];
			int bucketEndIndex = 1;
			float bucketRange = bucketEnd.Key - bucketStart.Key;

			for (int i = min; i <= max; i++)
			{
				float valueScale = valueIndex++/(float) valueFullScale;
				if (valueScale > bucketEnd.Key && bucketEndIndex + 1 < _fixedNodes.Count)
				{
					bucketStart = bucketEnd;
					bucketEnd = _fixedNodes[++bucketEndIndex];
					bucketRange = bucketEnd.Key - bucketStart.Key;
				}

				float colorScale = (valueScale - bucketStart.Key)/bucketRange;
				Color color = Color.FromArgb(255,
				                             (int) Interpolate(colorScale, bucketStart.Value.R, bucketEnd.Value.R),
				                             (int) Interpolate(colorScale, bucketStart.Value.G, bucketEnd.Value.G),
				                             (int) Interpolate(colorScale, bucketStart.Value.B, bucketEnd.Value.B));

				this[i] = color.ToArgb();
			}
		}

		public override string GetDescription()
		{
			return string.Format("{0}({1})", this.GetType().FullName, _name);
		}

		private static float Interpolate(float value, float low, float high)
		{
			return value*(high - low) + low;
		}
	}
}