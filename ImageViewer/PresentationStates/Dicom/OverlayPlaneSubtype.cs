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
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents defined terms for identifying the intended purpose of the <see cref="OverlayPlaneType"/>.
	/// </summary>
	public class OverlayPlaneSubtype
	{
		/// <summary>
		/// Identifies that the overlay is a user created graphic annotation (e.g. operator).
		/// </summary>
		public static readonly OverlayPlaneSubtype User = new OverlayPlaneSubtype(OverlaySubtype.User);

		/// <summary>
		/// Identifies that the overlay is a machine or algorithm generated graphic annotation, such as output of a Computer Assisted Diagnosis algorithm.
		/// </summary>
		public static readonly OverlayPlaneSubtype Automated = new OverlayPlaneSubtype(OverlaySubtype.Automated);

		/// <summary>
		/// Gets the represented defined term.
		/// </summary>
		public readonly OverlaySubtype OverlaySubtype;

		/// <summary>
		/// Constructs a defined term.
		/// </summary>
		/// <param name="definedTerm">The defined term.</param>
		public OverlayPlaneSubtype(string definedTerm)
		{
			if (string.IsNullOrEmpty(definedTerm))
				throw new ArgumentNullException("definedTerm");
			this.OverlaySubtype = new CustomOverlaySubtype(definedTerm);
		}

		/// <summary>
		/// Constructs a defined term.
		/// </summary>
		/// <param name="definedTerm">The defined term.</param>
		public OverlayPlaneSubtype(OverlaySubtype definedTerm)
		{
			if (definedTerm == null)
				throw new ArgumentNullException("definedTerm");
			this.OverlaySubtype = definedTerm;
		}

		/// <summary>
		/// Gets a hash code for the defined term suitable of hash tables.
		/// </summary>
		public override sealed int GetHashCode()
		{
			return this.OverlaySubtype.GetHashCode();
		}

		/// <summary>
		/// Tests to see if this object is equivalent to <paramref name="obj">another</paramref> <see cref="OverlayPlaneSubtype"/> or <see cref="OverlaySubtype"/>.
		/// </summary>
		public override sealed bool Equals(object obj)
		{
			if (obj is OverlayPlaneSubtype)
				return this.OverlaySubtype.Equals(((OverlayPlaneSubtype) obj).OverlaySubtype);
			else if (obj is OverlaySubtype)
				return this.OverlaySubtype.Equals(obj);
			return false;
		}

		/// <summary>
		/// Gets a string that represents the current <see cref="OverlayPlaneSubtype"/>.
		/// </summary>
		public override sealed string ToString()
		{
			return this.OverlaySubtype.ToString();
		}

		/// <summary>
		/// Implicitly casts <paramref name="type"/> as a <see cref="OverlaySubtype"/>.
		/// </summary>
		public static implicit operator OverlaySubtype(OverlayPlaneSubtype type)
		{
			if (type == null)
				return null;
			return type.OverlaySubtype;
		}

		/// <summary>
		/// Explicitly casts <paramref name="type"/> as a <see cref="OverlayPlaneSubtype"/>.
		/// </summary>
		public static explicit operator OverlayPlaneSubtype(OverlaySubtype type)
		{
			if (type == null)
				return null;
			return new OverlayPlaneSubtype(type);
		}

		private class CustomOverlaySubtype : OverlaySubtype
		{
			public CustomOverlaySubtype(string definedTerm) : base(definedTerm) {}
		}
	}
}