#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	/// <summary>
	/// An implementation of <see cref="IReservedActionModelKeyStrokeProvider"/> that reserves F2 (the auto VOI LUT applicator) and all other preset VOI LUT keystrokes.
	/// </summary>
	[ExtensionOf(typeof (ReservedActionModelKeyStrokeProviderExtensionPoint))]
	internal sealed class PresetVoiLutReservedActionModelKeyStrokesProvider : ReservedActionModelKeyStrokeProviderBase
	{
		public override IEnumerable<XKeys> ReservedKeyStrokes
		{
			get { return new List<XKeys>(AvailablePresetVoiLutKeyStrokeSettings.Default.GetAvailableKeyStrokes()) {XKeys.F2}; }
		}
	}
}