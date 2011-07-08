#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Class that provides a list of standard modalities.
	/// </summary>
	/// <remarks>
	/// For reasons of consistency, the provided modality list should be used throughout the 
	/// application wherever a list of modalities is used/shown.
	/// </remarks>
	public static class StandardModalities
	{
		/// <summary>
		/// Gets a list of standard modalities.
		/// </summary>
		public static IList<string> Modalities
		{
			get { return StandardModalitySettings.Default.GetModalities().AsReadOnly(); }
		}
	}
}
