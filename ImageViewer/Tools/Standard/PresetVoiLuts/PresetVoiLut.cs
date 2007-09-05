using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLut : IEquatable<PresetVoiLut>
	{
		private XKeys _keyStroke;
		private readonly IPresetVoiLutApplicator _applicator;

		public PresetVoiLut(IPresetVoiLutApplicator applicator)
		{
			Platform.CheckForNullReference(applicator, "applicator");
			this._applicator = applicator;
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

		public IPresetVoiLutApplicator Applicator
		{
			get { return _applicator; }
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
			return (String.Compare(this.Applicator.Name, other.Applicator.Name, true) == 0 || (KeyStroke != XKeys.None && KeyStroke == other.KeyStroke));
		}

		#endregion

		internal PresetVoiLut Clone()
		{
			PresetVoiLut clone = new PresetVoiLut(_applicator);
			clone.KeyStroke = this.KeyStroke;
			return clone;
		}
	}
}
