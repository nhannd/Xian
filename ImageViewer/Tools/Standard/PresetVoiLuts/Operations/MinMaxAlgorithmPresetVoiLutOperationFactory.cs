#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	internal sealed class MinMaxAlgorithmPresetVoiLutOperationFactory : PresetVoiLutOperationFactory<MinMaxAlgorithmPresetVoiLutOperationComponent>
	{
		internal static readonly string FactoryName = "Min/Max Algorithm";

		public MinMaxAlgorithmPresetVoiLutOperationFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutOperationFactoryDescription; }
		}
	}
}