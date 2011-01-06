#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLut : IEquatable<PresetVoiLut>
	{
		private KeyStrokeDescriptor _keyStrokeDescriptor;
		private readonly IPresetVoiLutOperation _operation;

		public PresetVoiLut(IPresetVoiLutOperation operation)
		{
			Platform.CheckForNullReference(operation, "operation");
			this._operation = operation;
			_keyStrokeDescriptor = XKeys.None;
		}

		public KeyStrokeDescriptor KeyStrokeDescriptor
		{
			get { return _keyStrokeDescriptor; }	
		}

		public XKeys KeyStroke
		{
			get { return _keyStrokeDescriptor.KeyStroke; }
			set { _keyStrokeDescriptor = value; }
		}

		public IPresetVoiLutOperation Operation
		{
			get { return _operation; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			if (obj is PresetVoiLut)
				return this.Equals((PresetVoiLut) obj);

			return false;
		}

		#region IEquatable<PresetVoiLut> Members

		public bool Equals(PresetVoiLut other)
		{
			if (other == null)
				return false;

			return (String.Compare(this.Operation.Name, other.Operation.Name, true) == 0 || (KeyStroke != XKeys.None && KeyStroke == other.KeyStroke));
		}

		#endregion

		public PresetVoiLut Clone()
		{
			PresetVoiLut clone = new PresetVoiLut(_operation);
			clone.KeyStroke = this.KeyStroke;
			return clone;
		}
	}
}
