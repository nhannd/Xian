#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	public partial class CopySubsetToClipboardComponent
	{
		internal struct Range
		{
			public Range(int start, int end)
			{
				Start = start;
				End = end;
			}

			public readonly int Start;
			public readonly int End;
		}

		internal class RangeImageSelectionStrategy : IImageSelectionStrategy
		{
			private readonly int _startValue;
			private readonly int _endValue;
			private readonly int _selectionInterval;
			private readonly bool _useInstanceNumbers;

			public RangeImageSelectionStrategy(int startValue, int endValue, int selectionInterval, bool useInstanceNumbers)
			{
				if (!useInstanceNumbers)
				{
					Platform.CheckPositive(startValue, "startValue");
					Platform.CheckPositive(endValue, "endValue");
				}
				else
				{
					Platform.CheckNonNegative(startValue, "startValue");
					Platform.CheckNonNegative(endValue, "endValue");
				}

				Platform.CheckPositive(selectionInterval, "selectionInterval");

				if (endValue < startValue)
					throw new ArgumentException("End value must be greater than or equal to start value.");

				_startValue = startValue;
				_endValue = endValue;
				_selectionInterval = selectionInterval;
				_useInstanceNumbers = useInstanceNumbers;
			}

			#region IImageSubsetSelectionStrategy Members

			public string Description
			{
				get { return SR.DescriptionSubsetRange; }
			}

			public IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet)
			{
				if (_useInstanceNumbers)
				{
					foreach (IPresentationImage image in displaySet.PresentationImages)
					{
						if (image is IImageSopProvider)
						{
							IImageSopProvider provider = (IImageSopProvider)image;
							
							int maxValue = _endValue - _startValue;
							int testValue = provider.ImageSop.InstanceNumber - _startValue;

							if (testValue >= 0 && testValue <= maxValue && (0 == testValue % _selectionInterval))
								yield return image;
						}
					}
				}
				else
				{
					//selection indices are 1-based.
					for (int i = _startValue - 1; i < _endValue && i < displaySet.PresentationImages.Count; i += _selectionInterval)
						yield return displaySet.PresentationImages[i];
				}
			}

			#endregion
		}

		internal class CustomImageSelectionStrategy : IImageSelectionStrategy
		{
			private readonly List<Range> _ranges;
			private readonly bool _useInstanceNumbers;

			public CustomImageSelectionStrategy(string custom, int rangeMin, int rangeMax, bool useInstanceNumbers)
			{
				Platform.CheckForEmptyString(custom, "custom");

				if (!Parse(custom, rangeMin, rangeMax, out _ranges))
					throw new ArgumentException(String.Format("Invalid custom range string ({0}).", custom));

				_useInstanceNumbers = useInstanceNumbers;
			}

			public static bool Parse(string customRanges, int rangeMin, int rangeMax, out List<Range> parsedRanges)
			{
				customRanges = (customRanges ?? "").Trim();
				parsedRanges = new List<Range>();

				if (rangeMin > rangeMax)
					return false;

				if (String.IsNullOrEmpty(customRanges))
					return false;

				string[] ranges = customRanges.Split(new char[] { ',' }, StringSplitOptions.None);
				foreach (string range in ranges)
				{
					if (String.IsNullOrEmpty(range))
					{
						parsedRanges.Clear();
						return false;
					}

					int start, end;
					if (!ParseRange(range, rangeMin, rangeMax, out start, out end))
					{
						parsedRanges.Clear();
						return false;
					}

					parsedRanges.Add(new Range(start, end));
				}

				return true;
			}

			private static bool ParseRange(string range, int rangeMin, int rangeMax, out int start, out int end)
			{
				start = end = -1;

				string[] splitRange = range.Trim().Split(new char[] { '-' }, StringSplitOptions.None);
				if (splitRange.Length == 0 || splitRange.Length > 2)
					return false;

				if (splitRange.Length == 1)
				{
					if (!int.TryParse(splitRange[0], out start))
						return false;

					end = start;
				}
				else if (splitRange.Length == 2)
				{
					string splitStart = splitRange[0].Trim();
					string splitEnd = splitRange[1].Trim();

					bool startValid = String.IsNullOrEmpty(splitStart) || int.TryParse(splitStart, out start);
					bool endValid = String.IsNullOrEmpty(splitEnd) || int.TryParse(splitEnd, out end);

					if ((!startValid && !endValid) || (String.IsNullOrEmpty(splitStart) && String.IsNullOrEmpty(splitEnd)))
						return false;

					if (String.IsNullOrEmpty(splitStart))
						start = rangeMin;
					else if (String.IsNullOrEmpty(splitEnd))
						end = rangeMax;
				}

				return start > 0 && end >= start && end <= rangeMax;
			}

			#region IImageSubsetSelectionStrategy Members

			public string Description
			{
				get { return SR.DescriptionSubsetCustom; }
			}

			public IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet)
			{
				List<IPresentationImage> images = new List<IPresentationImage>();

				if (_useInstanceNumbers)
				{
					foreach (IPresentationImage image in displaySet.PresentationImages)
					{
						if (!images.Contains(image))
						{
							if (image is IImageSopProvider)
							{
								int instanceNumber = ((IImageSopProvider)image).ImageSop.InstanceNumber;
								foreach (Range range in _ranges)
								{
									if (instanceNumber >= range.Start && instanceNumber <= range.End)
										images.Add(image);
								}
							}
						}
					}
				}
				else
				{
					foreach (Range range in _ranges)
					{
						for (int j = range.Start - 1; j <= range.End - 1; ++j)
						{
							if (j >= displaySet.PresentationImages.Count)
								break;

							if (!images.Contains(displaySet.PresentationImages[j]))
								images.Add(displaySet.PresentationImages[j]);
						}
					}
				}

				return images;
			}

			#endregion
		}
	}
}