#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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