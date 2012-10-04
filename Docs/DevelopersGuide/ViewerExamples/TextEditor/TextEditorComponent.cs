#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Validation;

namespace MyPlugin.TextEditor
{
	[ExtensionPoint()]
	public class TextEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (TextEditorComponentViewExtensionPoint))]
	public class TextEditorComponent : ApplicationComponent
	{
		private ToolSet _toolSet;
		private string _filename;
		private string _text;
		private int _wordCount;

		public TextEditorComponent()
		{
			_toolSet = new ToolSet(
				new TextEditorToolExtensionPoint(),
				new TextEditorToolContext(this));
		}

		[ValidateFilename(Message = "Invalid filename.")]
		public string Filename
		{
			get { return _filename; }
			set
			{
				_filename = value;
				this.Modified = true;
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				this.Modified = true;
				UpdateWordCount();
			}
		}

		public int WordCount
		{
			get { return _wordCount; }
			private set
			{
				_wordCount = value;
				NotifyPropertyChanged("WordCount");
			}
		}

		[ValidationMethodFor("Text")]
		private ValidationResult ValidateMinimumWordCount()
		{
			if (_wordCount < 3)
				return new ValidationResult(false, "There must be 3 or more words.");
			else
				return new ValidationResult(true, "");
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		public void Save()
		{
			if (base.HasValidationErrors)
			{
				base.ShowValidation(true);
			}
			else
			{
				File.WriteAllText(_filename, _text);
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		private void UpdateWordCount()
		{
			// really simple algorithm to count words for illustrative purposes only
			// by assuming spaces, period, comma, or (semi)colon between words
			char[] wordSeparators = new char[] {' ', '.', ',', ';', ':'};
			int wordCount = this.Text.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries).Length;
			this.WordCount = wordCount;
		}

		private class TextEditorToolContext : ToolContext, ITextEditorToolContext
		{
			private readonly TextEditorComponent _owner;

			public TextEditorToolContext(TextEditorComponent owner)
			{
				_owner = owner;
			}

			public string Text
			{
				get { return _owner.Text; }
				set { _owner.Text = value; }
			}

			public int WordCount
			{
				get { return _owner.WordCount; }
			}
		}
	}
}