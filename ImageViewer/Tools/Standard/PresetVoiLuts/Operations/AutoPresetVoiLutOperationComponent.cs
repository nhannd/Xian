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
	public sealed class AutoPresetVoiLutOperationComponent : DefaultPresetVoiLutOperationComponent
	{
		public AutoPresetVoiLutOperationComponent() {}

		public override string Name
		{
			get { return SR.AutoPresetVoiLutOperationName; }
		}

		public override string Description
		{
			get { return SR.AutoPresetVoiLutOperationDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
		    return AutoVoiLutApplicator.CanCreate(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
            var applicator = AutoVoiLutApplicator.Create(presentationImage);
            if (applicator == null)
				throw new InvalidOperationException("The input presentation image is not supported.");

		    applicator.ApplyNextLut();
		}
	}
}