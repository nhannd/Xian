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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Represents a window centre/width pair, with accompanying descriptive explanation.
	/// </summary>
	public class VoiWindow : IEquatable<VoiWindow>
	{
		private readonly double _width;
		private readonly double _center;
		private readonly string _explanation;

		/// <summary>
		/// Constructs a new instance of a <see cref="VoiWindow"/> with no explanation.
		/// </summary>
		/// <param name="width">The window width.</param>
		/// <param name="center">The window centre.</param>
		public VoiWindow(double width, double center) : this(width, center, string.Empty) {}

		/// <summary>
		/// Constructs a new instance of a <see cref="VoiWindow"/>.
		/// </summary>
		/// <param name="width">The window width.</param>
		/// <param name="center">The window centre.</param>
		/// <param name="explanation">A descriptive explanation for the window.</param>
		public VoiWindow(double width, double center, string explanation)
		{
			_width = width;
			_center = center;
			_explanation = explanation ?? string.Empty;
		}

		/// <summary>
		/// Gets the window width.
		/// </summary>
		public double Width
		{
			get { return _width; }
		}

		/// <summary>
		/// Gets the window centre.
		/// </summary>
		public double Center
		{
			get { return _center; }
		}

		/// <summary>
		/// Gets a descriptive explanation for the window.
		/// </summary>
		public string Explanation
		{
			get { return _explanation; }
		}

		/// <summary>
		/// Determines whether or not the current window has the same width and centre as another window.
		/// </summary>
		/// <param name="other">The other window.</param>
		/// <returns>True if the window width and centre are the same.</returns>
		public bool Equals(VoiWindow other)
		{
			return (_width == other._width && _center == other._center);
		}

		/// <summary>
		/// Determines whether or not the current window has the same width and centre as another window.
		/// </summary>
		/// <param name="obj">The other window.</param>
		/// <returns>True if the window width and centre are the same.</returns>
		public override bool Equals(object obj)
		{
			if (obj is VoiWindow)
				return this.Equals((VoiWindow) obj);
			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return _width.GetHashCode() ^ _center.GetHashCode() ^ -0x4B056F4A;
		}

		/// <summary>
		/// Formats the window width and centre as a string.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"{0:F2}/{1:F2}", _width, _center);
		}

		/// <summary>
		/// Converts the specified window into a <see cref="Window">DICOM window IOD</see>.
		/// </summary>
		public static explicit operator Window(VoiWindow window)
		{
			return new Window(window._width, window._center);
		}

		/// <summary>
		/// Converts a window from the specified <see cref="Window">DICOM window IOD</see>.
		/// </summary>
		public static explicit operator VoiWindow(Window window)
		{
			return new VoiWindow(window.Width, window.Center);
		}

		/// <summary>
		/// Gets an enumeration of <see cref="VoiWindow"/>s.
		/// </summary>
		/// <param name="centers">An enumeration of window centres.</param>
		/// <param name="widths">An enumeration of window widths.</param>
		/// <returns>An enumeation of <see cref="VoiWindow"/>s.</returns>
		public static IEnumerable<VoiWindow> GetWindows(IEnumerable<double> centers, IEnumerable<double> widths)
		{
			return GetWindows(centers, widths, null);
		}

		/// <summary>
		/// Gets an enumeration of <see cref="VoiWindow"/>s.
		/// </summary>
		/// <param name="centers">An enumeration of window centres.</param>
		/// <param name="widths">An enumeration of window widths.</param>
		/// <param name="explanations">An enumeration of window explanations.</param>
		/// <returns>An enumeation of <see cref="VoiWindow"/>s.</returns>
		public static IEnumerable<VoiWindow> GetWindows(IEnumerable<double> centers, IEnumerable<double> widths, IEnumerable<string> explanations)
		{
			if (centers == null || widths == null)
				yield break;

			double[] windowCenters = new List<double>(centers).ToArray();
			double[] windowWidths = new List<double>(widths).ToArray();

			List<string> windowExplanations = new List<string>();
			if (explanations != null)
				windowExplanations.AddRange(explanations);

			foreach (VoiWindow window in GetWindows(windowCenters, windowWidths, windowExplanations.ToArray()))
				yield return window;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="VoiWindow"/>s defined in the specified data source..
		/// </summary>
		/// <param name="dataset">A DICOM data source.</param>
		/// <returns>An enumeation of <see cref="VoiWindow"/>s.</returns>
		public static IEnumerable<VoiWindow> GetWindows(IDicomAttributeProvider dataset)
		{
			string windowCenterValues = dataset[DicomTags.WindowCenter].ToString();

			if (string.IsNullOrEmpty(windowCenterValues))
				yield break;

			string windowWidthValues = dataset[DicomTags.WindowWidth].ToString();

			if (string.IsNullOrEmpty(windowWidthValues))
				yield break;

			string windowExplanationValues = dataset[DicomTags.WindowCenterWidthExplanation].ToString();

			double[] windowCenters;
			DicomStringHelper.TryGetDoubleArray(windowCenterValues, out windowCenters);

			double[] windowWidths;
			DicomStringHelper.TryGetDoubleArray(windowWidthValues, out windowWidths);

			string[] windowExplanations = DicomStringHelper.GetStringArray(windowExplanationValues);

			foreach (VoiWindow window in GetWindows(windowCenters, windowWidths, windowExplanations))
				yield return window;
		}

		private static IEnumerable<VoiWindow> GetWindows(double[] windowCenters, double[] windowWidths, string[] windowExplanations)
		{
			if (windowCenters.Length == windowWidths.Length)
			{
				for (int i = 0; i < windowWidths.Length; ++i)
				{
					if (i < windowExplanations.Length)
						yield return new VoiWindow(windowWidths[i], windowCenters[i], windowExplanations[i]);
					else
						yield return new VoiWindow(windowWidths[i], windowCenters[i]);
				}
			}
		}
	}
}