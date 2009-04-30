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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutGroup : IEquatable<PresetVoiLutGroup>, IComparable<PresetVoiLutGroup>
	{
		private readonly string _modality;
		private readonly PresetVoiLutCollection _presets;
	
		public PresetVoiLutGroup(string modality)
		{
			Platform.CheckForEmptyString(modality, "modality");
			_modality = modality;
			_presets = new PresetVoiLutCollection();
		}

		public string Modality
		{
			get { return _modality; }	
		}

		public PresetVoiLutCollection Presets
		{
			get { return _presets; }
		}

		public bool AppliesTo(ImageSop sop)
		{
			return sop.Modality == _modality;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PresetVoiLutGroup)
				return this.Equals((PresetVoiLutGroup) obj);

			return false;
		}

		#region IEquatable<PresetVoiLutGroup> Members

		public bool Equals(PresetVoiLutGroup other)
		{
			if (other == null)
				return false;

			return this._modality == other._modality;
		}

		#endregion

		#region IComparable<PresetVoiLutGroup> Members

		public int CompareTo(PresetVoiLutGroup other)
		{
			return this.Modality.CompareTo(other.Modality);
		}

		#endregion

		internal PresetVoiLutGroup Clone()
		{
			PresetVoiLutGroup clone = new PresetVoiLutGroup(this.Modality);
			foreach (PresetVoiLut preset in _presets)
				clone._presets.Add(preset.Clone());

			return clone;
		}
	}
}
