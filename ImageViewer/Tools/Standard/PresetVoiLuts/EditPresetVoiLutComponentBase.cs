using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public abstract class EditPresetVoiLutComponentBase<T> : ApplicationComponent, IEditPresetVoiLutApplicationComponent
		where T : PresetVoiLutApplicator
	{
		private T _presetApplicator;
		private bool _valid;

		public EditPresetVoiLutComponentBase()
		{
			_valid = false;
		}

		protected T PresetApplicator
		{
			get { return _presetApplicator; }
		}

		internal void SetPresetApplicator(T applicator)
		{
			Platform.CheckForNullReference(applicator, "applicator");
			_presetApplicator = applicator;
		}

		#region IEditPresetVoiLutApplicationComponent Members

		public IPresetVoiLutApplicator GetApplicator()
		{
			_presetApplicator.Validate();
			return _presetApplicator;
		}

		public bool Valid
		{
			get { return _valid; }	
			protected set
			{
				if (value == _valid)
					return;

				_valid = value;
				NotifyPropertyChanged("Valid");
			}
		}

		#endregion
	}
}
