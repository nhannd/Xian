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
