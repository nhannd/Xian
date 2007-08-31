using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public sealed class PresetVoiLut : IEquatable<PresetVoiLut>
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
			if (_keyStroke != XKeys.None && _keyStroke == other._keyStroke)
				return true;

			if (_applicator.Name == other._applicator.Name)
				return true;

			return false;
		}

		#endregion
	}
}
