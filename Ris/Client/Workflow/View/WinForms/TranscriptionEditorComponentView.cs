#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="TranscriptionEditorComponent"/>.
	/// </summary>
	[ExtensionOf(typeof(TranscriptionEditorComponentViewExtensionPoint))]
	public class TranscriptionEditorComponentView : ReportEditorComponentViewBase
	{
	}
}
