using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	public partial class CopySubsetToClipboardComponent
	{
		internal class RangeImageSelectionStrategy : IImageSelectionStrategy
		{
			private readonly int _startValue;
			private readonly int _endValue;
			private readonly int _selectionInterval;
			private readonly bool _useInstanceNumbers;

			public RangeImageSelectionStrategy(int startValue, int endValue, int selectionInterval, bool useInstanceNumbers)
			{
				Platform.CheckPositive(startValue, "startValue");
				Platform.CheckPositive(endValue, "endValue");
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
			private readonly List<int> _ranges;
			private readonly bool _useInstanceNumbers;

			public CustomImageSelectionStrategy(string custom, int rangeMin, int rangeMax, bool useInstanceNumbers)
			{
				Platform.CheckForEmptyString(custom, "custom");

				if (!Parse(custom, rangeMin, rangeMax, out _ranges))
					throw new ArgumentException(String.Format("Invalid custom range string ({0}).", custom));

				_useInstanceNumbers = useInstanceNumbers;
			}

			public static bool Parse(string customRange, int rangeMin, int rangeMax, out List<int> rangeValues)
			{
				customRange = (customRange ?? "").Trim();
				rangeValues = new List<int>();

				if (rangeMin > rangeMax)
					return false;

				if (String.IsNullOrEmpty(customRange))
					return false;

				string[] ranges = customRange.Split(new char[] { ',' }, StringSplitOptions.None);
				foreach (string range in ranges)
				{
					if (String.IsNullOrEmpty(range))
					{
						rangeValues.Clear();
						return false;
					}

					int start, end;
					if (!ParseRange(range, rangeMin, rangeMax, out start, out end))
					{
						rangeValues.Clear();
						return false;
					}

					rangeValues.Add(start);
					rangeValues.Add(end);
				}

				return true;
			}

			private static bool ParseRange(string range, int rangeMin, int rangeMax, out int start, out int end)
			{
				start = end = -1;

				string[] subRange = range.Trim().Split(new char[] { '-' }, StringSplitOptions.None);
				if (subRange.Length == 0 || subRange.Length > 2)
					return false;

				if (subRange.Length == 1)
				{
					if (!int.TryParse(subRange[0], out start))
						return false;

					end = start;
				}
				else if (subRange.Length == 2)
				{
					string s1 = subRange[0].Trim();
					string s2 = subRange[1].Trim();

					bool startValid = String.IsNullOrEmpty(s1) || int.TryParse(s1, out start);
					bool endValid = String.IsNullOrEmpty(s2) || int.TryParse(s2, out end);

					if ((!startValid && !endValid) || (String.IsNullOrEmpty(s1) && String.IsNullOrEmpty(s2)))
						return false;

					if (String.IsNullOrEmpty(s1))
						start = rangeMin;
					else if (String.IsNullOrEmpty(s2))
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
								for (int i = 0; i < _ranges.Count - 1; i += 2)
								{
									if (instanceNumber >= _ranges[i] && instanceNumber <= _ranges[i + 1])
										images.Add(image);
								}
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < _ranges.Count - 1; i += 2)
					{
						for (int j = _ranges[i] - 1; j <= _ranges[i + 1] - 1; ++j)
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