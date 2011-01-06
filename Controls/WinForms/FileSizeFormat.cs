#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Controls.WinForms
{
	/// <summary>
	/// Specifies the formatting style for displaying file sizes.
	/// </summary>
	public enum FileSizeFormat
	{
		/// <summary>
		/// Specifies that file sizes should be displayed in binary octet units using binary prefixes (i.e. KiB = 2<sup>10</sup> bytes, MiB = 2<sup>20</sup> bytes, etc.).
		/// </summary>
		BinaryOctets,

		/// <summary>
		/// Specifies that file sizes should be displayed in binary octet units using legacy prefixes (i.e. KB = 2<sup>10</sup> bytes, MB = 2<sup>20</sup> bytes, etc.).
		/// </summary>
		LegacyOctets,

		/// <summary>
		/// Specifies that file sizes should be displayed in metric octet units using SI prefixes (i.e. KB = 10<sup>3</sup> bytes, MB = 10<sup>6</sup> bytes, etc.).
		/// </summary>
		MetricOctets
	}
}