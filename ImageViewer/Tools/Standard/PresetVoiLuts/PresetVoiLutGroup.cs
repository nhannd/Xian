using System;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutGroupSortByModality : IComparer<PresetVoiLutGroup>
	{
		#region IComparer<string> Members

		public int Compare(PresetVoiLutGroup x, PresetVoiLutGroup y)
		{
			//put "" (default modality) to the end.
			if (String.IsNullOrEmpty(x.Modality))
			{
				if (String.IsNullOrEmpty(y.Modality))
					return 0;

				return 1;
			}
			else if (String.IsNullOrEmpty(y.Modality))
				return -1;

			return String.Compare(x.Modality, y.Modality);
		}

		#endregion
	}

	internal sealed class PresetVoiLutGroup : IEquatable<PresetVoiLutGroup>
	{
		private readonly string _modality;
		private readonly PresetVoiLutCollection _presets;
	
		public PresetVoiLutGroup(string modality)
		{
			_modality = modality ?? "";
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

		internal PresetVoiLutGroup Clone()
		{
			PresetVoiLutGroup clone = new PresetVoiLutGroup(this.Modality);
			foreach (PresetVoiLut preset in _presets)
				clone._presets.Add(preset.Clone());

			return clone;
		}
	}
}
