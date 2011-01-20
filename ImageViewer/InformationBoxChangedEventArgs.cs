#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="Tile.InformationBoxChanged"/> event.
	/// </summary>
	public class InformationBoxChangedEventArgs : EventArgs
	{
		private InformationBox _informationBox;

		internal InformationBoxChangedEventArgs(InformationBox informationBox)
		{
			_informationBox = informationBox;
		}

		/// <summary>
		/// Gets the <see cref="InformationBox"/> that has changed.
		/// </summary>
		public InformationBox InformationBox 
		{
			get { return _informationBox; }
		}
	}
}
