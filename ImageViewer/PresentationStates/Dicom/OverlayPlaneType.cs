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

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Indicates the type of data represented by the overlay plane.
	/// </summary>
	public sealed class OverlayPlaneType
	{
		/// <summary>
		/// Indicates that the overlay plane represents some generic graphics.
		/// </summary>
		public static readonly OverlayPlaneType Graphics = new OverlayPlaneType(OverlayType.G, "Graphics");

		/// <summary>
		/// Indicates that the overlay plane represents a region of interest.
		/// </summary>
		public static readonly OverlayPlaneType ROI = new OverlayPlaneType(OverlayType.R, "ROI");

		/// <summary>
		/// Gets the DICOM enumerated value for the overlay plane type.
		/// </summary>
		public readonly OverlayType OverlayType;

		/// <summary>
		/// Gets a string description of the overlay plane type.
		/// </summary>
		public readonly string Description;

		private OverlayPlaneType(OverlayType type, string description)
		{
			this.OverlayType = type;
			this.Description = description;
		}

		/// <summary>
		/// Gets a hash code for the type suitable of hash tables.
		/// </summary>
		public override int GetHashCode()
		{
			return this.OverlayType.GetHashCode();
		}

		/// <summary>
		/// Tests to see if this object is equivalent to <paramref name="obj">another</paramref> <see cref="OverlayPlaneType"/> or <see cref="OverlayType"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is OverlayPlaneType)
				return this.OverlayType == ((OverlayPlaneType) obj).OverlayType;
			else if (obj is OverlayType)
				return this.OverlayType == (OverlayType) obj;
			return false;
		}

		/// <summary>
		/// Gets a string that represents the current <see cref="OverlayPlaneType"/>.
		/// </summary>
		public override string ToString()
		{
			return this.Description;
		}

		/// <summary>
		/// Implicitly casts <paramref name="type"/> as a <see cref="OverlayType"/>.
		/// </summary>
		public static implicit operator OverlayType(OverlayPlaneType type)
		{
			if (type == null)
				return OverlayType.None;
			return type.OverlayType;
		}

		/// <summary>
		/// Implicitly casts <paramref name="type"/> as a <see cref="OverlayPlaneType"/>.
		/// </summary>
		public static implicit operator OverlayPlaneType(OverlayType type)
		{
			switch (type)
			{
				case OverlayType.G:
					return Graphics;
				case OverlayType.R:
					return ROI;
				case OverlayType.None:
				default:
					return null;
			}
		}
	}
}