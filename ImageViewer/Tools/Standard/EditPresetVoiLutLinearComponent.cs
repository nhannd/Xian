using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="PresetVoiLutLinearEditComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class EditPresetVoiLutLinearComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PresetVoiLutLinearEditComponent class
	/// </summary>
	[AssociateView(typeof(EditPresetVoiLutLinearComponentViewExtensionPoint))]
	public class EditPresetVoiLutLinearComponent : EditPresetVoiLutComponentBase<PresetVoiLutLinearApplicator>
	{
		public EditPresetVoiLutLinearComponent()
		{
		}

		public string Name
		{
			get { return base.PresetApplicator.PresetName; }
			set
			{
				if (base.PresetApplicator.PresetName == value)
					return;

				base.PresetApplicator.PresetName = value;
				NotifyPropertyChanged("Name");
				this.Modified = true;
				UpdateValid();
			}
		}

		public double WindowWidth
		{
			get { return base.PresetApplicator.WindowWidth; }
			set
			{
				if (base.PresetApplicator.WindowWidth == value)
					return;

				base.PresetApplicator.WindowWidth = value;
				NotifyPropertyChanged("WindowWidth");
				this.Modified = true;
				UpdateValid();
			}
		}

		public double WindowCenter
		{
			get { return base.PresetApplicator.WindowCenter; }
			set
			{
				if (base.PresetApplicator.WindowCenter == value)
					return;

				base.PresetApplicator.WindowCenter = value;
				NotifyPropertyChanged("WindowCenter");
				this.Modified = true;
				UpdateValid();
			}
		}
		
		public override void Start()
		{
			if (this.WindowWidth < 1 || double.IsNaN(this.WindowWidth))
				this.WindowWidth = 1;
			if (double.IsNaN(this.WindowCenter))
				this.WindowCenter = 0;

			base.Start();
		}

		private void UpdateValid()
		{
			bool valid = true;
			if (String.IsNullOrEmpty(this.Name))
				valid = false;

			if (this.WindowWidth < 1 || double.IsNaN(this.WindowWidth))
				valid = false;

			if (double.IsNaN(this.WindowCenter))
				valid = false;

			base.Valid = valid;
		}
	}
}
