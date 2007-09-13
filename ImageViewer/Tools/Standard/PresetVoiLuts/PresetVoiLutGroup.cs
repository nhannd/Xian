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
