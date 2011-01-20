#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public abstract class DefaultPresetVoiLutOperationComponent : PresetVoiLutOperationComponent
	{
		protected DefaultPresetVoiLutOperationComponent()
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
				throw new InvalidOperationException(SR.ExceptionThePropertyCannotBeModified);
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
