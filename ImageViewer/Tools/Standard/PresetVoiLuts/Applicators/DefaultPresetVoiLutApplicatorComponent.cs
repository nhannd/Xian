using System;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	public abstract class DefaultPresetVoiLutApplicatorComponent : PresetVoiLutApplicatorComponent
	{
		protected DefaultPresetVoiLutApplicatorComponent()
		{
			this.Valid = true;
		}

		#region Sealed Off Application Component functionality

		public sealed override bool Modified
		{
			get
			{
				return false;
			}
			protected set
			{
				throw new InvalidOperationException("The property cannot be modified.");
			}
		}

		public sealed override bool HasValidationErrors
		{
			get
			{
				return false;
			}
		}

		public sealed override void ShowValidation(bool show)
		{
		}

		public sealed override void Start()
		{
			base.Start();
		}

		public sealed override void Stop()
		{
			base.Stop();
		}

		#endregion

		public sealed override void Validate()
		{
		}
	}
}
