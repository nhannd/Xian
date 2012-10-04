#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace MyPlugin.TextEditor
{
	[ExtensionPoint()]
	public class TextEditorToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface ITextEditorToolContext : IToolContext
	{
		string Text { get; set; }
		int WordCount { get; }
	}
}