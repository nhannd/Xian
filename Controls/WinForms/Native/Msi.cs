#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// ReSharper disable InconsistentNaming

using System.Runtime.InteropServices;
using System.Text;

namespace ClearCanvas.Controls.WinForms.Native
{
	internal static class Msi
	{
		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		public static extern uint MsiGetShortcutTargetW(
			string szShortcutTarget,
			[Out] StringBuilder szProductCode,
			[Out] StringBuilder szFeatureId,
			[Out] StringBuilder szComponentCode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		public static extern uint MsiGetComponentPath(
			string szProduct,
			string szComponent,
			[Out] StringBuilder lpPathBuf,
			ref uint pcchBuf);
	}
}

// ReSharper restore InconsistentNaming